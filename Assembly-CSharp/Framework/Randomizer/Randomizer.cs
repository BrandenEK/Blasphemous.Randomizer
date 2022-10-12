using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using UnityEngine;

namespace Framework.Randomizer
{
	public class Randomizer : GameSystem, PersistentInterface
	{
		public string GetPersistenID()
		{
			return "ID_RANDOMIZER";
		}

		public int GetOrder()
		{
			return 0;
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			return new RandomizerPersistenceData
			{
				seed = this.seed,
				itemsCollected = this.itemsCollected,
				startedInRando = this.startedInRando,
				randomizerSettings = this.gameConfig
			};
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			RandomizerPersistenceData randomizerPersistenceData = (RandomizerPersistenceData)data;
			this.seed = randomizerPersistenceData.seed;
			this.startedInRando = randomizerPersistenceData.startedInRando;
			this.itemsCollected = randomizerPersistenceData.itemsCollected;
			this.gameConfig = randomizerPersistenceData.randomizerSettings;
			if (!this.inGame)
			{
				this.Log("Loading seed: " + this.seed, 0);
				if (PersistentManager.GetAutomaticSlot() != this.lastSlotLoaded)
				{
					this.Randomize(this.seed, true);
				}
				this.inGame = true;
				this.lastSlotLoaded = PersistentManager.GetAutomaticSlot();
			}
		}

		public void ResetPersistence()
		{
			this.itemsCollected = 0;
			this.seed = -1;
			this.startedInRando = false;
			this.lastReward = null;
			this.gameConfig = this.fileConfig;
			for (int i = 0; i < this.logs.Length; i++)
			{
				this.logs[i].Clear();
			}
			this.inGame = false;
		}

		public int itemsCollected { get; private set; }

		public int totalItems { get; private set; }

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
			this.inGame = false;
			this.lastSlotLoaded = -1;
		}

		private Reward getRewardFromId(string id)
		{
			if (this.newItems == null)
			{
				UIController.instance.ShowPopUp("Error: Rewards list not initialized!", "", 0f, false);
				return null;
			}
			if (!this.newItems.ContainsKey(id))
			{
				this.Log("Location " + id + " was not loaded!");
				return null;
			}
			if (this.checkForDuplicate(id))
			{
				this.Log("Location " + id + " is already retrieved!");
				return null;
			}
			return this.newItems[id];
		}

		public void giveReward(string id, bool showMessage)
		{
			Reward rewardFromId = this.getRewardFromId(id);
			if (rewardFromId != null)
			{
				this.giveReward(rewardFromId);
				this.lastReward = rewardFromId;
				if (showMessage)
				{
					this.displayReward(rewardFromId);
				}
			}
		}

		private void giveReward(Reward reward)
		{
			InventoryManager inventoryManager = Core.InventoryManager;
			EntityStats stats = Core.Logic.Penitent.Stats;
			int itemsCollected = this.itemsCollected;
			this.itemsCollected = itemsCollected + 1;
			this.Log(string.Format("Reward: ({0}) {1}", reward.type, reward.id), 0);
			switch (reward.type)
			{
			case 0:
			{
				BaseInventoryObject baseObject = inventoryManager.GetBaseObject("RB" + reward.id.ToString("00"), InventoryManager.ItemType.Bead);
				inventoryManager.AddBaseObjectOrTears(baseObject);
				return;
			}
			case 1:
			{
				BaseInventoryObject baseObject2 = inventoryManager.GetBaseObject("PR" + reward.id.ToString("00"), InventoryManager.ItemType.Prayer);
				inventoryManager.AddBaseObjectOrTears(baseObject2);
				return;
			}
			case 2:
			{
				BaseInventoryObject inbObject = inventoryManager.GetBaseObject("RE" + reward.id.ToString("00"), InventoryManager.ItemType.Relic);
				inbObject = inventoryManager.AddBaseObjectOrTears(inbObject);
				return;
			}
			case 3:
			{
				BaseInventoryObject baseObject3 = inventoryManager.GetBaseObject("HE" + reward.id.ToString("00"), InventoryManager.ItemType.Sword);
				inventoryManager.AddBaseObjectOrTears(baseObject3);
				return;
			}
			case 4:
			{
				BaseInventoryObject baseObject4 = inventoryManager.GetBaseObject("CO" + reward.id.ToString("00"), InventoryManager.ItemType.Collectible);
				inventoryManager.AddBaseObjectOrTears(baseObject4);
				return;
			}
			case 5:
			{
				BaseInventoryObject baseObject5 = inventoryManager.GetBaseObject("QI" + reward.id.ToString("00"), InventoryManager.ItemType.Quest);
				inventoryManager.AddBaseObjectOrTears(baseObject5);
				return;
			}
			case 6:
			{
				string id = "RESCUED_CHERUB_" + reward.id.ToString("00");
				Core.Events.SetFlag(id, true, false);
				return;
			}
			case 7:
				stats.Life.Upgrade();
				stats.Life.SetToCurrentMax();
				return;
			case 8:
				stats.Fervour.Upgrade();
				stats.Fervour.SetToCurrentMax();
				return;
			case 9:
				stats.MeaCulpa.Upgrade();
				stats.Strength.Upgrade();
				return;
			case 10:
				stats.Purge.Current += (float)reward.id;
				return;
			default:
				return;
			}
		}

		private void displayReward(Reward reward)
		{
			InventoryManager inventoryManager = Core.InventoryManager;
			EntityStats stats = Core.Logic.Penitent.Stats;
			RewardAchievement achievement;
			switch (reward.type)
			{
			case 0:
			{
				BaseInventoryObject baseObject = inventoryManager.GetBaseObject("RB" + reward.id.ToString("00"), InventoryManager.ItemType.Bead);
				achievement = new RewardAchievement(baseObject.caption, "New rosary bead obtained!", baseObject.picture);
				break;
			}
			case 1:
			{
				BaseInventoryObject baseObject2 = inventoryManager.GetBaseObject("PR" + reward.id.ToString("00"), InventoryManager.ItemType.Prayer);
				achievement = new RewardAchievement(baseObject2.caption, "New prayer obtained!", baseObject2.picture);
				break;
			}
			case 2:
			{
				BaseInventoryObject baseObject3 = inventoryManager.GetBaseObject("RE" + reward.id.ToString("00"), InventoryManager.ItemType.Relic);
				achievement = new RewardAchievement(baseObject3.caption, "New relic obtained!", baseObject3.picture);
				break;
			}
			case 3:
			{
				BaseInventoryObject baseObject4 = inventoryManager.GetBaseObject("HE" + reward.id.ToString("00"), InventoryManager.ItemType.Sword);
				achievement = new RewardAchievement(baseObject4.caption, "New sword heart obtained!", baseObject4.picture);
				break;
			}
			case 4:
			{
				BaseInventoryObject baseObject5 = inventoryManager.GetBaseObject("CO" + reward.id.ToString("00"), InventoryManager.ItemType.Collectible);
				achievement = new RewardAchievement(baseObject5.caption, "New collectible obtained!", baseObject5.picture);
				break;
			}
			case 5:
			{
				BaseInventoryObject baseObject6 = inventoryManager.GetBaseObject("QI" + reward.id.ToString("00"), InventoryManager.ItemType.Quest);
				achievement = new RewardAchievement(baseObject6.caption, "New quest item obtained!", baseObject6.picture);
				break;
			}
			case 6:
				achievement = new RewardAchievement("Cherub " + CherubCaptorPersistentObject.CountRescuedCherubs() + "/38", "Cherub rescued!", this.customImages[0]);
				break;
			case 7:
				achievement = new RewardAchievement("Life Upgrade " + stats.Life.GetUpgrades() + "/6", "Stat increased!", this.customImages[1]);
				break;
			case 8:
				achievement = new RewardAchievement("Fervour Upgrade " + stats.Fervour.GetUpgrades() + "/6", "Stat increased!", this.customImages[2]);
				break;
			case 9:
				achievement = new RewardAchievement("Mea Culpa Upgrade " + stats.MeaCulpa.GetUpgrades() + "/7", "Stat increased!", this.customImages[3]);
				break;
			case 10:
			{
				TearsObject tearsGenericObject = inventoryManager.TearsGenericObject;
				achievement = new RewardAchievement(tearsGenericObject.caption, reward.id + " tears added!", tearsGenericObject.picture);
				break;
			}
			default:
				return;
			}
			Core.AchievementsManager.ShowPopUp = true;
			UIController.instance.ShowPopupAchievement(achievement);
		}

		public void showNotification(string id)
		{
			Reward reward = (id == "QI78") ? this.lastReward : this.getRewardFromId(id);
			if (reward != null)
			{
				this.displayReward(reward);
			}
			if (id == "QI110")
			{
				int itemsCollected = this.itemsCollected;
				this.itemsCollected = itemsCollected + 1;
			}
		}

		public override void Update()
		{
			if (Input.GetKeyDown(KeyCode.Keypad7))
			{
				string log = this.logs[0].getLog();
				UIController.instance.ShowPopUp(log, "", 0f, false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Keypad8))
			{
				string log2 = this.logs[1].getLog();
				UIController.instance.ShowPopUp(log2, "", 0f, false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Keypad9))
			{
				string log3 = this.logs[2].getLog();
				UIController.instance.ShowPopUp(log3, "", 0f, false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				string message = "Current Seed: " + this.seed;
				UIController.instance.ShowPopUp(message, "", 0f, false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Keypad3) && this.seed == -1)
			{
				this.giveReward(new Reward(0, 203, true));
				UIController.instance.ShowPopUp("Giving the Speed rosary", "", 0f, false);
				return;
			}
		}

		public void Log(string message)
		{
			this.logs[0].Log(message);
		}

		public void LogFile(string message)
		{
			new FileIO().writeAll(message, "data.txt");
		}

		private int generateSeed()
		{
			if (this.gameConfig.customSeed > 0)
			{
				return this.gameConfig.customSeed;
			}
			return new System.Random().Next();
		}

		public void newGame()
		{
			this.inGame = true;
			this.startedInRando = true;
			this.gameConfig = this.fileConfig;
			this.setupExtras();
			this.seed = this.generateSeed();
			this.Log("Generating new seed: " + this.seed, 0);
			this.Randomize(this.seed, true);
			this.lastSlotLoaded = PersistentManager.GetAutomaticSlot();
		}

		private void setCutsceneFlags()
		{
			foreach (string id in this.cutsceneFlags)
			{
				Core.Events.SetFlag(id, true, false);
			}
		}

		public override void Start()
		{
			this.file = new FileIO();
			this.fileConfig = this.file.loadConfig();
			if (this.fileConfig.versionCreated != Randomizer.getVersion())
			{
				this.fileConfig = this.file.createNewConfig();
			}
			this.gameConfig = this.fileConfig;
			this.logs = new Logger[3];
			for (int i = 0; i < this.logs.Length; i++)
			{
				this.logs[i] = new Logger(4);
			}
			this.customImages = new Sprite[]
			{
				Core.AchievementsManager.Achievements["AC46"].Image,
				Core.AchievementsManager.Achievements["AC44"].Image,
				Core.AchievementsManager.Achievements["AC43"].Image,
				Core.AchievementsManager.Achievements["AC42"].Image
			};
			this.cutsceneFlags = new string[]
			{
				"PONTIFF_ALBERO_EVENT",
				"BROTHERS_EVENT1_COMPLETED",
				"BROTHERS_EVENT2_COMPLETED",
				"BROTHERS_GRAVEYARD_EVENT",
				"BROTHERS_WASTELAND_EVENT"
			};
			this.potentialDuplicates = new string[]
			{
				"BS13",
				"BS16",
				"QI201"
			};
		}

		private void setupExtras()
		{
			if (!this.gameConfig.allowPenitence)
			{
				Core.Events.SetFlag("PENITENCE_EVENT_FINISHED", true, false);
				Core.Events.SetFlag("PENITENCE_NO_PENITENCE", true, false);
			}
			if (this.gameConfig.skipCutscenes)
			{
				this.setCutsceneFlags();
			}
		}

		public Config gameConfig { get; private set; }

		public void Log(string message, int type)
		{
			if (type >= 0 && type < this.logs.Length)
			{
				this.logs[type].Log(message);
				if (type == 999)
				{
					this.file.writeLine(message + "\n", "log.txt");
				}
			}
		}

		public static string getVersion()
		{
			return "v0.2.0";
		}

		private bool checkForDuplicate(string id)
		{
			int i = 0;
			while (i < this.potentialDuplicates.Length)
			{
				if (id == this.potentialDuplicates[i])
				{
					if (Core.Events.GetFlag("Location_" + id))
					{
						return true;
					}
					Core.Events.SetFlag("Location_" + id, true, false);
					return false;
				}
				else
				{
					i++;
				}
			}
			return false;
		}

		private void onLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.Log(newLevel.LevelName, 0);
		}

		public override void Dispose()
		{
			LevelManager.OnLevelLoaded -= this.onLevelLoaded;
		}

		public override void Initialize()
		{
			LevelManager.OnLevelLoaded += this.onLevelLoaded;
		}

		private void Randomize(int seed, bool newGame)
		{
			if (seed == -1)
			{
				this.seed = -1;
				this.newItems = null;
				return;
			}
			this.newItems = new Dictionary<string, Reward>();
			ForwardFiller forwardFiller = new ForwardFiller(this.gameConfig.randomizerSettings, newGame);
			while (!forwardFiller.Fill(seed, this.newItems))
			{
				seed++;
				this.Log("Seed invalid - trying " + seed, 0);
			}
			this.seed = seed;
			this.totalItems = this.newItems.Count;
		}

		private int seed;

		private Dictionary<string, Reward> newItems;

		private bool startedInRando;

		private bool inGame;

		private string[] cutsceneFlags;

		private Config fileConfig;

		private Logger[] logs;

		public Reward lastReward;

		private Sprite[] customImages;

		private string[] potentialDuplicates;

		private FileIO file;

		private int lastSlotLoaded;
	}
}
