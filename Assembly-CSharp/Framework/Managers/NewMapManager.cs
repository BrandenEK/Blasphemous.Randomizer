using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Framework.FrameworkCore;
using Framework.Map;
using Gameplay.UI;
using I2.Loc;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Managers
{
	public class NewMapManager : GameSystem, PersistentInterface
	{
		public ZoneKey CurrentScene { get; private set; }

		public ZoneKey CurrentSafeScene { get; private set; }

		public string DefaultMapID { get; private set; }

		public override void Initialize()
		{
			this.CreateLocaliztionDicts();
			LocalizationManager.OnLocalizeEvent += new LocalizationManager.OnLocalizeCallback(this.OnLocalizationChange);
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			this.CurrentLanguage = string.Empty;
			this.CurrentScene = new ZoneKey();
			this.LoadAllMaps();
			this.ZonesForAC21 = Resources.Load<ZonesAC21>("New Maps/ZonesAC21");
			this.OnLocalizationChange();
		}

		public override void Dispose()
		{
			LocalizationManager.OnLocalizeEvent -= new LocalizationManager.OnLocalizeCallback(this.OnLocalizationChange);
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			string levelName = newLevel.LevelName;
			if (levelName == "NONE")
			{
				return;
			}
			ZoneKey zoneKey = new ZoneKey();
			if (this.GetZoneKeyFromBundleName(levelName, ref zoneKey) && Core.Logic.CurrentLevelConfig.ShowZoneTitle(oldLevel) && this.CurrentScene.GetLocalizationKey() != zoneKey.GetLocalizationKey())
			{
				string zoneName = this.GetZoneName(zoneKey);
				if (zoneName != string.Empty && Core.LevelManager.InCinematicsChangeLevel == LevelManager.CinematicsChangeLevel.No)
				{
					this.CurrentSafeScene = zoneKey;
					this.DisplayZoneName(zoneName);
					this.AddProgressToAC21(zoneKey);
				}
			}
			this.CurrentScene = zoneKey;
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
		}

		public string GetCurrentDistrictName()
		{
			return this.GetDistrictName(this.CurrentScene.District);
		}

		public string GetCurrentZoneName()
		{
			return this.GetZoneName(this.CurrentScene);
		}

		public bool CanShowMapInCurrentZone()
		{
			bool result = false;
			if (this.CurrentScene != null && !GameConstants.DistrictsWithoutName.Contains(this.CurrentScene.District))
			{
				string localizationKey = this.CurrentScene.GetLocalizationKey();
				result = this.ZonesLocalization.ContainsKey(localizationKey);
			}
			return result;
		}

		public string GetDistrictName(string district)
		{
			string result = "![NO_LOC_" + district + "]";
			if (GameConstants.DistrictsWithoutName.Contains(district))
			{
				result = string.Empty;
			}
			else if (this.DistrictLocalization.ContainsKey(district))
			{
				result = this.DistrictLocalization[district];
			}
			return result;
		}

		public string GetZoneName(ZoneKey sceneKey)
		{
			string localizationKey = sceneKey.GetLocalizationKey();
			string text = "![ERROR NO KEY " + localizationKey + "]";
			if (GameConstants.DistrictsWithoutName.Contains(sceneKey.District))
			{
				text = string.Empty;
			}
			else if (this.ZonesLocalization.ContainsKey(localizationKey))
			{
				text = this.ZonesLocalization[localizationKey];
				if (text.Trim().Length == 0)
				{
					text = "[!ERROR NO LOC FOR " + localizationKey + "]";
				}
			}
			return text;
		}

		public bool ZoneHasName(ZoneKey sceneKey)
		{
			string localizationKey = sceneKey.GetLocalizationKey();
			return !GameConstants.DistrictsWithoutName.Contains(sceneKey.District) && this.ZonesLocalization.ContainsKey(localizationKey) && this.ZonesLocalization[localizationKey].Trim().Length > 0;
		}

		public string GetZoneNameFromBundle(string bundle)
		{
			string result = string.Empty;
			ZoneKey sceneKey = new ZoneKey();
			if (this.GetZoneKeyFromBundleName(bundle, ref sceneKey))
			{
				result = this.GetZoneName(sceneKey);
			}
			return result;
		}

		public void DisplayZoneName()
		{
			this.AddProgressToAC21(this.CurrentScene);
			this.DisplayZoneName(this.GetCurrentZoneName());
		}

		public void DisplayZoneName(string zone)
		{
			UIController.instance.ShowAreaPopUp(zone, 3f, false);
		}

		public List<SecretData> GetAllSecrets()
		{
			if (this.CurrentMap != null)
			{
				return this.CurrentMap.Secrets.Values.ToList<SecretData>();
			}
			return new List<SecretData>();
		}

		public bool SetSecret(string secretId, bool enable)
		{
			bool result = false;
			if (this.CurrentMap != null)
			{
				result = this.SetSecret(this.CurrentMap.Name, secretId, enable);
			}
			return result;
		}

		public bool SetSecret(string mapId, string secretId, bool enabled)
		{
			bool result = false;
			if (this.Maps.ContainsKey(mapId))
			{
				MapData mapData = this.Maps[mapId];
				if (mapData.Secrets.ContainsKey(secretId))
				{
					result = (enabled != mapData.Secrets[secretId].Revealed);
					mapData.Secrets[secretId].Revealed = enabled;
				}
			}
			return result;
		}

		public int GetTotalCells()
		{
			int result = 0;
			if (this.CurrentMap != null)
			{
				List<CellData> cells = this.CurrentMap.Cells;
				if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.NEW_GAME))
				{
					cells.RemoveAll((CellData cell) => cell.NGPlus);
				}
				IEnumerable<CellData> source = from cell in cells
				where !cell.IgnoredForMapPercentage
				select cell;
				result = source.Count<CellData>();
			}
			return result;
		}

		public CellKey GetPlayerCell()
		{
			if (this.LastPlayerCell != null)
			{
				return this.LastPlayerCell.CellKey;
			}
			return null;
		}

		public CellKey GetCellKeyFromPosition(string scene, Vector2 position)
		{
			CellKey result = null;
			ZoneKey key = new ZoneKey();
			if (this.GetZoneKeyFromBundleName(scene, ref key) && this.CurrentMap.CellsByZone.ContainsKey(key))
			{
				foreach (CellData cellData in this.CurrentMap.CellsByZone[key])
				{
					if (cellData.Bounding.Contains(position))
					{
						result = new CellKey(cellData.CellKey);
						break;
					}
				}
			}
			return result;
		}

		public CellKey GetCellKeyFromPosition(Vector2 position)
		{
			CellKey result = null;
			if (this.CurrentMap != null)
			{
				foreach (CellData cellData in this.Maps[this.DefaultMapID].Cells)
				{
					if (cellData.Bounding.Contains(position))
					{
						result = cellData.CellKey;
						break;
					}
				}
			}
			return result;
		}

		public List<CellData> GetAllRevealedCells()
		{
			return this.GetAllRevealedCells(this.CurrentMap);
		}

		public List<CellKey> GetAllRevealSecretsCells()
		{
			List<CellKey> list = new List<CellKey>();
			foreach (SecretData secretData in from x in this.CurrentMap.Secrets.Values
			where x.Revealed
			select x)
			{
				foreach (CellKey item in secretData.Cells.Keys)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public List<CellData> GetAllRevealedCells(string mapId)
		{
			MapData map = null;
			if (this.Maps.ContainsKey(mapId))
			{
				map = this.Maps[mapId];
			}
			return this.GetAllRevealedCells(map);
		}

		public List<string> GetAllMaps()
		{
			return this.Maps.Keys.ToList<string>();
		}

		public string GetCurrentMap()
		{
			string result = string.Empty;
			if (this.CurrentMap != null)
			{
				result = this.CurrentMap.Name;
			}
			return result;
		}

		public bool SetCurrentMap(string mapId)
		{
			if (this.Maps.ContainsKey(mapId))
			{
				this.CurrentMap = this.Maps[mapId];
				return true;
			}
			return false;
		}

		public void RevealCellInCurrentPlayerPosition()
		{
			if (Core.Logic.Penitent != null)
			{
				this.RevealCellInPosition(Core.Logic.Penitent.transform.position);
			}
		}

		public void RevealCellInPosition(Vector2 position)
		{
			if (this.CurrentMap == null || !this.CurrentMap.CellsByZone.ContainsKey(this.CurrentScene))
			{
				return;
			}
			bool flag = false;
			foreach (CellData cellData in this.CurrentMap.CellsByZone[this.CurrentScene])
			{
				if (cellData.Bounding.Contains(position))
				{
					flag = (flag || !cellData.Revealed);
					cellData.Revealed = true;
					this.LastPlayerCell = cellData;
				}
			}
			if (flag)
			{
				Core.AchievementsManager.CheckProgressToAC46();
			}
		}

		public void RevealAllMap()
		{
			foreach (CellData cellData in this.CurrentMap.Cells)
			{
				cellData.Revealed = true;
			}
			Core.AchievementsManager.CheckProgressToAC46();
		}

		public void RevealAllNGMap()
		{
			foreach (CellData cellData in this.CurrentMap.Cells)
			{
				if (!cellData.NGPlus)
				{
					cellData.Revealed = true;
				}
			}
			Core.AchievementsManager.CheckProgressToAC46();
		}

		public List<CellData> GetUnrevealedCellsForCompletion()
		{
			bool isNGMap = Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS);
			return (from c in this.CurrentMap.Cells
			where !c.Revealed && c.NGPlus == isNGMap && !c.IgnoredForMapPercentage
			select c).ToList<CellData>();
		}

		public void RevealAllDistrict(string district)
		{
			IEnumerable<CellData> enumerable = from p in this.CurrentMap.Cells
			where p.ZoneId.District == district
			select p;
			foreach (CellData cellData in enumerable)
			{
				cellData.Revealed = true;
			}
			Core.AchievementsManager.CheckProgressToAC46();
		}

		public void RevealAllZone(string district, string zone)
		{
			IEnumerable<CellData> enumerable = from p in this.CurrentMap.Cells
			where p.ZoneId.District == district && p.ZoneId.Zone == zone
			select p;
			foreach (CellData cellData in enumerable)
			{
				cellData.Revealed = true;
			}
			Core.AchievementsManager.CheckProgressToAC46();
		}

		public float GetPercentageCompletition()
		{
			float num = 0f;
			if (this.CurrentMap != null)
			{
				num = this.GetCellPercentage(false) * GameConstants.PercentageValues[PersistentManager.PercentageType.Map];
				if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS))
				{
					float num2 = this.GetCellPercentage(true) * GameConstants.PercentageValues[PersistentManager.PercentageType.Map_NgPlus];
					num += num2;
				}
			}
			return num;
		}

		public bool CanAddMark(CellKey key)
		{
			bool flag = false;
			if (this.CurrentMap != null && this.CurrentMap.CellsByCellKey.ContainsKey(key))
			{
				CellData cellData = this.CurrentMap.CellsByCellKey[key];
				flag = (cellData.Revealed && cellData.Type == EditorMapCellData.CellType.Normal);
				flag = (flag && !this.CurrentMap.Marks.ContainsKey(key));
			}
			return flag;
		}

		public bool IsMarkOnCell(CellKey key)
		{
			bool result = false;
			if (this.CurrentMap != null && this.CurrentMap.Marks.ContainsKey(key))
			{
				result = !MapData.MarkPrivate.Contains(this.CurrentMap.Marks[key]);
			}
			return result;
		}

		public bool GetMarkOnCell(CellKey key, ref MapData.MarkType type)
		{
			bool result = false;
			if (this.CurrentMap != null && this.CurrentMap.Marks.ContainsKey(key))
			{
				result = true;
				type = this.CurrentMap.Marks[key];
			}
			return result;
		}

		public bool GetCellType(CellKey key, ref EditorMapCellData.CellType type)
		{
			bool result = false;
			if (this.CurrentMap != null && this.CurrentMap.CellsByCellKey.ContainsKey(key))
			{
				result = true;
				type = this.CurrentMap.CellsByCellKey[key].Type;
			}
			return result;
		}

		public Dictionary<CellKey, List<MapData.MarkType>> GetAllMarks()
		{
			Dictionary<CellKey, List<MapData.MarkType>> dictionary = new Dictionary<CellKey, List<MapData.MarkType>>();
			if (this.CurrentMap != null)
			{
				foreach (GuiltManager.GuiltDrop guiltDrop in Core.GuiltManager.GetAllCurrentMapDrops())
				{
					if (guiltDrop != null && guiltDrop.cellKey != null)
					{
						dictionary[guiltDrop.cellKey] = new List<MapData.MarkType>();
						dictionary[guiltDrop.cellKey].Add(MapData.MarkType.Guilt);
					}
					else
					{
						Core.GuiltManager.ResetGuilt(true);
					}
				}
				Dictionary<CellKey, EditorMapCellData.CellType> dictionary2 = new Dictionary<CellKey, EditorMapCellData.CellType>();
				foreach (SecretData secretData in from x in this.CurrentMap.Secrets.Values
				where x.Revealed
				select x)
				{
					foreach (CellData cellData in secretData.Cells.Values)
					{
						dictionary2[cellData.CellKey] = cellData.Type;
					}
				}
				IEnumerable<CellData> enumerable = from p in this.CurrentMap.Cells
				where p.Revealed
				select p;
				foreach (CellData cellData2 in enumerable)
				{
					EditorMapCellData.CellType cellType = cellData2.Type;
					if (dictionary2.ContainsKey(cellData2.CellKey))
					{
						cellType = dictionary2[cellData2.CellKey];
					}
					if (cellType != EditorMapCellData.CellType.Normal)
					{
						if (!dictionary.ContainsKey(cellData2.CellKey))
						{
							dictionary[cellData2.CellKey] = new List<MapData.MarkType>();
						}
						switch (cellType)
						{
						case EditorMapCellData.CellType.PrieDieu:
							dictionary[cellData2.CellKey].Add(MapData.MarkType.PrieDieu);
							break;
						case EditorMapCellData.CellType.Teleport:
							dictionary[cellData2.CellKey].Add(MapData.MarkType.Teleport);
							break;
						case EditorMapCellData.CellType.MeaCulpa:
							dictionary[cellData2.CellKey].Add(MapData.MarkType.MeaCulpa);
							break;
						case EditorMapCellData.CellType.Soledad:
							dictionary[cellData2.CellKey].Add(MapData.MarkType.Soledad);
							break;
						case EditorMapCellData.CellType.Nacimiento:
							dictionary[cellData2.CellKey].Add(MapData.MarkType.Nacimiento);
							break;
						case EditorMapCellData.CellType.Confessor:
							dictionary[cellData2.CellKey].Add(MapData.MarkType.Confessor);
							break;
						case EditorMapCellData.CellType.FuenteFlask:
							dictionary[cellData2.CellKey].Add(MapData.MarkType.FuenteFlask);
							break;
						case EditorMapCellData.CellType.MiriamPortal:
							dictionary[cellData2.CellKey].Add(MapData.MarkType.MiriamPortal);
							break;
						}
					}
				}
				foreach (KeyValuePair<CellKey, MapData.MarkType> keyValuePair in this.CurrentMap.Marks)
				{
					if (!dictionary.ContainsKey(keyValuePair.Key))
					{
						dictionary[keyValuePair.Key] = new List<MapData.MarkType>();
					}
					dictionary[keyValuePair.Key].Add(keyValuePair.Value);
				}
			}
			return dictionary;
		}

		public bool AddMarkOnCell(CellKey key, MapData.MarkType type)
		{
			bool flag = this.CanAddMark(key);
			if (flag)
			{
				this.CurrentMap.Marks[key] = type;
			}
			return flag;
		}

		public bool RemoveMarkOnCell(CellKey key)
		{
			bool result = false;
			if (this.CurrentMap != null)
			{
				result = this.CurrentMap.Marks.Remove(key);
			}
			return result;
		}

		public MapData DEBUG_GetCurrentMap()
		{
			return this.CurrentMap;
		}

		private float GetCellPercentage(bool forNgPlus)
		{
			float result = 0f;
			IEnumerable<CellData> source = from cell in this.CurrentMap.Cells
			where cell.NGPlus == forNgPlus && !cell.IgnoredForMapPercentage
			select cell;
			float num = (float)source.Count<CellData>();
			if (num > 0f)
			{
				result = (float)source.Count((CellData cell) => cell.Revealed) / num;
			}
			return result;
		}

		private void AddProgressToAC21(ZoneKey zone)
		{
			string id = "ZONE_NAME_OF_" + zone.District.ToUpper() + zone.Zone.ToUpper() + "_DISPLAYED";
			if (this.ZonesForAC21.AllowZoneForAc21(zone) && !Core.Events.GetFlag(id))
			{
				Core.Events.SetFlag(id, true, true);
				Core.AchievementsManager.Achievements["AC21"].AddProgress(4.347826f);
			}
		}

		private bool GetZoneKeyFromBundleName(string bundle, ref ZoneKey zoneKey)
		{
			Regex regex = new Regex("^D(?<district>[0-9]{2})Z(?<zone>[0-9]{2})S(?<scene>[0-9]{2})$");
			Match match = regex.Match(bundle);
			if (match.Success)
			{
				string district = "D" + match.Groups["district"].Value;
				string zone = "Z" + match.Groups["zone"].Value;
				string scene = "S" + match.Groups["scene"].Value;
				zoneKey = new ZoneKey(district, zone, scene);
			}
			return match.Success;
		}

		private void LoadAllMaps()
		{
			this.Maps.Clear();
			this.CurrentMap = null;
			this.LastPlayerCell = null;
			this.CurrentSafeScene = new ZoneKey();
			EditorMapData[] array = Resources.LoadAll<EditorMapData>("New Maps/");
			foreach (EditorMapData editorMapData in array)
			{
				MapData mapData = new MapData();
				mapData.Name = editorMapData.name;
				mapData.Cells = new List<CellData>();
				mapData.Secrets = new Dictionary<string, SecretData>();
				mapData.Marks = new Dictionary<CellKey, MapData.MarkType>();
				foreach (KeyValuePair<CellKey, EditorMapCellData> keyValuePair in editorMapData.Cells.CellsDict)
				{
					if (keyValuePair.Value != null)
					{
						CellData cellData = new CellData(keyValuePair.Key, keyValuePair.Value);
						mapData.Cells.Add(cellData);
						if (!mapData.CellsByZone.ContainsKey(keyValuePair.Value.ZoneId))
						{
							mapData.CellsByZone[keyValuePair.Value.ZoneId] = new List<CellData>();
						}
						mapData.CellsByZone[keyValuePair.Value.ZoneId].Add(cellData);
						mapData.CellsByCellKey[keyValuePair.Key] = cellData;
					}
				}
				foreach (KeyValuePair<string, EditorMapCellGrid> keyValuePair2 in editorMapData.Secrets)
				{
					if (!mapData.Secrets.ContainsKey(keyValuePair2.Key))
					{
						mapData.Secrets[keyValuePair2.Key] = new SecretData();
						mapData.Secrets[keyValuePair2.Key].Name = keyValuePair2.Key;
					}
					foreach (KeyValuePair<CellKey, EditorMapCellData> keyValuePair3 in keyValuePair2.Value.CellsDict)
					{
						if (keyValuePair3.Value != null)
						{
							mapData.Secrets[keyValuePair2.Key].Cells[keyValuePair3.Key] = new CellData(keyValuePair3.Key, keyValuePair3.Value);
						}
					}
				}
				this.Maps[editorMapData.name] = mapData;
			}
			if (this.Maps.Count > 0)
			{
				this.CurrentMap = this.Maps.First((KeyValuePair<string, MapData> x) => x.Key.EndsWith("DLC3")).Value;
				this.DefaultMapID = this.CurrentMap.Name;
			}
			Log.Debug("New Map", this.Maps.Count.ToString() + " maps loaded succesfully.", null);
			Log.Debug("******************************************* CELLS", null);
			Log.Debug(" NG+: " + (from cell in this.CurrentMap.Cells
			where cell.NGPlus
			select cell).Count<CellData>(), null);
			Log.Debug(" Normal: " + (from cell in this.CurrentMap.Cells
			where !cell.NGPlus
			select cell).Count<CellData>(), null);
		}

		private List<CellData> GetAllRevealedCells(MapData map)
		{
			List<CellData> list = new List<CellData>();
			if (map != null)
			{
				foreach (CellData cellData in from cell in map.Cells
				where cell.Revealed
				select cell)
				{
					bool flag = false;
					foreach (SecretData secretData in map.Secrets.Values.Where((SecretData x) => x.Revealed))
					{
						if (secretData.Cells.ContainsKey(cellData.CellKey))
						{
							flag = true;
							list.Add(secretData.Cells[cellData.CellKey]);
							break;
						}
					}
					if (!flag)
					{
						list.Add(cellData);
					}
				}
			}
			return list;
		}

		private void OnLocalizationChange()
		{
			if (this.CurrentLanguage != LocalizationManager.CurrentLanguage)
			{
				if (this.CurrentLanguage != string.Empty)
				{
					Log.Debug("MapManager", "Language change, localize items to: " + LocalizationManager.CurrentLanguage, null);
				}
				this.CurrentLanguage = LocalizationManager.CurrentLanguage;
				LanguageSource mainLanguageSource = LocalizationManager.GetMainLanguageSource();
				int languageIndexFromCode = mainLanguageSource.GetLanguageIndexFromCode(LocalizationManager.CurrentLanguageCode, true);
				foreach (string text in new List<string>(this.DistrictLocalization.Keys))
				{
					string text2 = "Map/" + text;
					TermData termData = mainLanguageSource.GetTermData(text2, false);
					if (termData == null)
					{
						Debug.LogWarning("Term " + text2 + " not found in Maps Localization");
					}
					else
					{
						this.DistrictLocalization[text] = termData.Languages[languageIndexFromCode];
					}
				}
				foreach (string text3 in new List<string>(this.ZonesLocalization.Keys))
				{
					string text4 = "Map/" + text3;
					TermData termData2 = mainLanguageSource.GetTermData(text4, false);
					if (termData2 == null)
					{
						Debug.LogWarning("Term " + text4 + " not found in Maps Localization");
					}
					else
					{
						this.ZonesLocalization[text3] = termData2.Languages[languageIndexFromCode];
					}
				}
			}
		}

		private void CreateLocaliztionDicts()
		{
			this.DistrictLocalization.Clear();
			this.ZonesLocalization.Clear();
			Regex regex = new Regex("^D(?<district>[0-9]{2})Z(?<zone>[0-9]{2})S(?<scene>[0-9]{2})$");
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(scenePathByBuildIndex);
				string input = fileNameWithoutExtension.Split(new char[]
				{
					'_'
				})[0];
				Match match = regex.Match(input);
				if (match.Success)
				{
					string text = "D" + match.Groups["district"].Value;
					string zone = "Z" + match.Groups["zone"].Value;
					string scene = "S" + match.Groups["scene"].Value;
					ZoneKey zoneKey = new ZoneKey(text, zone, scene);
					if (!GameConstants.DistrictsWithoutName.Contains(text))
					{
						if (!this.DistrictLocalization.ContainsKey(text))
						{
							this.DistrictLocalization[text] = string.Empty;
						}
						string localizationKey = zoneKey.GetLocalizationKey();
						if (!this.ZonesLocalization.ContainsKey(localizationKey))
						{
							this.ZonesLocalization[localizationKey] = string.Empty;
						}
					}
				}
			}
		}

		public int GetOrder()
		{
			return -10;
		}

		public string GetPersistenID()
		{
			return "ID_NEW_MAPS";
		}

		public void ResetPersistence()
		{
			this.LoadAllMaps();
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			NewMapManager.NewMapPersistenceData newMapPersistenceData = new NewMapManager.NewMapPersistenceData();
			newMapPersistenceData.currentMapId = string.Empty;
			if (this.CurrentMap != null)
			{
				newMapPersistenceData.currentMapId = this.CurrentMap.Name;
			}
			foreach (KeyValuePair<string, MapData> keyValuePair in this.Maps)
			{
				if (!newMapPersistenceData.Maps.ContainsKey(keyValuePair.Key))
				{
					newMapPersistenceData.Maps[keyValuePair.Key] = new NewMapManager.NewMapPersistenceDataItem();
				}
				foreach (CellData cellData in keyValuePair.Value.Cells)
				{
					if (cellData.Revealed)
					{
						newMapPersistenceData.Maps[keyValuePair.Key].RevealedCells.Add(cellData.CellKey);
					}
				}
				foreach (KeyValuePair<string, SecretData> keyValuePair2 in keyValuePair.Value.Secrets)
				{
					if (keyValuePair2.Value.Revealed)
					{
						newMapPersistenceData.Maps[keyValuePair.Key].Secrets.Add(keyValuePair2.Key);
					}
				}
				newMapPersistenceData.Maps[keyValuePair.Key].Marks = new Dictionary<CellKey, MapData.MarkType>(keyValuePair.Value.Marks);
			}
			return newMapPersistenceData;
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			foreach (KeyValuePair<string, MapData> keyValuePair in this.Maps)
			{
				foreach (CellData cellData in keyValuePair.Value.Cells)
				{
					cellData.Revealed = false;
				}
				foreach (SecretData secretData in keyValuePair.Value.Secrets.Values)
				{
					secretData.Revealed = false;
				}
				keyValuePair.Value.Marks.Clear();
			}
			NewMapManager.NewMapPersistenceData newMapPersistenceData = (NewMapManager.NewMapPersistenceData)data;
			this.PassRevealedCells(newMapPersistenceData, string.Empty, string.Empty);
			this.PassRevealedSecrets(newMapPersistenceData, string.Empty, string.Empty);
			this.PassMarks(newMapPersistenceData, string.Empty, string.Empty);
			this.CurrentMap = null;
			if (newMapPersistenceData.currentMapId != string.Empty)
			{
				this.CurrentMap = this.Maps.First((KeyValuePair<string, MapData> x) => x.Key.EndsWith("DLC3")).Value;
				if (newMapPersistenceData.currentMapId.EndsWith("DLC3"))
				{
					return;
				}
				MapData mapData = null;
				if (this.Maps.TryGetValue(newMapPersistenceData.currentMapId, out mapData))
				{
					this.PassRevealedCells(newMapPersistenceData, mapData.Name, this.CurrentMap.Name);
					this.PassRevealedSecrets(newMapPersistenceData, mapData.Name, this.CurrentMap.Name);
					this.PassMarks(newMapPersistenceData, mapData.Name, this.CurrentMap.Name);
				}
			}
		}

		private void PassRevealedCells(NewMapManager.NewMapPersistenceData dataSource, string sorceMapKey = "", string destinationMapKey = "")
		{
			foreach (KeyValuePair<string, NewMapManager.NewMapPersistenceDataItem> keyValuePair in dataSource.Maps)
			{
				if (string.IsNullOrEmpty(sorceMapKey) || !(keyValuePair.Key != sorceMapKey))
				{
					if (this.Maps.ContainsKey(keyValuePair.Key))
					{
						MapData mapData = this.Maps[keyValuePair.Key];
						if (!string.IsNullOrEmpty(destinationMapKey))
						{
							mapData = this.Maps[destinationMapKey];
						}
						using (List<CellKey>.Enumerator enumerator2 = keyValuePair.Value.RevealedCells.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								CellKey storedCellKey = enumerator2.Current;
								CellData cellData = mapData.Cells.FirstOrDefault((CellData x) => x.CellKey.Equals(storedCellKey));
								if (cellData != null)
								{
									cellData.Revealed = true;
								}
								else
								{
									Debug.LogError("*** MAP Persistence: Cell Key " + storedCellKey.ToString() + " not found in map " + keyValuePair.Key);
								}
							}
						}
					}
					else
					{
						Debug.LogError("*** MAP Persistence: Map " + keyValuePair.Key + " not found");
					}
				}
			}
		}

		private void PassRevealedSecrets(NewMapManager.NewMapPersistenceData dataSource, string sorceMapKey = "", string destinationMapKey = "")
		{
			foreach (KeyValuePair<string, NewMapManager.NewMapPersistenceDataItem> keyValuePair in dataSource.Maps)
			{
				if (string.IsNullOrEmpty(sorceMapKey) || !(keyValuePair.Key != sorceMapKey))
				{
					if (this.Maps.ContainsKey(keyValuePair.Key))
					{
						MapData mapData = this.Maps[keyValuePair.Key];
						if (!string.IsNullOrEmpty(destinationMapKey))
						{
							mapData = this.Maps[destinationMapKey];
						}
						foreach (string text in keyValuePair.Value.Secrets)
						{
							if (mapData.Secrets.ContainsKey(text))
							{
								mapData.Secrets[text].Revealed = true;
							}
							else
							{
								Debug.LogError("*** MAP Persistence: Secret " + text + " not found in map " + keyValuePair.Key);
							}
						}
					}
					else
					{
						Debug.LogError("*** MAP Persistence: Map " + keyValuePair.Key + " not found");
					}
				}
			}
		}

		private void PassMarks(NewMapManager.NewMapPersistenceData dataSource, string sorceMapKey = "", string destinationMapKey = "")
		{
			foreach (KeyValuePair<string, NewMapManager.NewMapPersistenceDataItem> keyValuePair in dataSource.Maps)
			{
				if (string.IsNullOrEmpty(sorceMapKey) || !(keyValuePair.Key != sorceMapKey))
				{
					if (this.Maps.ContainsKey(keyValuePair.Key))
					{
						MapData mapData = this.Maps[keyValuePair.Key];
						if (!string.IsNullOrEmpty(destinationMapKey))
						{
							mapData = this.Maps[destinationMapKey];
						}
						mapData.Marks = new Dictionary<CellKey, MapData.MarkType>(keyValuePair.Value.Marks);
					}
					else
					{
						Debug.LogError("*** MAP Persistence: Map " + keyValuePair.Key + " not found");
					}
				}
			}
		}

		public override void OnGUI()
		{
			base.DebugResetLine();
			base.DebugDrawTextLine("NewMapManager -------------------------------------", 10, 1500);
			if (this.CurrentMap == null)
			{
				base.DebugDrawTextLine("  NO MAP", 10, 1500);
				return;
			}
			base.DebugDrawTextLine("--Current Map: " + this.CurrentMap.Name, 10, 1500);
			CellKey playerCell = this.GetPlayerCell();
			if (this.CurrentScene != null)
			{
				base.DebugDrawTextLine("--Current SCENE: " + this.CurrentScene.GetKey(), 10, 1500);
			}
			else
			{
				base.DebugDrawTextLine("--Current SCENE: NONE", 10, 1500);
			}
			if (playerCell != null)
			{
				base.DebugDrawTextLine("--Current CELL  X:" + playerCell.X.ToString() + "   Y:" + playerCell.Y.ToString(), 10, 1500);
				base.DebugDrawTextLine("    TYPE:" + this.LastPlayerCell.Type.ToString(), 10, 1500);
			}
			else
			{
				base.DebugDrawTextLine("--Current CELL: NONE", 10, 1500);
			}
			base.DebugDrawTextLine("--Number of revealed: " + this.GetAllRevealedCells().Count.ToString(), 10, 1500);
			base.DebugDrawTextLine("--Secrets", 10, 1500);
			foreach (KeyValuePair<string, SecretData> keyValuePair in this.CurrentMap.Secrets)
			{
				base.DebugDrawTextLine("    " + keyValuePair.Value.Name + ": " + keyValuePair.Value.Revealed.ToString(), 10, 1500);
			}
		}

		private ZoneKey LastScene = new ZoneKey();

		private Dictionary<string, string> DistrictLocalization = new Dictionary<string, string>();

		private Dictionary<string, string> ZonesLocalization = new Dictionary<string, string>();

		private const string REG_EXP = "^D(?<district>[0-9]{2})Z(?<zone>[0-9]{2})S(?<scene>[0-9]{2})$";

		private const int TOTAL_NUMBER_OF_ZONES_FOR_AC21 = 23;

		private const string MAP_DIRECTORY = "New Maps/";

		private const string AC21_CONFIG_PATH = "New Maps/ZonesAC21";

		private ZonesAC21 ZonesForAC21;

		private string CurrentLanguage = string.Empty;

		private Dictionary<string, MapData> Maps = new Dictionary<string, MapData>();

		private MapData CurrentMap;

		private CellData LastPlayerCell;

		private const string PERSITENT_ID = "ID_NEW_MAPS";

		[Serializable]
		public class NewMapPersistenceDataItem
		{
			public NewMapPersistenceDataItem()
			{
				this.RevealedCells = new List<CellKey>();
				this.Secrets = new List<string>();
				this.Marks = new Dictionary<CellKey, MapData.MarkType>();
			}

			public List<CellKey> RevealedCells;

			public List<string> Secrets;

			public Dictionary<CellKey, MapData.MarkType> Marks;
		}

		[Serializable]
		public class NewMapPersistenceData : PersistentManager.PersistentData
		{
			public NewMapPersistenceData() : base("ID_NEW_MAPS")
			{
			}

			public string currentMapId;

			public Dictionary<string, NewMapManager.NewMapPersistenceDataItem> Maps = new Dictionary<string, NewMapManager.NewMapPersistenceDataItem>();
		}
	}
}
