using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Framework.FrameworkCore;
using Framework.Inventory;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Damage;
using I2.Loc;
using Tools.Level.Actionables;
using UnityEngine;

namespace Framework.Managers
{
	public class InventoryManager : GameSystem, PersistentInterface
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event InventoryManager.ItemEvent OnItemEquiped;

		public BaseInventoryObject LastAddedObject { get; private set; }

		public InventoryManager.ItemType LastAddedObjectType
		{
			get
			{
				this.AnyLastUsedObjectUntilLastCalled = false;
				return this._LastAddedObjectType;
			}
			private set
			{
				this._LastAddedObjectType = value;
				this.AnyLastUsedObjectUntilLastCalled = true;
			}
		}

		public bool AnyLastUsedObjectUntilLastCalled { get; private set; }

		public TearsObject TearsGenericObject { get; private set; }

		public override void Start()
		{
			this.language = InventoryManager.GetLanguageSource();
			LocalizationManager.OnLocalizeEvent += this.OnLocalizationChange;
			PenitentDamageArea.OnHitGlobal = (PenitentDamageArea.PlayerHitEvent)Delegate.Combine(PenitentDamageArea.OnHitGlobal, new PenitentDamageArea.PlayerHitEvent(this.OnPenitentHit));
			this.InitializeObjects();
		}

		public override void Update()
		{
			base.Update();
			if (Core.Logic != null && Core.Logic.Penitent != null)
			{
				if (this.PlayerHealth != Core.Logic.Penitent.Stats.Life.Current)
				{
					if (this.PlayerHealth != -1f)
					{
						this.PlayerHealth = Core.Logic.Penitent.Stats.Life.Current;
						this.SendPenitentHealthChanged<RosaryBead>(this.PlayerHealth, this.wearBeads);
						this.SendPenitentHealthChanged<Relic>(this.PlayerHealth, this.wearRellics);
						this.SendPenitentHealthChanged<Prayer>(this.PlayerHealth, this.wearPrayers);
						this.SendPenitentHealthChanged<Sword>(this.PlayerHealth, this.wearSwords);
					}
					else
					{
						this.PlayerHealth = Core.Logic.Penitent.Stats.Life.Current;
					}
				}
				if (this.NumOfFlasks != Core.Logic.Penitent.Stats.Flask.Current)
				{
					if (this.NumOfFlasks != -1f)
					{
						this.NumOfFlasks = Core.Logic.Penitent.Stats.Flask.Current;
						this.SendNumberOfCurrentFlasksChanged<RosaryBead>(this.NumOfFlasks, this.wearBeads);
						this.SendNumberOfCurrentFlasksChanged<Relic>(this.NumOfFlasks, this.wearRellics);
						this.SendNumberOfCurrentFlasksChanged<Prayer>(this.NumOfFlasks, this.wearPrayers);
						this.SendNumberOfCurrentFlasksChanged<Sword>(this.NumOfFlasks, this.wearSwords);
					}
					else
					{
						this.NumOfFlasks = Core.Logic.Penitent.Stats.Flask.Current;
					}
				}
			}
		}

		private void InitializeObjects()
		{
			if (this.mainObject == null)
			{
				this.mainObject = new GameObject("Inventory Objects");
				UnityEngine.Object.DontDestroyOnLoad(this.mainObject);
			}
			this.allRellics = new Dictionary<string, Relic>();
			this.ownRellics = new List<Relic>();
			for (int i = 0; i < this.wearRellics.Length; i++)
			{
				this.wearRellics[i] = null;
			}
			this.allBeads = new Dictionary<string, RosaryBead>();
			this.ownBeads = new List<RosaryBead>();
			for (int j = 0; j < this.wearRellics.Length; j++)
			{
				this.wearBeads[j] = null;
			}
			this.allQuestItems = new Dictionary<string, QuestItem>();
			this.ownQuestItems = new List<QuestItem>();
			this.allCollectibleItems = new Dictionary<string, Framework.Inventory.CollectibleItem>();
			this.ownCollectibleItems = new List<Framework.Inventory.CollectibleItem>();
			this.allPrayers = new Dictionary<string, Prayer>();
			this.ownPrayers = new List<Prayer>();
			for (int k = 0; k < this.wearPrayers.Length; k++)
			{
				this.wearPrayers[k] = null;
			}
			this.allSwords = new Dictionary<string, Sword>();
			this.ownSwords = new List<Sword>();
			for (int l = 0; l < this.wearSwords.Length; l++)
			{
				this.wearSwords[l] = null;
			}
			for (int m = 0; m < this.ownBossKeys.Length; m++)
			{
				this.ownBossKeys[m] = false;
			}
			this.LoadBaseInventoryObjects<Relic>(this.allRellics, this.ownRellics);
			this.LoadBaseInventoryObjects<RosaryBead>(this.allBeads, this.ownBeads);
			this.LoadBaseInventoryObjects<QuestItem>(this.allQuestItems, this.ownQuestItems);
			this.LoadBaseInventoryObjects<Framework.Inventory.CollectibleItem>(this.allCollectibleItems, this.ownCollectibleItems);
			this.LoadBaseInventoryObjects<Prayer>(this.allPrayers, this.ownPrayers);
			this.LoadBaseInventoryObjects<Sword>(this.allSwords, this.ownSwords);
			int num = this.allBeads.Count + this.allRellics.Count + this.allQuestItems.Count + this.allPrayers.Count + this.allCollectibleItems.Count + this.allSwords.Count;
			Log.Debug("Inventory", num + " items loaded succesfully.", null);
			Log.Debug("Inventory", this.allBeads.Count + " beads.", null);
			Log.Debug("Inventory", this.allRellics.Count + " relics.", null);
			Log.Debug("Inventory", this.allQuestItems.Count + " quest items.", null);
			Log.Debug("Inventory", this.allPrayers.Count + " prayers.", null);
			Log.Debug("Inventory", this.allCollectibleItems.Count + " collectibles.", null);
			Log.Debug("Inventory", this.allSwords.Count + " swords.", null);
			this.TearsGenericObject = Resources.Load<TearsObject>("Inventory/TearObject");
			InventoryManager.currentLanguage = string.Empty;
			this.OnLocalizationChange();
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
		}

		public override void Dispose()
		{
			LocalizationManager.OnLocalizeEvent -= this.OnLocalizationChange;
			PenitentDamageArea.OnHitGlobal = (PenitentDamageArea.PlayerHitEvent)Delegate.Remove(PenitentDamageArea.OnHitGlobal, new PenitentDamageArea.PlayerHitEvent(this.OnPenitentHit));
		}

		private void LoadBaseInventoryObjects<T>(Dictionary<string, T> allDict, List<T> ownList) where T : BaseInventoryObject
		{
			string name = typeof(T).Name;
			Transform transform = this.mainObject.transform.Find(name);
			if (transform == null)
			{
				GameObject gameObject = new GameObject(name);
				gameObject.transform.SetParent(this.mainObject.transform);
				transform = gameObject.transform;
			}
			else
			{
				IEnumerator enumerator = transform.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Transform transform2 = (Transform)obj;
						UnityEngine.Object.Destroy(transform2.gameObject);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			UnityEngine.Object[] array = Resources.LoadAll("Inventory/" + name);
			foreach (UnityEngine.Object @object in array)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(@object, transform) as GameObject;
				if (gameObject2 == null)
				{
					Log.Error("Inventory", "Error instanciating prefab " + @object.name, null);
				}
				else
				{
					T component = gameObject2.GetComponent<T>();
					if (component == null)
					{
						Log.Error("Inventory", string.Concat(new string[]
						{
							"Error on prefab ",
							gameObject2.name,
							" hasn't ",
							name,
							" component, skip"
						}), null);
						UnityEngine.Object.Destroy(gameObject2);
					}
					else
					{
						gameObject2.name = component.id;
						allDict[component.id] = component;
						if (component.carryonstart)
						{
							this.AddObject<T>(component, ownList);
						}
					}
				}
			}
		}

		public void OnDamageInflicted(Hit hit)
		{
			this.SendHitEvent<RosaryBead>(hit, this.wearBeads);
			this.SendHitEvent<Relic>(hit, this.wearRellics);
			this.SendHitEvent<Prayer>(hit, this.wearPrayers);
			this.SendHitEvent<Sword>(hit, this.wearSwords);
		}

		public void OnEnemyKilled(Enemy e)
		{
			this.SendKillEnemyEvent<RosaryBead>(e, this.wearBeads);
			this.SendKillEnemyEvent<Relic>(e, this.wearRellics);
			this.SendKillEnemyEvent<Prayer>(e, this.wearPrayers);
			this.SendKillEnemyEvent<Sword>(e, this.wearSwords);
		}

		public void OnPenitentHit(Penitent damaged, Hit hit)
		{
			this.SendReceiveHitEvent<RosaryBead>(hit, this.wearBeads);
			this.SendReceiveHitEvent<Relic>(hit, this.wearRellics);
			this.SendReceiveHitEvent<Prayer>(hit, this.wearPrayers);
			this.SendReceiveHitEvent<Sword>(hit, this.wearSwords);
		}

		public void OnBreakBreakable(BreakableObject breakable)
		{
			this.SendBreakBreakableEvent<RosaryBead>(breakable, this.wearBeads);
			this.SendBreakBreakableEvent<Relic>(breakable, this.wearRellics);
			this.SendBreakBreakableEvent<Prayer>(breakable, this.wearPrayers);
			this.SendBreakBreakableEvent<Sword>(breakable, this.wearSwords);
		}

		public void OnPenitentDead()
		{
			this.SendOnPenitentDead<RosaryBead>(this.wearBeads);
			this.SendOnPenitentDead<Prayer>(this.wearPrayers);
			this.SendOnPenitentDead<Prayer>(this.wearPrayers);
			this.SendOnPenitentDead<Sword>(this.wearSwords);
		}

		private void SendOnPenitentDead<T>(T[] ownObject) where T : BaseInventoryObject
		{
			T[] array = (T[])ownObject.Clone();
			foreach (T t in array)
			{
				if (t)
				{
					t.PenitentDead();
				}
			}
		}

		private void SendBreakBreakableEvent<T>(BreakableObject breakable, T[] ownObject) where T : BaseInventoryObject
		{
			foreach (T t in ownObject)
			{
				if (t)
				{
					t.BreakableBreak(breakable);
				}
			}
		}

		private void SendHitEvent<T>(Hit hit, T[] ownObject) where T : BaseInventoryObject
		{
			foreach (T t in ownObject)
			{
				if (t)
				{
					t.HitEnemy(hit);
				}
			}
		}

		private void SendKillEnemyEvent<T>(Enemy e, T[] ownObject) where T : BaseInventoryObject
		{
			foreach (T t in ownObject)
			{
				if (t)
				{
					t.KillEnemy(e);
				}
			}
		}

		private void SendReceiveHitEvent<T>(Hit hit, T[] ownObject) where T : BaseInventoryObject
		{
			foreach (T t in ownObject)
			{
				if (t)
				{
					t.HitReceived(hit);
				}
			}
		}

		private void SendPenitentHealthChanged<T>(float life, T[] ownObject) where T : BaseInventoryObject
		{
			foreach (T t in ownObject)
			{
				if (t)
				{
					t.PenitentHealthChanged(life);
				}
			}
		}

		private void SendNumberOfCurrentFlasksChanged<T>(float newNumberOfFlasks, T[] ownObject) where T : BaseInventoryObject
		{
			foreach (T t in ownObject)
			{
				if (t)
				{
					t.NumberOfCurrentFlasksChanged(newNumberOfFlasks);
				}
			}
		}

		public BaseInventoryObject AddBaseObjectOrTears(BaseInventoryObject inbObject)
		{
			BaseInventoryObject result = inbObject;
			if (!this.AddBaseObject(inbObject))
			{
				string internalId = inbObject.GetInternalId();
				if (!this.ObjectsConvertedToTears.Contains(internalId))
				{
					this.ObjectsConvertedToTears.Add(internalId);
				}
				result = this.TearsGenericObject;
				Core.Logic.Penitent.Stats.Purge.Current += this.TearsGenericObject.TearsForDuplicatedObject;
			}
			return result;
		}

		public BaseInventoryObject GetBaseObjectOrTears(string idObj, InventoryManager.ItemType itemType)
		{
			BaseInventoryObject baseInventoryObject = this.GetBaseObject(idObj, itemType);
			string internalId = baseInventoryObject.GetInternalId();
			if (this.ObjectsConvertedToTears.Contains(internalId))
			{
				baseInventoryObject = this.TearsGenericObject;
			}
			return baseInventoryObject;
		}

		public BaseInventoryObject GetBaseObject(string idObj, InventoryManager.ItemType itemType)
		{
			BaseInventoryObject result = null;
			switch (itemType)
			{
			case InventoryManager.ItemType.Relic:
				result = Core.InventoryManager.GetRelic(idObj);
				break;
			case InventoryManager.ItemType.Prayer:
				result = Core.InventoryManager.GetPrayer(idObj);
				break;
			case InventoryManager.ItemType.Bead:
				result = Core.InventoryManager.GetRosaryBead(idObj);
				break;
			case InventoryManager.ItemType.Quest:
				result = Core.InventoryManager.GetQuestItem(idObj);
				break;
			case InventoryManager.ItemType.Collectible:
				result = Core.InventoryManager.GetCollectibleItem(idObj);
				break;
			case InventoryManager.ItemType.Sword:
				result = Core.InventoryManager.GetSword(idObj);
				break;
			}
			return result;
		}

		public bool AddBaseObject(BaseInventoryObject inbObject)
		{
			bool result = false;
			if (inbObject.GetType() == typeof(Relic))
			{
				result = this.AddRelic((Relic)inbObject);
			}
			if (inbObject.GetType() == typeof(RosaryBead))
			{
				result = this.AddRosaryBead((RosaryBead)inbObject);
			}
			if (inbObject.GetType() == typeof(QuestItem))
			{
				result = this.AddQuestItem((QuestItem)inbObject);
			}
			if (inbObject.GetType() == typeof(Prayer))
			{
				result = this.AddPrayer((Prayer)inbObject);
			}
			if (inbObject.GetType() == typeof(Framework.Inventory.CollectibleItem))
			{
				result = this.AddCollectibleItem(inbObject.id);
			}
			if (inbObject.GetType() == typeof(Sword))
			{
				result = this.AddSword(inbObject.id);
			}
			return result;
		}

		public void TestAddAllObjects()
		{
			this.allBeads.Values.ToList<RosaryBead>().ForEach(delegate(RosaryBead item)
			{
				this.AddBaseObject(item);
			});
			this.allPrayers.Values.ToList<Prayer>().ForEach(delegate(Prayer item)
			{
				this.AddBaseObject(item);
			});
			this.allQuestItems.Values.ToList<QuestItem>().ForEach(delegate(QuestItem item)
			{
				this.AddBaseObject(item);
			});
			this.allRellics.Values.ToList<Relic>().ForEach(delegate(Relic item)
			{
				this.AddBaseObject(item);
			});
			this.allCollectibleItems.Values.ToList<Framework.Inventory.CollectibleItem>().ForEach(delegate(Framework.Inventory.CollectibleItem item)
			{
				this.AddBaseObject(item);
			});
			this.allSwords.Values.ToList<Sword>().ForEach(delegate(Sword item)
			{
				this.AddBaseObject(item);
			});
		}

		public void AddAll(InventoryManager.ItemType itemType)
		{
			switch (itemType)
			{
			case InventoryManager.ItemType.Relic:
				this.allRellics.Values.ToList<Relic>().ForEach(delegate(Relic item)
				{
					this.AddBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Prayer:
				this.allPrayers.Values.ToList<Prayer>().ForEach(delegate(Prayer item)
				{
					this.AddBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Bead:
				this.allBeads.Values.ToList<RosaryBead>().ForEach(delegate(RosaryBead item)
				{
					this.AddBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Quest:
				this.allQuestItems.Values.ToList<QuestItem>().ForEach(delegate(QuestItem item)
				{
					this.AddBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Collectible:
				this.allCollectibleItems.Values.ToList<Framework.Inventory.CollectibleItem>().ForEach(delegate(Framework.Inventory.CollectibleItem item)
				{
					this.AddBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Sword:
				this.allSwords.Values.ToList<Sword>().ForEach(delegate(Sword item)
				{
					this.AddBaseObject(item);
				});
				break;
			default:
				UnityEngine.Debug.LogError("Unknown AddAll param: " + itemType);
				break;
			}
		}

		public void RemoveAll(InventoryManager.ItemType itemType)
		{
			switch (itemType)
			{
			case InventoryManager.ItemType.Relic:
				this.allRellics.Values.ToList<Relic>().ForEach(delegate(Relic item)
				{
					this.RemoveBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Prayer:
				this.allPrayers.Values.ToList<Prayer>().ForEach(delegate(Prayer item)
				{
					this.RemoveBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Bead:
				this.allBeads.Values.ToList<RosaryBead>().ForEach(delegate(RosaryBead item)
				{
					this.RemoveBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Quest:
				this.allQuestItems.Values.ToList<QuestItem>().ForEach(delegate(QuestItem item)
				{
					this.RemoveBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Collectible:
				this.allCollectibleItems.Values.ToList<Framework.Inventory.CollectibleItem>().ForEach(delegate(Framework.Inventory.CollectibleItem item)
				{
					this.RemoveBaseObject(item);
				});
				break;
			case InventoryManager.ItemType.Sword:
				this.allSwords.Values.ToList<Sword>().ForEach(delegate(Sword item)
				{
					this.RemoveBaseObject(item);
				});
				break;
			default:
				UnityEngine.Debug.LogError("Unknown AddAll param: " + itemType);
				break;
			}
		}

		public void PrepareForNewGamePlus()
		{
			this.RemoveObjectForNewGamePlus<Relic>(this.ownRellics);
			this.RemoveObjectForNewGamePlus<RosaryBead>(this.ownBeads);
			this.RemoveObjectForNewGamePlus<QuestItem>(this.ownQuestItems);
			this.RemoveObjectForNewGamePlus<Prayer>(this.ownPrayers);
			this.RemoveObjectForNewGamePlus<Framework.Inventory.CollectibleItem>(this.ownCollectibleItems);
			this.RemoveObjectForNewGamePlus<Sword>(this.ownSwords);
			for (int i = 0; i < 4; i++)
			{
				Core.InventoryManager.RemoveBossKey(i);
			}
			this.RemoveEquipableObjects();
		}

		public void RemoveEquipableObjects()
		{
			this.UnequipAll(this.wearBeads);
			this.UnequipAll(this.wearPrayers);
			this.UnequipAll(this.wearRellics);
			this.UnequipAll(this.wearSwords);
		}

		public void RemoveBeads()
		{
			this.UnequipAll(this.wearBeads);
		}

		public void RemovePrayers()
		{
			this.UnequipAll(this.wearPrayers);
		}

		public bool RemoveBaseObject(BaseInventoryObject inbObject)
		{
			bool result = false;
			if (inbObject.GetType() == typeof(Relic))
			{
				result = this.RemoveRelic((Relic)inbObject);
			}
			if (inbObject.GetType() == typeof(RosaryBead))
			{
				result = this.RemoveRosaryBead((RosaryBead)inbObject);
			}
			if (inbObject.GetType() == typeof(QuestItem))
			{
				result = this.RemoveQuestItem((QuestItem)inbObject);
			}
			if (inbObject.GetType() == typeof(Prayer))
			{
				result = this.RemovePrayer((Prayer)inbObject);
			}
			if (inbObject.GetType() == typeof(Framework.Inventory.CollectibleItem))
			{
				result = this.RemoveCollectibleItem(inbObject.id);
			}
			if (inbObject.GetType() == typeof(Sword))
			{
				result = this.RemoveSword((Sword)inbObject);
			}
			return result;
		}

		public bool IsBaseObjectEquipped(BaseInventoryObject inbObject)
		{
			bool result = false;
			if (inbObject.GetType() == typeof(Relic))
			{
				result = this.IsRelicEquipped((Relic)inbObject);
			}
			if (inbObject.GetType() == typeof(RosaryBead))
			{
				result = this.IsRosaryBeadEquipped((RosaryBead)inbObject);
			}
			if (inbObject.GetType() == typeof(QuestItem))
			{
				result = this.IsQuestItemOwned((QuestItem)inbObject);
			}
			if (inbObject.GetType() == typeof(Prayer))
			{
				result = this.IsPrayerEquipped((Prayer)inbObject);
			}
			if (inbObject.GetType() == typeof(Framework.Inventory.CollectibleItem))
			{
				result = this.IsCollectibleItemOwned(inbObject.id);
			}
			if (inbObject.GetType() == typeof(Sword))
			{
				result = this.IsSwordOwned(inbObject.id);
			}
			return result;
		}

		public bool EquipBaseObject(BaseInventoryObject inbObject, int slot)
		{
			bool result = false;
			if (inbObject.GetType() == typeof(Relic))
			{
				result = this.SetRelicInSlot(slot, (Relic)inbObject);
			}
			if (inbObject.GetType() == typeof(RosaryBead))
			{
				result = this.SetRosaryBeadInSlot(slot, (RosaryBead)inbObject);
			}
			if (inbObject.GetType() == typeof(Prayer))
			{
				result = this.SetPrayerInSlot(slot, (Prayer)inbObject);
			}
			if (inbObject.GetType() == typeof(Sword))
			{
				result = this.SetSwordInSlot(slot, inbObject.id);
			}
			return result;
		}

		public int GetBaseObjectEquippedSlot(BaseInventoryObject inbObject)
		{
			int result = -1;
			if (inbObject.GetType() == typeof(Relic))
			{
				result = this.GetRelicSlot((Relic)inbObject);
			}
			if (inbObject.GetType() == typeof(RosaryBead))
			{
				result = this.GetRosaryBeadSlot((RosaryBead)inbObject);
			}
			if (inbObject.GetType() == typeof(Prayer))
			{
				result = this.GetPrayerInSlot((Prayer)inbObject);
			}
			if (inbObject.GetType() == typeof(Sword))
			{
				result = this.GetSwordInSlot((Sword)inbObject);
			}
			return result;
		}

		public Relic GetRelic(string idRelic)
		{
			Relic result = null;
			this.allRellics.TryGetValue(idRelic.ToUpper(), out result);
			return result;
		}

		public ReadOnlyCollection<Relic> GetAllRelics()
		{
			return this.GetDictValueReadOnly<Relic>(this.allRellics);
		}

		public ReadOnlyCollection<Relic> GetRelicsOwned()
		{
			return this.ownRellics.AsReadOnly();
		}

		public bool AddRelic(string idRelic)
		{
			return this.AddRelic(this.GetRelic(idRelic.ToUpper()));
		}

		public bool AddRelic(Relic relic)
		{
			bool flag = relic && !this.ownRellics.Contains(relic);
			if (flag)
			{
				this.AddObject<Relic>(relic, this.ownRellics);
			}
			return flag;
		}

		public bool RemoveRelic(string idRelic)
		{
			return this.RemoveRelic(this.GetRelic(idRelic.ToUpper()));
		}

		public bool RemoveRelic(Relic relic)
		{
			bool flag = relic && this.ownRellics.Contains(relic);
			if (flag)
			{
				this.Unequip(relic, this.wearRellics);
				this.RemoveObject<Relic>(relic, this.ownRellics);
			}
			return flag;
		}

		public bool IsRelicOwned(string idRelic)
		{
			return this.IsRelicOwned(this.GetRelic(idRelic.ToUpper()));
		}

		public bool IsRelicOwned(Relic relic)
		{
			bool result = false;
			if (relic != null)
			{
				result = this.ownRellics.Contains(relic);
			}
			return result;
		}

		public bool IsRelicEquipped(string idRelic)
		{
			return this.IsRelicEquipped(this.GetRelic(idRelic.ToUpper()));
		}

		public bool IsRelicEquipped(Relic relic)
		{
			bool flag = false;
			if (relic != null)
			{
				for (int i = 0; i < 3; i++)
				{
					flag = (this.wearRellics[i] == relic);
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		public bool IsAnyRelicEquipped()
		{
			bool result = false;
			for (int i = 0; i < 3; i++)
			{
				if (this.wearRellics[i] && this.wearRellics[i].IsEquiped)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public int GetRelicSlot(Relic relic)
		{
			int result = -1;
			if (relic != null)
			{
				for (int i = 0; i < 3; i++)
				{
					if (this.wearRellics[i] == relic)
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		public Relic GetRelicInSlot(int slot)
		{
			if (slot >= 0 && slot < 3)
			{
				return this.wearRellics[slot];
			}
			return null;
		}

		public bool SetRelicInSlot(int slot, string idRelic)
		{
			return this.SetRelicInSlot(slot, this.GetRelic(idRelic.ToUpper()));
		}

		public bool SetRelicInSlot(int slot, Relic relic)
		{
			bool flag = slot >= 0 && slot < 3;
			if (flag)
			{
				if (this.wearRellics[slot] != null)
				{
					this.Unequip(this.wearRellics[slot], this.wearRellics);
				}
				if (relic)
				{
					this.Equip<Relic>(relic, slot, this.wearRellics);
				}
			}
			return flag;
		}

		public RosaryBead GetRosaryBead(string idRosaryBead)
		{
			RosaryBead result = null;
			this.allBeads.TryGetValue(idRosaryBead.ToUpper(), out result);
			return result;
		}

		public ReadOnlyCollection<RosaryBead> GetAllRosaryBeads()
		{
			return this.GetDictValueReadOnly<RosaryBead>(this.allBeads);
		}

		public ReadOnlyCollection<RosaryBead> GetRosaryBeadOwned()
		{
			return this.ownBeads.AsReadOnly();
		}

		public bool AddRosaryBead(string idRosaryBead)
		{
			return this.AddRosaryBead(this.GetRosaryBead(idRosaryBead.ToUpper()));
		}

		public bool AddRosaryBead(RosaryBead rosaryBead)
		{
			bool flag = rosaryBead && !this.ownBeads.Contains(rosaryBead);
			if (flag)
			{
				this.AddObject<RosaryBead>(rosaryBead, this.ownBeads);
			}
			return flag;
		}

		public bool RemoveRosaryBead(string idRosaryBead)
		{
			return this.RemoveRosaryBead(this.GetRosaryBead(idRosaryBead.ToUpper()));
		}

		public bool RemoveRosaryBead(RosaryBead bead)
		{
			bool flag = bead && this.ownBeads.Contains(bead);
			if (flag)
			{
				this.Unequip(bead, this.wearBeads);
				this.RemoveObject<RosaryBead>(bead, this.ownBeads);
			}
			return flag;
		}

		public bool IsRosaryBeadOwned(string idRosaryBead)
		{
			return this.IsRosaryBeadOwned(this.GetRosaryBead(idRosaryBead.ToUpper()));
		}

		public bool IsRosaryBeadOwned(RosaryBead rosaryBead)
		{
			bool result = false;
			if (rosaryBead != null)
			{
				result = this.ownBeads.Contains(rosaryBead);
			}
			return result;
		}

		public bool IsRosaryBeadEquipped(string idRosaryBead)
		{
			return this.IsRosaryBeadEquipped(this.GetRosaryBead(idRosaryBead.ToUpper()));
		}

		public bool IsRosaryBeadEquipped(RosaryBead bead)
		{
			bool flag = false;
			if (bead != null)
			{
				for (int i = 0; i < 8; i++)
				{
					flag = (this.wearBeads[i] == bead);
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		public bool IsAnyRosaryBeadEquiped()
		{
			bool result = false;
			for (int i = 0; i < 8; i++)
			{
				if (this.wearBeads[i] && this.wearBeads[i].IsEquiped)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public RosaryBead GetRosaryBeadInSlot(int slot)
		{
			if (slot >= 0 && slot < 8)
			{
				return this.wearBeads[slot];
			}
			return null;
		}

		public int GetRosaryBeadSlot(RosaryBead bead)
		{
			int result = -1;
			if (bead != null)
			{
				for (int i = 0; i < 8; i++)
				{
					if (this.wearBeads[i] == bead)
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		public int GetRosaryBeadSlots()
		{
			int result = 0;
			if (Core.Logic.Penitent)
			{
				result = (int)Core.Logic.Penitent.Stats.BeadSlots.Final;
			}
			return result;
		}

		public bool SetRosaryBeadInSlot(int slot, string idRosaryBead)
		{
			return this.SetRosaryBeadInSlot(slot, this.GetRosaryBead(idRosaryBead.ToUpper()));
		}

		public bool SetRosaryBeadInSlot(int slot, RosaryBead bead)
		{
			bool flag = slot >= 0 && slot < this.wearBeads.Length;
			if (flag)
			{
				if (this.wearBeads[slot] != null)
				{
					this.Unequip(this.wearBeads[slot], this.wearBeads);
				}
				if (bead)
				{
					this.Equip<RosaryBead>(bead, slot, this.wearBeads);
				}
			}
			return flag;
		}

		public ReadOnlyCollection<QuestItem> GetAllQuestItems()
		{
			return this.GetDictValueReadOnly<QuestItem>(this.allQuestItems);
		}

		public QuestItem GetQuestItem(string idQuestItem)
		{
			QuestItem result = null;
			this.allQuestItems.TryGetValue(idQuestItem.ToUpper(), out result);
			return result;
		}

		public ReadOnlyCollection<QuestItem> GetQuestItemOwned()
		{
			return this.ownQuestItems.AsReadOnly();
		}

		public bool AddQuestItem(string idQuestItem)
		{
			return this.AddQuestItem(this.GetQuestItem(idQuestItem.ToUpper()));
		}

		public bool AddQuestItem(QuestItem questItem)
		{
			bool result = false;
			if (questItem && !this.ownQuestItems.Contains(questItem))
			{
				result = true;
				this.AddObject<QuestItem>(questItem, this.ownQuestItems);
			}
			return result;
		}

		public bool RemoveQuestItem(string idQuestItem)
		{
			return this.RemoveQuestItem(this.GetQuestItem(idQuestItem.ToUpper()));
		}

		public bool RemoveQuestItem(QuestItem questItem)
		{
			bool result = false;
			if (questItem && this.ownQuestItems.Contains(questItem))
			{
				result = true;
				this.RemoveObject<QuestItem>(questItem, this.ownQuestItems);
			}
			return result;
		}

		public bool IsQuestItemOwned(string idQuestItem)
		{
			return this.IsQuestItemOwned(this.GetQuestItem(idQuestItem.ToUpper()));
		}

		public bool IsQuestItemOwned(QuestItem questItem)
		{
			bool result = false;
			if (questItem != null)
			{
				result = this.ownQuestItems.Contains(questItem);
			}
			return result;
		}

		public ReadOnlyCollection<Prayer> GetAllPrayers()
		{
			return this.GetDictValueReadOnly<Prayer>(this.allPrayers);
		}

		public Prayer GetPrayer(string idPrayer)
		{
			Prayer result = null;
			this.allPrayers.TryGetValue(idPrayer.ToUpper(), out result);
			return result;
		}

		public ReadOnlyCollection<Prayer> GetPrayersOwned()
		{
			return this.ownPrayers.AsReadOnly();
		}

		public ReadOnlyCollection<Prayer> GetPrayersOwned(Prayer.PrayerType prayerType)
		{
			return this.ownPrayers.FindAll((Prayer prayer) => prayer.prayerType == prayerType).AsReadOnly();
		}

		public bool AddPrayer(string idPrayer)
		{
			return this.AddPrayer(this.GetPrayer(idPrayer.ToUpper()));
		}

		public bool AddPrayer(Prayer prayer)
		{
			bool result = false;
			if (prayer && !this.ownPrayers.Contains(prayer))
			{
				result = true;
				this.AddObject<Prayer>(prayer, this.ownPrayers);
			}
			return result;
		}

		public bool RemovePrayer(string idPrayer)
		{
			return this.RemovePrayer(this.GetPrayer(idPrayer.ToUpper()));
		}

		public bool RemovePrayer(Prayer prayer)
		{
			bool flag = prayer && this.ownPrayers.Contains(prayer);
			if (flag)
			{
				this.Unequip(prayer, this.wearPrayers);
				this.RemoveObject<Prayer>(prayer, this.ownPrayers);
			}
			return flag;
		}

		public bool IsPrayerOwned(string idPrayer)
		{
			return this.IsPrayerOwned(this.GetPrayer(idPrayer.ToUpper()));
		}

		public bool IsPrayerOwned(Prayer prayer)
		{
			bool result = false;
			if (prayer != null)
			{
				result = this.ownPrayers.Contains(prayer);
			}
			return result;
		}

		public bool IsPrayerEquipped(string idPrayer)
		{
			return this.IsPrayerEquipped(this.GetPrayer(idPrayer.ToUpper()));
		}

		public bool IsAnyPrayerEquipped()
		{
			bool result = false;
			for (int i = 0; i < 1; i++)
			{
				if (this.wearPrayers[i] && this.wearPrayers[i].IsEquiped)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool IsPrayerEquipped(Prayer prayer)
		{
			bool flag = false;
			if (prayer != null)
			{
				for (int i = 0; i < 1; i++)
				{
					flag = (this.wearPrayers[i] == prayer);
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		public Prayer GetPrayerInSlot(int slot)
		{
			if (slot >= 0 && slot < 1)
			{
				return this.wearPrayers[slot];
			}
			return null;
		}

		public int GetPrayerInSlot(Prayer prayer)
		{
			int result = -1;
			if (prayer != null)
			{
				for (int i = 0; i < 1; i++)
				{
					if (this.wearPrayers[i] == prayer)
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		public bool SetPrayerInSlot(int slot, string idPrayer)
		{
			return this.SetPrayerInSlot(slot, this.GetPrayer(idPrayer.ToUpper()));
		}

		public bool SetPrayerInSlot(int slot, Prayer prayer)
		{
			bool flag = slot >= 0 && slot < 1;
			if (flag)
			{
				if (this.wearPrayers[slot] != null)
				{
					this.Unequip(this.wearPrayers[slot], this.wearPrayers);
				}
				if (prayer && prayer.IsDeciphered())
				{
					this.Equip<Prayer>(prayer, slot, this.wearPrayers);
				}
			}
			return flag;
		}

		public ReadOnlyCollection<Framework.Inventory.CollectibleItem> GetAllCollectibleItems()
		{
			return this.GetDictValueReadOnly<Framework.Inventory.CollectibleItem>(this.allCollectibleItems);
		}

		public Framework.Inventory.CollectibleItem GetCollectibleItem(string idCollectibleItem)
		{
			Framework.Inventory.CollectibleItem result = null;
			this.allCollectibleItems.TryGetValue(idCollectibleItem.ToUpper(), out result);
			return result;
		}

		public ReadOnlyCollection<Framework.Inventory.CollectibleItem> GetCollectibleItemOwned()
		{
			return this.ownCollectibleItems.AsReadOnly();
		}

		public bool AddCollectibleItem(string idCollectibleItem)
		{
			return this.AddCollectibleItem(this.GetCollectibleItem(idCollectibleItem.ToUpper()));
		}

		public bool AddCollectibleItem(Framework.Inventory.CollectibleItem collectibleItem)
		{
			bool result = false;
			if (collectibleItem && !this.ownCollectibleItems.Contains(collectibleItem))
			{
				result = true;
				this.AddObject<Framework.Inventory.CollectibleItem>(collectibleItem, this.ownCollectibleItems);
			}
			return result;
		}

		public bool RemoveCollectibleItem(string idCollectibleItem)
		{
			return this.RemoveCollectibleItem(this.GetCollectibleItem(idCollectibleItem.ToUpper()));
		}

		public bool RemoveCollectibleItem(Framework.Inventory.CollectibleItem collectibleItem)
		{
			bool result = false;
			if (collectibleItem && this.ownCollectibleItems.Contains(collectibleItem))
			{
				result = true;
				this.RemoveObject<Framework.Inventory.CollectibleItem>(collectibleItem, this.ownCollectibleItems);
			}
			return result;
		}

		public bool IsCollectibleItemOwned(string idCollectibleItem)
		{
			return this.IsCollectibleItemOwned(this.GetCollectibleItem(idCollectibleItem.ToUpper()));
		}

		public bool IsCollectibleItemOwned(Framework.Inventory.CollectibleItem collectibleItem)
		{
			bool result = false;
			if (collectibleItem != null)
			{
				result = this.ownCollectibleItems.Contains(collectibleItem);
			}
			return result;
		}

		public Sword GetSword(string idSword)
		{
			Sword result = null;
			if (this.allSwords != null)
			{
				this.allSwords.TryGetValue(idSword.ToUpper(), out result);
			}
			return result;
		}

		public ReadOnlyCollection<Sword> GetAllSwords()
		{
			return this.GetDictValueReadOnly<Sword>(this.allSwords);
		}

		public ReadOnlyCollection<Sword> GetSwordsOwned()
		{
			return this.ownSwords.AsReadOnly();
		}

		public bool AddSword(string idSword)
		{
			return this.AddSword(this.GetSword(idSword.ToUpper()));
		}

		public bool AddSword(Sword sword)
		{
			bool flag = sword && !this.ownSwords.Contains(sword);
			if (flag)
			{
				this.AddObject<Sword>(sword, this.ownSwords);
			}
			return flag;
		}

		public bool RemoveSword(string idSword)
		{
			return this.RemoveSword(this.GetSword(idSword.ToUpper()));
		}

		public bool RemoveSword(Sword sword)
		{
			bool flag = sword && this.ownSwords.Contains(sword);
			if (flag)
			{
				this.Unequip(sword, this.wearSwords);
				this.RemoveObject<Sword>(sword, this.ownSwords);
			}
			return flag;
		}

		public bool IsSwordOwned(string idSword)
		{
			return this.IsSwordOwned(this.GetSword(idSword.ToUpper()));
		}

		public bool IsSwordOwned(Sword sword)
		{
			bool result = false;
			if (sword != null)
			{
				result = this.ownSwords.Contains(sword);
			}
			return result;
		}

		public bool IsSwordEquipped(string idSword)
		{
			return this.IsSwordEquipped(this.GetSword(idSword.ToUpper()));
		}

		public bool IsSwordEquipped(Sword sword)
		{
			bool flag = false;
			if (sword != null)
			{
				for (int i = 0; i < 1; i++)
				{
					flag = (this.wearSwords[i] == sword);
					if (flag)
					{
						break;
					}
				}
			}
			return flag;
		}

		public bool IsAnySwordHeartEquiped()
		{
			bool result = false;
			for (int i = 0; i < 1; i++)
			{
				if (this.wearSwords[i] && this.wearSwords[i].IsEquiped)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool IsTrueSwordHeartEquiped()
		{
			bool result = false;
			for (int i = 0; i < 1; i++)
			{
				if (this.wearSwords[i] && this.wearSwords[i].IsEquiped && this.wearSwords[i].id.Equals("HE201"))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public int GetSwordInSlot(Sword sword)
		{
			int result = -1;
			if (sword != null)
			{
				for (int i = 0; i < 1; i++)
				{
					if (this.wearSwords[i] == sword)
					{
						result = i;
						break;
					}
				}
			}
			return result;
		}

		public Sword GetSwordInSlot(int slot)
		{
			if (slot >= 0 && slot < 1)
			{
				return this.wearSwords[slot];
			}
			return null;
		}

		public bool SetSwordInSlot(int slot, string idSword)
		{
			return this.SetSwordInSlot(slot, this.GetSword(idSword.ToUpper()));
		}

		public bool SetSwordInSlot(int slot, Sword sword)
		{
			bool flag = slot >= 0 && slot < 1;
			if (flag)
			{
				if (this.wearSwords[slot] != null)
				{
					this.Unequip(this.wearSwords[slot], this.wearSwords);
				}
				if (sword)
				{
					this.Equip<Sword>(sword, slot, this.wearSwords);
				}
			}
			return flag;
		}

		public void AddBossKey(int slot)
		{
			if (slot >= 0 && slot < 4)
			{
				this.ownBossKeys[slot] = true;
			}
		}

		public void RemoveBossKey(int slot)
		{
			if (slot >= 0 && slot < 4)
			{
				this.ownBossKeys[slot] = false;
			}
		}

		public bool CheckBossKey(int slot)
		{
			bool result = false;
			if (slot >= 0 && slot < 4)
			{
				result = this.ownBossKeys[slot];
			}
			return result;
		}

		public float GetPercentageCompletition()
		{
			float num = this.GetPercentageCompletitionList<Relic>(this.ownRellics);
			num += this.GetPercentageCompletitionList<RosaryBead>(this.ownBeads);
			num += this.GetPercentageCompletitionList<QuestItem>(this.ownQuestItems);
			num += this.GetPercentageCompletitionList<Framework.Inventory.CollectibleItem>(this.ownCollectibleItems);
			num += this.GetPercentageCompletitionList<Prayer>(this.ownPrayers);
			return num + this.GetPercentageCompletitionList<Sword>(this.ownSwords);
		}

		private float GetPercentageCompletitionList<T>(List<T> ownObj) where T : BaseInventoryObject
		{
			return (float)ownObj.Count((T x) => x.AddPercentageCompletition()) * GameConstants.PercentageValues[PersistentManager.PercentageType.ItemAdded];
		}

		private void OnLocalizationChange()
		{
			if (InventoryManager.currentLanguage != LocalizationManager.CurrentLanguage)
			{
				if (InventoryManager.currentLanguage != string.Empty)
				{
					Log.Debug("Inventory", "Language change, localize items to: " + LocalizationManager.CurrentLanguage, null);
				}
				int languageIndexFromCode = this.language.GetLanguageIndexFromCode(LocalizationManager.CurrentLanguageCode, true);
				this.TranslateAllObjects<RosaryBead>(this.allBeads, languageIndexFromCode);
				this.TranslateAllObjects<Relic>(this.allRellics, languageIndexFromCode);
				this.TranslateAllObjects<QuestItem>(this.allQuestItems, languageIndexFromCode);
				this.TranslateAllObjects<Framework.Inventory.CollectibleItem>(this.allCollectibleItems, languageIndexFromCode);
				this.TranslateAllObjects<Prayer>(this.allPrayers, languageIndexFromCode);
				this.TranslateAllObjects<Sword>(this.allSwords, languageIndexFromCode);
				this.TranslateObject<TearsObject>(this.TearsGenericObject, languageIndexFromCode);
				InventoryManager.currentLanguage = LocalizationManager.CurrentLanguage;
			}
		}

		private void TranslateAllObjects<T>(Dictionary<string, T> allObjects, int idxLanguage) where T : BaseInventoryObject
		{
			foreach (KeyValuePair<string, T> keyValuePair in allObjects)
			{
				this.TranslateObject<T>(keyValuePair.Value, idxLanguage);
			}
		}

		private void TranslateObject<T>(T entry, int idxLanguage) where T : BaseInventoryObject
		{
			string str = entry.GetBaseTranslationID() + "_";
			foreach (string text in InventoryManager.LANGUAGE_ELEMENT_LIST)
			{
				string text2 = str + text.ToUpper();
				TermData termData = this.language.GetTermData(text2, false);
				if (termData == null)
				{
					UnityEngine.Debug.LogError("Term " + text2 + " not found in Inventory Localization");
				}
				else if (termData.Languages.ElementAtOrDefault(idxLanguage) != null)
				{
					string text3 = termData.Languages[idxLanguage];
					if (text != null)
					{
						if (!(text == "caption"))
						{
							if (!(text == "description"))
							{
								if (text == "lore")
								{
									entry.lore = text3;
								}
							}
							else
							{
								entry.description = text3;
							}
						}
						else
						{
							entry.caption = text3;
						}
					}
				}
			}
		}

		private void AddObject<T>(T invObj, List<T> ownObj) where T : BaseInventoryObject
		{
			Core.Metrics.CustomEvent("ITEM_ADQUIRED", invObj.name, -1f);
			ownObj.Add(invObj);
			invObj.Add();
			this.LastAddedObjectType = invObj.GetItemType();
			this.LastAddedObject = invObj;
			if (invObj.AddPercentageCompletition())
			{
				Core.AchievementsManager.CheckProgressToAC46();
			}
		}

		private void RemoveObjectForNewGamePlus<T>(List<T> ownObj) where T : BaseInventoryObject
		{
			List<T> list = new List<T>(ownObj);
			foreach (T t in list)
			{
				if (!t.preserveInNewGamePlus)
				{
					ownObj.Remove(t);
					t.Remove();
				}
			}
		}

		private void RemoveObject<T>(T invObj, List<T> ownObj) where T : BaseInventoryObject
		{
			ownObj.Remove(invObj);
			invObj.Remove();
		}

		private void Unequip(EquipableInventoryObject invObj, EquipableInventoryObject[] wearObj)
		{
			for (int i = 0; i < wearObj.Length; i++)
			{
				if (wearObj[i] == invObj)
				{
					invObj.UnEquip();
					wearObj[i] = null;
				}
			}
		}

		private void UnequipAll(EquipableInventoryObject[] wearObj)
		{
			for (int i = 0; i < wearObj.Length; i++)
			{
				if (wearObj[i] != null)
				{
					wearObj[i].UnEquip();
					wearObj[i] = null;
				}
			}
		}

		private void Equip<T>(T invObj, int slot, T[] wearObj) where T : EquipableInventoryObject
		{
			Core.Metrics.CustomEvent("ITEM_EQUIPED", invObj.name, -1f);
			wearObj[slot] = invObj;
			invObj.Equip();
			if (this.OnItemEquiped != null)
			{
				this.OnItemEquiped(invObj.id);
			}
		}

		private ReadOnlyCollection<T> GetDictValueReadOnly<T>(Dictionary<string, T> dictionary)
		{
			List<T> list = new List<T>();
			foreach (T item in dictionary.Values)
			{
				list.Add(item);
			}
			return list.AsReadOnly();
		}

		public static List<string> GetAllRelicsId()
		{
			InventoryManager.ReloadBaseElemetsIDIfNeed<Relic>(InventoryManager.cachedRelicsId);
			return InventoryManager.cachedRelicsId;
		}

		public static List<string> GetAllBeadsId()
		{
			InventoryManager.ReloadBaseElemetsIDIfNeed<RosaryBead>(InventoryManager.cachedBeadsId);
			return InventoryManager.cachedBeadsId;
		}

		public static List<string> GetAllQuestItemsId()
		{
			InventoryManager.ReloadBaseElemetsIDIfNeed<QuestItem>(InventoryManager.cachedQuestItemsId);
			return InventoryManager.cachedQuestItemsId;
		}

		public static List<string> GetAllPrayersId()
		{
			InventoryManager.ReloadBaseElemetsIDIfNeed<Prayer>(InventoryManager.cachedPrayersId);
			return InventoryManager.cachedPrayersId;
		}

		public static List<string> GetAllCollectibleItemsId()
		{
			InventoryManager.ReloadBaseElemetsIDIfNeed<Framework.Inventory.CollectibleItem>(InventoryManager.cachedCollectibleItemsId);
			return InventoryManager.cachedCollectibleItemsId;
		}

		public static List<string> GetAllSwordsId()
		{
			InventoryManager.ReloadBaseElemetsIDIfNeed<Sword>(InventoryManager.cachedSwordsId);
			return InventoryManager.cachedSwordsId;
		}

		public static LanguageSource GetLanguageSource()
		{
			GameObject asset = ResourceManager.pInstance.GetAsset<GameObject>("Inventory/Languages");
			return (!asset) ? null : asset.GetComponent<LanguageSource>();
		}

		private static void ReloadBaseElemetsIDIfNeed<BaseClass>(List<string> list) where BaseClass : BaseInventoryObject
		{
			if (InventoryManager.forceReload || list.Count == 0)
			{
				list.Clear();
				UnityEngine.Object[] array = Resources.LoadAll("Inventory/" + typeof(BaseClass).Name);
				foreach (UnityEngine.Object original in array)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
					BaseClass component = gameObject.GetComponent<BaseClass>();
					if (component != null)
					{
						list.Add(component.id);
					}
					UnityEngine.Object.Destroy(gameObject);
				}
				list.Sort();
			}
			InventoryManager.forceReload = false;
		}

		public int GetOrder()
		{
			return 0;
		}

		public string GetPersistenID()
		{
			return "ID_INVENTORY";
		}

		public void ResetPersistence()
		{
			this.ResetObjectsEffects();
			this.InitializeObjects();
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			InventoryManager.InventoryPersistenceData inventoryPersistenceData = new InventoryManager.InventoryPersistenceData();
			inventoryPersistenceData.ownRellics = this.GetListId<Relic>(this.ownRellics);
			for (int i = 0; i < this.wearRellics.Length; i++)
			{
				inventoryPersistenceData.wearRellics[i] = this.GetIdIfAny<Relic>(this.wearRellics[i]);
			}
			inventoryPersistenceData.ownBeads = this.GetListId<RosaryBead>(this.ownBeads);
			for (int j = 0; j < this.wearBeads.Length; j++)
			{
				inventoryPersistenceData.wearBeads[j] = this.GetIdIfAny<RosaryBead>(this.wearBeads[j]);
			}
			inventoryPersistenceData.ownQuestItems = this.GetListId<QuestItem>(this.ownQuestItems);
			inventoryPersistenceData.ownCollectibleItems = this.GetListId<Framework.Inventory.CollectibleItem>(this.ownCollectibleItems);
			inventoryPersistenceData.ownSwords = this.GetListId<Sword>(this.ownSwords);
			for (int k = 0; k < this.wearSwords.Length; k++)
			{
				inventoryPersistenceData.wearSwords[k] = this.GetIdIfAny<Sword>(this.wearSwords[k]);
			}
			inventoryPersistenceData.ownPrayers = this.GetListId<Prayer>(this.ownPrayers);
			for (int l = 0; l < this.wearPrayers.Length; l++)
			{
				inventoryPersistenceData.wearPrayers[l] = this.GetIdIfAny<Prayer>(this.wearPrayers[l]);
			}
			foreach (KeyValuePair<string, Prayer> keyValuePair in this.allPrayers)
			{
				if (keyValuePair.Value.CurrentDecipher != 0)
				{
					inventoryPersistenceData.prayerDecipher[keyValuePair.Value.id] = keyValuePair.Value.CurrentDecipher;
				}
			}
			for (int m = 0; m < this.ownBossKeys.Length; m++)
			{
				inventoryPersistenceData.keys[m] = this.ownBossKeys[m];
			}
			return inventoryPersistenceData;
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			InventoryManager.InventoryPersistenceData inventoryPersistenceData = (InventoryManager.InventoryPersistenceData)data;
			this.ownRellics = this.GetInstanceList<Relic>(inventoryPersistenceData.ownRellics, new Func<string, Relic>(this.GetRelic));
			for (int i = 0; i < inventoryPersistenceData.wearRellics.Length; i++)
			{
				string idRelic = inventoryPersistenceData.wearRellics[i];
				this.SetRelicInSlot(i, idRelic);
			}
			this.ownBeads = this.GetInstanceList<RosaryBead>(inventoryPersistenceData.ownBeads, new Func<string, RosaryBead>(this.GetRosaryBead));
			for (int j = 0; j < inventoryPersistenceData.wearBeads.Length; j++)
			{
				string idRosaryBead = inventoryPersistenceData.wearBeads[j];
				this.SetRosaryBeadInSlot(j, idRosaryBead);
			}
			this.ownQuestItems = this.GetInstanceList<QuestItem>(inventoryPersistenceData.ownQuestItems, new Func<string, QuestItem>(this.GetQuestItem));
			this.ownCollectibleItems = this.GetInstanceList<Framework.Inventory.CollectibleItem>(inventoryPersistenceData.ownCollectibleItems, new Func<string, Framework.Inventory.CollectibleItem>(this.GetCollectibleItem));
			this.ownSwords = this.GetInstanceList<Sword>(inventoryPersistenceData.ownSwords, new Func<string, Sword>(this.GetSword));
			for (int k = 0; k < inventoryPersistenceData.wearSwords.Length; k++)
			{
				string idSword = inventoryPersistenceData.wearSwords[k];
				this.SetSwordInSlot(k, idSword);
			}
			this.ownPrayers = this.GetInstanceList<Prayer>(inventoryPersistenceData.ownPrayers, new Func<string, Prayer>(this.GetPrayer));
			for (int l = 0; l < inventoryPersistenceData.wearPrayers.Length; l++)
			{
				string idPrayer = inventoryPersistenceData.wearPrayers[l];
				this.SetPrayerInSlot(l, idPrayer);
			}
			foreach (KeyValuePair<string, Prayer> keyValuePair in this.allPrayers)
			{
				Prayer value = keyValuePair.Value;
				value.ResetDecipher();
				if (inventoryPersistenceData.prayerDecipher.ContainsKey(value.id))
				{
					value.AddDecipher(inventoryPersistenceData.prayerDecipher[value.id]);
				}
			}
			for (int m = 0; m < inventoryPersistenceData.keys.Length; m++)
			{
				this.ownBossKeys[m] = inventoryPersistenceData.keys[m];
			}
		}

		public void ResetObjectsEffects()
		{
			this.ResetEffects<RosaryBead>(this.ownBeads);
			this.ResetEffects<Framework.Inventory.CollectibleItem>(this.ownCollectibleItems);
			this.ResetEffects<Prayer>(this.ownPrayers);
			this.ResetEffects<QuestItem>(this.ownQuestItems);
			this.ResetEffects<Relic>(this.ownRellics);
			this.ResetEffects<Sword>(this.ownSwords);
		}

		private void ResetEffects<T>(List<T> ownObjects) where T : BaseInventoryObject
		{
			List<T> list = new List<T>(ownObjects);
			foreach (T t in list)
			{
				t.Reset();
			}
		}

		private string GetIdIfAny<T>(T element) where T : BaseInventoryObject
		{
			string result = string.Empty;
			if (element)
			{
				result = element.id;
			}
			return result;
		}

		private List<string> GetListId<T>(List<T> instanceList) where T : BaseInventoryObject
		{
			List<string> list = new List<string>();
			foreach (T t in instanceList)
			{
				BaseInventoryObject baseInventoryObject = t;
				list.Add(baseInventoryObject.id);
			}
			return list;
		}

		private List<T> GetInstanceList<T>(List<string> idList, Func<string, T> funcGet) where T : BaseInventoryObject
		{
			List<T> list = new List<T>();
			foreach (string text in idList)
			{
				T t = funcGet(text);
				if (t)
				{
					list.Add(t);
					t.Add();
				}
				else
				{
					UnityEngine.Debug.LogError("*** Inventory Persistence, missing ID:" + text);
				}
			}
			return list;
		}

		private GameObject mainObject;

		private InventoryManager.ItemType _LastAddedObjectType = InventoryManager.ItemType.Bead;

		private const string LANGUAGE_PREFAB_NAME = "Inventory/Languages";

		public static readonly string[] LANGUAGE_ELEMENT_LIST = new string[]
		{
			"caption",
			"description",
			"lore"
		};

		private LanguageSource language;

		private static string currentLanguage = string.Empty;

		public const int NUM_RELLICS_SLOTS = 3;

		private Dictionary<string, Relic> allRellics;

		private List<Relic> ownRellics;

		private Relic[] wearRellics = new Relic[3];

		public const int MAX_BEADS_SLOTS = 8;

		private Dictionary<string, RosaryBead> allBeads;

		private List<RosaryBead> ownBeads;

		private RosaryBead[] wearBeads = new RosaryBead[8];

		private Dictionary<string, QuestItem> allQuestItems;

		private List<QuestItem> ownQuestItems;

		public const int MAX_PRAYERS_SLOTS = 1;

		private Dictionary<string, Prayer> allPrayers;

		private List<Prayer> ownPrayers;

		private Prayer[] wearPrayers = new Prayer[1];

		private Dictionary<string, Framework.Inventory.CollectibleItem> allCollectibleItems;

		private List<Framework.Inventory.CollectibleItem> ownCollectibleItems;

		public const int NUM_SWORDS_SLOTS = 1;

		public const string TRUE_SWORD_ID = "HE201";

		private Dictionary<string, Sword> allSwords;

		private List<Sword> ownSwords;

		private Sword[] wearSwords = new Sword[1];

		public const int NUM_BOSS_KEYS = 4;

		private bool[] ownBossKeys = new bool[4];

		private float PlayerHealth = -1f;

		private float NumOfFlasks = -1f;

		private List<string> ObjectsConvertedToTears = new List<string>();

		private static bool forceReload = true;

		private static List<string> cachedRelicsId = new List<string>();

		private static List<string> cachedQuestItemsId = new List<string>();

		private static List<string> cachedPrayersId = new List<string>();

		private static List<string> cachedBeadsId = new List<string>();

		private static List<string> cachedCollectibleItemsId = new List<string>();

		private static List<string> cachedSwordsId = new List<string>();

		private const string PERSITENT_ID = "ID_INVENTORY";

		public enum ItemType
		{
			Relic,
			Prayer,
			Bead,
			Quest,
			Collectible,
			Sword
		}

		public delegate void ItemEvent(string item);

		[Serializable]
		public class InventoryPersistenceData : PersistentManager.PersistentData
		{
			public InventoryPersistenceData() : base("ID_INVENTORY")
			{
			}

			public List<string> ownRellics = new List<string>();

			public string[] wearRellics = new string[3];

			public List<string> ownBeads = new List<string>();

			public string[] wearBeads = new string[8];

			public List<string> ownQuestItems = new List<string>();

			public List<string> ownCollectibleItems = new List<string>();

			public List<string> ownPrayers = new List<string>();

			public string[] wearPrayers = new string[1];

			public Dictionary<string, int> prayerDecipher = new Dictionary<string, int>();

			public List<string> ownSwords = new List<string>();

			public string[] wearSwords = new string[1];

			public bool[] keys = new bool[4];
		}
	}
}
