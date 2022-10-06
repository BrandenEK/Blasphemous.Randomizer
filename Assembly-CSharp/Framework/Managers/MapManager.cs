using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Framework.FrameworkCore;
using I2.Loc;
using Tools.DataContainer;
using Tools.Level;
using Tools.Level.Actionables;
using Tools.Level.Interactables;
using UnityEngine;

namespace Framework.Managers
{
	public class MapManager : GameSystem, PersistentInterface
	{
		public string CurrentDomain { get; private set; }

		public string CurrentZone { get; private set; }

		public Vector3 playerMapOffset { get; set; }

		public override void Initialize()
		{
			LocalizationManager.OnLocalizeEvent += new LocalizationManager.OnLocalizeCallback(this.OnLocalizationChange);
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			LevelManager.OnBeforeLevelLoad += this.OnBeforeLevelLoad;
			this.CurrentDomain = string.Empty;
			this.CurrentZone = string.Empty;
			this.currentLanguage = string.Empty;
			this.LoadAllMaps();
			this.cacheObjects.Clear();
			this.OnLocalizationChange();
			this.playerMapOffset = Vector3.zero;
		}

		public override void Dispose()
		{
			this.cacheObjects.Clear();
			LocalizationManager.OnLocalizeEvent -= new LocalizationManager.OnLocalizeCallback(this.OnLocalizationChange);
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			LevelManager.OnBeforeLevelLoad -= this.OnBeforeLevelLoad;
		}

		private void OnBeforeLevelLoad(Level oldLevel, Level newLevel)
		{
			this.cacheObjects.Clear();
			this.playerMapOffset = Vector3.zero;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			string levelName = newLevel.LevelName;
			this.cacheObjects.Clear();
			this.FillSceneCacheObject<Door>();
			this.FillSceneCacheObject<PrieDieu>();
			this.FillSceneCacheObject<Gate>();
			this.FillSceneCacheObject<Teleport>();
			if (levelName == "NONE")
			{
				return;
			}
			string empty = string.Empty;
			string empty2 = string.Empty;
			if (this.GetDomainAndZoneFromBundleName(levelName, ref empty, ref empty2) && Core.Logic.CurrentLevelConfig.ShowZoneTitle(oldLevel) && empty + empty2 != this.CurrentDomain + this.CurrentZone)
			{
				string zoneName = this.GetZoneName(empty, empty2);
				if (zoneName != string.Empty)
				{
					this.DisplayZoneName(zoneName);
					Core.AchievementsManager.AddProgressToAC21(empty, empty2);
				}
			}
			this.CurrentDomain = empty;
			this.CurrentZone = empty2;
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
		}

		public string GetZoneNameFromBundle(string bundle)
		{
			string result = string.Empty;
			string empty = string.Empty;
			string empty2 = string.Empty;
			if (this.GetDomainAndZoneFromBundleName(bundle, ref empty, ref empty2))
			{
				result = this.GetZoneName(empty, empty2);
			}
			return result;
		}

		public void DisplayZoneName(string zoneName)
		{
		}

		public MapManager.DataMapReveal GetNearestZone(Vector2 mapPos)
		{
			MapManager.DataMapReveal dataMapReveal = null;
			float num = 0f;
			foreach (MapManager.DataMapReveal dataMapReveal2 in this.mapReveal.Values)
			{
				float sqrMagnitude = (dataMapReveal2.GetWorldPosition2() - mapPos).sqrMagnitude;
				bool flag = dataMapReveal2.CellContainsMapPos(mapPos);
				if (flag && (dataMapReveal == null || sqrMagnitude < num))
				{
					num = sqrMagnitude;
					dataMapReveal = dataMapReveal2;
				}
			}
			return dataMapReveal;
		}

		public MapManager.DataMapReveal GetCurrentZone()
		{
			return this.GetZone(this.CurrentDomain, this.CurrentZone);
		}

		public List<string> GetZonesList()
		{
			return this.mapReveal.Keys.ToList<string>();
		}

		public MapManager.DataMapReveal GetZone(string key)
		{
			if (this.mapReveal.ContainsKey(key))
			{
				return this.mapReveal[key];
			}
			return null;
		}

		public MapManager.DataMapReveal GetZone(string domain, string zone)
		{
			return this.GetZone(this.GetDictKey(domain, zone));
		}

		public void DEBUG_RevealAllMaps(int numberOfReveals = 10)
		{
			foreach (MapManager.DataMapReveal dataMapReveal in this.mapReveal.Values)
			{
				Graphics.CopyTexture(dataMapReveal.map.texture, dataMapReveal.mask.texture);
				dataMapReveal.mask = Sprite.Create(dataMapReveal.mask.texture, dataMapReveal.mask.rect, new Vector2(0.5f, 0.5f));
				foreach (MapManager.DataMapCell dataMapCell in dataMapReveal.cells)
				{
					dataMapCell.crawled = true;
				}
				dataMapReveal.elements.Clear();
				for (int i = 0; i < numberOfReveals; i++)
				{
					MapManager.ElementsRevealed value = new MapManager.ElementsRevealed(Vector3.zero, MapManager.MapElementType.Door);
					dataMapReveal.elements[dataMapReveal.domain + dataMapReveal.zone + "_" + i.ToString()] = value;
				}
				dataMapReveal.updatedAnyTime = true;
			}
		}

		public bool DigCurrentMask(Vector3 worldPosition, bool forceUpdate = false)
		{
			Vector3 worldPosition2 = worldPosition + this.playerMapOffset;
			bool result = false;
			MapManager.DataMapReveal zone = this.GetZone(this.CurrentDomain, this.CurrentZone);
			if (zone != null)
			{
				bool flag = false;
				foreach (MapManager.DataMapCell dataMapCell in zone.MarkAndGetMapCellFromWorld(worldPosition2, forceUpdate))
				{
					Rect textureBounds = dataMapCell.textureBounds;
					if (textureBounds.size.x > 0f && textureBounds.min.x > 0f && textureBounds.max.x < (float)zone.width && textureBounds.size.y > 0f && textureBounds.min.y > 0f && textureBounds.max.y < (float)zone.height)
					{
						Color[] pixels = zone.map.texture.GetPixels((int)textureBounds.min.x, (int)textureBounds.min.y, (int)textureBounds.size.x, (int)textureBounds.size.y);
						zone.mask.texture.SetPixels((int)textureBounds.min.x, (int)textureBounds.min.y, (int)textureBounds.size.x, (int)textureBounds.size.y, pixels);
						zone.mask.texture.Apply();
						zone.mask = Sprite.Create(zone.mask.texture, zone.mask.rect, new Vector2(0.5f, 0.5f));
						result = true;
						zone.CheckNewElements(dataMapCell.worldBounds, this.cacheObjects);
						flag = true;
					}
					else
					{
						Debug.LogError("*** DigCurrentMask texture asking outside bounds: " + zone.domain + zone.zone);
					}
				}
				if (flag)
				{
					zone.updated = true;
					zone.updatedAnyTime = true;
				}
				zone.UpdateElementsStatus();
			}
			return result;
		}

		public string GetCurrentDomainName()
		{
			return this.GetDomainName(this.CurrentDomain);
		}

		public string GetCurrentZoneName()
		{
			return this.GetZoneName(this.CurrentDomain, this.CurrentZone);
		}

		public string GetDomainName(string domain)
		{
			string result = "![NO_LOC_" + domain + "]";
			if (this.domainLocalization.ContainsKey(domain))
			{
				result = this.domainLocalization[domain];
			}
			return result;
		}

		public string GetZoneName(string domain, string zone)
		{
			string dictKey = this.GetDictKey(domain, zone);
			string text = "![ERROR NO KEY " + dictKey + "]";
			if (this.zonesLocalization.ContainsKey(dictKey))
			{
				text = this.zonesLocalization[dictKey];
				if (text.Trim().Length == 0)
				{
					text = "[!ERROR NO ZONE NAME " + dictKey + "]";
				}
			}
			return text;
		}

		private bool GetDomainAndZoneFromBundleName(string bundle, ref string domain, ref string zone)
		{
			Regex regex = new Regex("^D(?<domain>[0-9]{2})Z(?<zone>[0-9]{2})S(?<scene>[0-9]{2})$");
			Match match = regex.Match(bundle);
			if (match.Success)
			{
				domain = "D" + match.Groups["domain"].Value;
				zone = "Z" + match.Groups["zone"].Value;
			}
			return match.Success;
		}

		private string GetDictKey(string domain, string zone)
		{
			return domain + "_" + zone;
		}

		private void LoadAllMaps()
		{
		}

		private void FillSceneCacheObject<T>() where T : PersistentObject
		{
			Type typeFromHandle = typeof(T);
			foreach (T t in Object.FindObjectsOfType<T>())
			{
				if (!this.cacheObjects.ContainsKey(typeFromHandle))
				{
					this.cacheObjects[typeFromHandle] = new List<PersistentObject>();
				}
				this.cacheObjects[typeFromHandle].Add(t);
			}
		}

		private void OnLocalizationChange()
		{
			if (this.currentLanguage != LocalizationManager.CurrentLanguage)
			{
				if (this.currentLanguage != string.Empty)
				{
					Log.Debug("MapManager", "Language change, localize items to: " + LocalizationManager.CurrentLanguage, null);
				}
				this.currentLanguage = LocalizationManager.CurrentLanguage;
				LanguageSource mainLanguageSource = LocalizationManager.GetMainLanguageSource();
				int languageIndexFromCode = mainLanguageSource.GetLanguageIndexFromCode(LocalizationManager.CurrentLanguageCode, true);
				this.domainLocalization.Clear();
				this.zonesLocalization.Clear();
			}
		}

		public int GetOrder()
		{
			return 10;
		}

		public string GetPersistenID()
		{
			return "ID_MAPS";
		}

		public void ResetPersistence()
		{
			this.LoadAllMaps();
			this.playerMapOffset = Vector3.zero;
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			return new MapManager.MapPersistenceData();
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			MapManager.MapPersistenceData mapPersistenceData = (MapManager.MapPersistenceData)data;
			foreach (KeyValuePair<string, MapManager.MapPersistenceDataItem> keyValuePair in mapPersistenceData.reveal)
			{
				string[] array = keyValuePair.Key.Split(new char[]
				{
					'_'
				});
				if (array.Length == 2)
				{
					Core.NewMapManager.RevealAllZone(array[0], array[1]);
				}
			}
		}

		private Dictionary<string, string> domainLocalization = new Dictionary<string, string>();

		private Dictionary<string, string> zonesLocalization = new Dictionary<string, string>();

		private const string MAP_RESOURCE_CONFIG = "Maps/MapData";

		private MapData mapData;

		private Dictionary<string, MapManager.DataMapReveal> mapReveal = new Dictionary<string, MapManager.DataMapReveal>();

		private const string REG_EXP = "^D(?<domain>[0-9]{2})Z(?<zone>[0-9]{2})S(?<scene>[0-9]{2})$";

		private string currentLanguage = string.Empty;

		private Dictionary<Type, List<PersistentObject>> cacheObjects = new Dictionary<Type, List<PersistentObject>>();

		private const string PERSITENT_ID = "ID_MAPS";

		public class DataMapCell
		{
			public DataMapCell(MapManager.DataMapReveal reference, Bounds cell)
			{
				this.worldBounds = new Bounds(new Vector3(cell.center.x, cell.center.y, 0f), new Vector3(cell.size.x, cell.size.y, 100f));
				Vector2 vector = reference.WorldToMaskCoordinates(cell.min);
				vector..ctor(Mathf.Floor(vector.x), Mathf.Floor(vector.y));
				Vector2 vector2 = reference.WorldToMaskCoordinates(cell.max);
				vector2..ctor(Mathf.Ceil(vector2.x), Mathf.Ceil(vector2.y));
				Vector2 vector3 = vector2 - vector;
				this.textureBounds = new Rect(vector.x, vector.y, vector3.x, vector3.y);
				Vector3 vector4 = reference.WorldToTexture(cell.center);
				Vector3 vector5 = reference.WorldToTexture(cell.size);
				this.mapBounds = new Bounds(new Vector3(vector4.x, vector4.y, 0f), new Vector3(vector5.x, vector5.y, 100f));
			}

			public Bounds worldBounds { get; private set; }

			public Rect textureBounds { get; private set; }

			public Bounds mapBounds { get; private set; }

			public bool crawled;
		}

		public enum MapElementType
		{
			Reclinatory,
			Gate,
			Door,
			Teleport,
			MeaCulpa
		}

		public class ElementsRevealed
		{
			public ElementsRevealed(Vector3 position, MapManager.MapElementType typeEleement)
			{
				this.pos = position;
				this.element = typeEleement;
				this.activatedOrOpen = false;
			}

			public Vector3 pos { get; private set; }

			public MapManager.MapElementType element { get; private set; }

			public bool activatedOrOpen;
		}

		public class DataMapReveal
		{
			public Vector3 GetWorldPosition()
			{
				return this.WorldToTexture(new Vector3(this.pos.x, this.pos.y, 0f));
			}

			public Vector2 GetWorldPosition2()
			{
				return this.WorldToTexture(new Vector2(this.pos.x, this.pos.y));
			}

			public Vector3 WorldToTexture(Vector3 world)
			{
				return new Vector3(world.x * this.orthogonalFactor, world.y * this.orthogonalFactor, world.z);
			}

			public Vector2 WorldToMaskCoordinates(Vector3 world)
			{
				Vector3 vector = world - this.pos;
				vector *= this.orthogonalFactor;
				vector += new Vector3((float)this.width / 2f, (float)this.height / 2f, 0f);
				return new Vector2(vector.x, vector.y);
			}

			public List<MapManager.DataMapCell> MarkAndGetMapCellFromWorld(Vector3 worldPosition, bool forceUpdate = false)
			{
				List<MapManager.DataMapCell> list = new List<MapManager.DataMapCell>();
				Vector3 vector;
				vector..ctor(worldPosition.x, worldPosition.y, 0f);
				foreach (MapManager.DataMapCell dataMapCell in this.cells)
				{
					if (!dataMapCell.crawled || forceUpdate)
					{
						if (dataMapCell.worldBounds.Contains(vector))
						{
							dataMapCell.crawled = true;
							list.Add(dataMapCell);
						}
					}
				}
				return list;
			}

			public bool CellContainsMapPos(Vector2 mapPos)
			{
				Vector3 vector;
				vector..ctor(mapPos.x, mapPos.y, 0f);
				bool flag = this.mapBounds.Contains(vector);
				if (flag)
				{
					flag = false;
					foreach (MapManager.DataMapCell dataMapCell in this.cells)
					{
						if (dataMapCell.mapBounds.Contains(vector))
						{
							flag = true;
							break;
						}
					}
				}
				return flag;
			}

			public void CheckNewElements(Bounds worldBounds, Dictionary<Type, List<PersistentObject>> sceneData)
			{
				this.CheckElements<Door>(MapManager.MapElementType.Door, worldBounds, sceneData);
				this.CheckElements<PrieDieu>(MapManager.MapElementType.Reclinatory, worldBounds, sceneData);
				this.CheckElements<Gate>(MapManager.MapElementType.Gate, worldBounds, sceneData);
				this.CheckElements<Teleport>(MapManager.MapElementType.Teleport, worldBounds, sceneData);
			}

			public void UpdateElementsStatus()
			{
				foreach (PersistentObject persistentObject in Object.FindObjectsOfType<PersistentObject>())
				{
					if (this.elements.ContainsKey(persistentObject.GetPersistenID()))
					{
						this.elements[persistentObject.GetPersistenID()].activatedOrOpen = persistentObject.IsOpenOrActivated();
					}
				}
			}

			private void CheckElements<T>(MapManager.MapElementType elementType, Bounds worldBounds, Dictionary<Type, List<PersistentObject>> sceneData) where T : PersistentObject
			{
				Type typeFromHandle = typeof(T);
				if (!sceneData.ContainsKey(typeFromHandle))
				{
					return;
				}
				foreach (PersistentObject persistentObject in sceneData[typeFromHandle])
				{
					if (!(persistentObject == null) && !this.elements.ContainsKey(persistentObject.GetPersistenID()))
					{
						bool flag = true;
						if (typeFromHandle == typeof(Door))
						{
							Door door = (Door)persistentObject;
							flag = !door.autoEnter;
						}
						else if (typeFromHandle == typeof(Teleport))
						{
							Teleport teleport = (Teleport)persistentObject;
							flag = teleport.showOnMap;
						}
						Vector3 position;
						if (flag && this.CheckAndGetObjectSafePoint(worldBounds, persistentObject.gameObject, out position))
						{
							MapManager.ElementsRevealed value = new MapManager.ElementsRevealed(position, elementType);
							this.elements[persistentObject.GetPersistenID()] = value;
						}
					}
				}
			}

			private bool CheckAndGetObjectSafePoint(Bounds worldBounds, GameObject obj, out Vector3 finalPos)
			{
				bool result = false;
				finalPos..ctor(obj.transform.position.x, obj.transform.position.y, 0f);
				if (worldBounds.Contains(finalPos))
				{
					result = true;
					Transform transform = obj.transform.Find("MAPELEMENT");
					if (transform && transform.gameObject != null && transform.gameObject.activeInHierarchy)
					{
						finalPos..ctor(transform.position.x, transform.position.y, 0f);
					}
				}
				return result;
			}

			public Sprite mask;

			public Sprite map;

			public Vector3 pos;

			public string domain = string.Empty;

			public string zone = string.Empty;

			public float orthogonalFactor;

			public int height;

			public int width;

			public bool updated;

			public bool updatedAnyTime;

			public Bounds mapBounds;

			public List<MapManager.DataMapCell> cells = new List<MapManager.DataMapCell>();

			public Dictionary<string, MapManager.ElementsRevealed> elements = new Dictionary<string, MapManager.ElementsRevealed>();
		}

		[Serializable]
		public class MapPersistenceDataElements
		{
			public Vector3 pos;

			public MapManager.MapElementType element;

			public bool activatedOrOpen;
		}

		[Serializable]
		public class MapPersistenceDataItem
		{
			public string filename;

			public Dictionary<string, MapManager.MapPersistenceDataElements> elements = new Dictionary<string, MapManager.MapPersistenceDataElements>();
		}

		[Serializable]
		public class MapPersistenceData : PersistentManager.PersistentData
		{
			public MapPersistenceData() : base("ID_MAPS")
			{
			}

			public Dictionary<string, MapManager.MapPersistenceDataItem> reveal = new Dictionary<string, MapManager.MapPersistenceDataItem>();
		}
	}
}
