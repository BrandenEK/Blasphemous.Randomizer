using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Dialog;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Framework.Randomizer.Config;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using Tools.Level;
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
				config = this.gameConfig
			};
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			if (data != null)
			{
				RandomizerPersistenceData randomizerPersistenceData = (RandomizerPersistenceData)data;
				this.seed = randomizerPersistenceData.seed;
				this.startedInRando = randomizerPersistenceData.startedInRando;
				this.itemsCollected = randomizerPersistenceData.itemsCollected;
				this.gameConfig = randomizerPersistenceData.config;
			}
			if (!this.inGame)
			{
				this.Log("Loading seed: " + this.seed, 0);
				if (this.seed == -1)
				{
					this.newItems = null;
					this.enemizer = null;
					this.gameConfig = this.fileConfig;
					this.totalItems = 0;
					this.errorOnLoad = "This save file was not created in randomizer.  Item locations are invalid!";
				}
				else if (!this.isConfigVersionValid(this.gameConfig.versionCreated))
				{
					this.newItems = null;
					this.enemizer = null;
					this.gameConfig = this.fileConfig;
					this.totalItems = 0;
					this.errorOnLoad = "This save file was created in an older version of the randomizer.  Item locations are invalid!";
				}
				else if (PersistentManager.GetAutomaticSlot() != this.lastSlotLoaded)
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
			this.errorOnLoad = "";
		}

		public int itemsCollected { get; private set; }

		public int totalItems { get; private set; }

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
			this.inGame = false;
			this.lastSlotLoaded = -1;
			this.errorOnLoad = "";
		}

		public void giveReward(string id, bool showMessage)
		{
			Reward reward = this.getRewardFromId(id, true);
			if (reward != null)
			{
				if (reward.type == 99)
				{
					ProgressiveReward progressiveReward = reward as ProgressiveReward;
					reward = progressiveReward.getLevel(true);
					this.giveReward(reward);
					progressiveReward.removeItem();
				}
				else
				{
					this.giveReward(reward);
				}
				this.lastReward = reward;
				if (showMessage)
				{
					this.displayReward(reward);
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
			RewardInfo rewardInfo = this.GetRewardInfo(reward, false);
			if (rewardInfo == null)
			{
				return;
			}
			RewardAchievement achievement = new RewardAchievement(rewardInfo.name, rewardInfo.notification, rewardInfo.sprite);
			Core.AchievementsManager.ShowPopUp = true;
			UIController.instance.ShowPopupAchievement(achievement);
		}

		public void showNotification(string id)
		{
			Reward reward = "QI78RB17RB18RB19RB24RB25RB26".Contains(id) ? this.lastReward : this.getRewardFromId(id, false);
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
			if (Input.GetKeyDown(KeyCode.Keypad7) && this.fileConfig.debug.type > 0)
			{
				string log = this.logs[0].getLog();
				UIController.instance.ShowPopUp(log, "", 0f, false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Keypad8) && this.fileConfig.debug.type > 0)
			{
				string log2 = this.logs[1].getLog();
				UIController.instance.ShowPopUp(log2, "", 0f, false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Keypad9) && this.fileConfig.debug.type > 0)
			{
				string log3 = this.logs[2].getLog();
				UIController.instance.ShowPopUp(log3, "", 0f, false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Keypad6))
			{
				string message = "Current Seed: " + this.seed;
				UIController.instance.ShowPopUp(message, "", 0f, false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Keypad3))
			{
				this.itemsCollected = this.itemsCollected;
				this.LogFile(Roomemizer.getData());
			}
		}

		public void Log(string message)
		{
			this.logs[0].Log("Using wrong log function: " + message);
		}

		public void LogFile(string message)
		{
			if (this.fileConfig.debug.type > 1)
			{
				new FileIO().writeAll(message, "data.txt");
			}
		}

		private int generateSeed()
		{
			if (this.gameConfig.general.customSeed > 0)
			{
				return this.gameConfig.general.customSeed;
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
			if (!this.isConfigVersionValid(this.fileConfig.versionCreated))
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
			this.cutsceneNames = new string[]
			{
				"IntroBrotherhood",
				"IntroDeosgracias",
				"CTS12-Intro2",
				"CTS07-Deosgracias",
				"MeaCulpa"
			};
			this.cutsceneFlags = new string[]
			{
				"PONTIFF_ALBERO_EVENT",
				"PONTIFF_BRIDGE_EVENT",
				"PONTIFF_KEY1_USED",
				"PONTIFF_KEY2_USED",
				"PONTIFF_KEY3_USED",
				"PONTIFF_ARCHDEACON1_EVENT",
				"PONTIFF_ARCHDEACON2_EVENT",
				"BROTHERS_EVENT1_COMPLETED",
				"BROTHERS_EVENT2_COMPLETED",
				"BROTHERS_GRAVEYARD_EVENT",
				"BROTHERS_WASTELAND_EVENT"
			};
		}

		private void setupExtras()
		{
			if (!this.gameConfig.general.enablePenitence)
			{
				Core.Events.SetFlag("PENITENCE_EVENT_FINISHED", true, false);
				Core.Events.SetFlag("PENITENCE_NO_PENITENCE", true, false);
			}
			if (this.gameConfig.general.skipCutscenes)
			{
				this.setCutsceneFlags();
			}
		}

		public void Log(string message, int type)
		{
			if (type >= 0 && type < this.logs.Length)
			{
				this.logs[type].Log(message);
				if (this.fileConfig.debug.type > 1)
				{
					this.file.writeLine(message + "\n", "log.txt");
				}
			}
		}

		public static string getVersion()
		{
			return "v0.5.0";
		}

		private bool checkForDuplicate(string id)
		{
			if (Core.Events.GetFlag("Location_" + id))
			{
				return true;
			}
			Core.Events.SetFlag("Location_" + id, true, false);
			return false;
		}

		private void onLevelLoaded(Level oldLevel, Level newLevel)
		{
			if (this.errorOnLoad != "")
			{
				this.Log(this.errorOnLoad, 0);
				UIController.instance.StartCoroutine(this.showErrorMessage(2.1f));
			}
			EnemyLoader.loadEnemies();
			this.checkForSpecialItems(newLevel.LevelName);
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
			this.newItems = new Dictionary<string, Reward>();
			ForwardFiller forwardFiller = new ForwardFiller(this.gameConfig.items.randomizedLocations, newGame);
			while (!forwardFiller.Fill(seed, this.newItems))
			{
				seed++;
				this.Log("Seed invalid - trying " + seed, 0);
			}
			this.seed = seed;
			this.totalItems = this.newItems.Count;
			this.enemizer = new Enemizer(seed);
			this.enemizer.Randomize();
		}

		private IEnumerator showErrorMessage(float waitTime)
		{
			yield return new WaitForSecondsRealtime(waitTime);
			UIController.instance.ShowPopUp(this.errorOnLoad, "", 0f, false);
			this.errorOnLoad = "";
			yield break;
		}

		private bool isConfigVersionValid(string configVersion)
		{
			string version = Randomizer.getVersion();
			return version.Substring(version.IndexOf('.') + 1, 1) == configVersion.Substring(configVersion.IndexOf('.') + 1, 1);
		}

		private void checkForSpecialItems(string scene)
		{
			if (scene == "D01Z04S19")
			{
				this.giveReward("QI38", true);
				Core.Events.SetFlag("ATTRITION_ALTAR_DONE", true, false);
				this.disableAltar("22c0f081-b3a0-4310-8a40-9506d4a1315c");
				return;
			}
			if (scene == "D03Z03S16")
			{
				this.giveReward("QI39", true);
				Core.Events.SetFlag("CONTRITION_ALTAR_DONE", true, false);
				this.disableAltar("27213fd3-b05b-4157-b067-5206321cacb7");
				return;
			}
			if (scene == "D02Z03S21")
			{
				this.giveReward("QI40", true);
				Core.Events.SetFlag("COMPUNCTION_ALTAR_DONE", true, false);
				this.disableAltar("bc2b17e1-5c8c-4a90-b7c8-160eacdd538d");
			}
			this.updateShop(scene);
		}

		private void disableAltar(string id)
		{
			foreach (Interactable interactable in UnityEngine.Object.FindObjectsOfType<Interactable>())
			{
				Core.Randomizer.Log(interactable.gameObject.name + ": " + interactable.GetPersistenID(), 0);
				if (interactable.GetPersistenID() == id)
				{
					interactable.gameObject.SetActive(false);
					return;
				}
			}
		}

		public Reward getRewardFromId(string id, bool avoidDuplicate)
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
			if (avoidDuplicate && this.checkForDuplicate(id))
			{
				this.Log("Location " + id + " is already retrieved!");
				return null;
			}
			return this.newItems[id];
		}

		public bool shouldSkipCutscene(string id)
		{
			return this.gameConfig.general.skipCutscenes && FileIO.arrayContains(this.cutsceneNames, id);
		}

		public RewardInfo GetRewardInfo(Reward reward, bool upgraded)
		{
			if (reward == null)
			{
				return null;
			}
			InventoryManager inventoryManager = Core.InventoryManager;
			EntityStats stats = Core.Logic.Penitent.Stats;
			int type = reward.type;
			switch (type)
			{
			case 0:
			{
				BaseInventoryObject baseObject = inventoryManager.GetBaseObject("RB" + reward.id.ToString("00"), InventoryManager.ItemType.Bead);
				return new RewardInfo(baseObject.caption, baseObject.description, "New rosary bead obtained!", baseObject.picture);
			}
			case 1:
			{
				BaseInventoryObject baseObject2 = inventoryManager.GetBaseObject("PR" + reward.id.ToString("00"), InventoryManager.ItemType.Prayer);
				return new RewardInfo(baseObject2.caption, baseObject2.description, "New prayer obtained!", baseObject2.picture);
			}
			case 2:
			{
				BaseInventoryObject baseObject3 = inventoryManager.GetBaseObject("RE" + reward.id.ToString("00"), InventoryManager.ItemType.Relic);
				return new RewardInfo(baseObject3.caption, baseObject3.description, "New relic obtained!", baseObject3.picture);
			}
			case 3:
			{
				BaseInventoryObject baseObject4 = inventoryManager.GetBaseObject("HE" + reward.id.ToString("00"), InventoryManager.ItemType.Sword);
				return new RewardInfo(baseObject4.caption, baseObject4.description, "New sword heart obtained!", baseObject4.picture);
			}
			case 4:
			{
				BaseInventoryObject baseObject5 = inventoryManager.GetBaseObject("CO" + reward.id.ToString("00"), InventoryManager.ItemType.Collectible);
				return new RewardInfo(baseObject5.caption, baseObject5.description, "New collectible obtained!", baseObject5.picture);
			}
			case 5:
			{
				BaseInventoryObject baseObject6 = inventoryManager.GetBaseObject("QI" + reward.id.ToString("00"), InventoryManager.ItemType.Quest);
				return new RewardInfo(baseObject6.caption, baseObject6.description, "New quest item obtained!", baseObject6.picture);
			}
			case 6:
				return new RewardInfo("Cherub " + (CherubCaptorPersistentObject.CountRescuedCherubs() + (upgraded ? 1 : 0)) + "/38", "A little floating baby that you rescued from a cage.", "Cherub rescued!", this.customImages[0]);
			case 7:
				return new RewardInfo("Life Upgrade " + (stats.Life.GetUpgrades() + (upgraded ? 1 : 0)) + "/6", "An increase to your maximum health.", "Stat increased!", this.customImages[1]);
			case 8:
				return new RewardInfo("Fervour Upgrade " + (stats.Fervour.GetUpgrades() + (upgraded ? 1 : 0)) + "/6", "An increase to your maximum fervour.", "Stat increased!", this.customImages[2]);
			case 9:
				return new RewardInfo("Mea Culpa Upgrade " + (stats.MeaCulpa.GetUpgrades() + (upgraded ? 1 : 0)) + "/7", "An increase to the strength of your sword.", "Stat increased!", this.customImages[3]);
			case 10:
			{
				TearsObject tearsGenericObject = inventoryManager.TearsGenericObject;
				return new RewardInfo(tearsGenericObject.caption, "A bundle of " + reward.id + " tears.", reward.id + " tears added!", tearsGenericObject.picture);
			}
			default:
			{
				if (type != 99)
				{
					return null;
				}
				Reward level = (reward as ProgressiveReward).getLevel(upgraded);
				return this.GetRewardInfo(level, false);
			}
			}
		}

		private void updateShop(string scene)
		{
			if (scene == "D02BZ02S01" || scene == "D01BZ02S01" || scene == "D05BZ02S01")
			{
				foreach (SpriteRenderer spriteRenderer in UnityEngine.Object.FindObjectsOfType<SpriteRenderer>())
				{
					if (spriteRenderer.name == "Body" && spriteRenderer.sprite != null && "qi11rb02rb37qi58rb09rb05qi49rb12qi71".Contains(spriteRenderer.sprite.name))
					{
						RewardInfo rewardInfo = Core.Randomizer.GetRewardInfo(Core.Randomizer.getRewardFromId(spriteRenderer.sprite.name.ToUpper(), false), true);
						spriteRenderer.sprite = ((rewardInfo == null) ? null : rewardInfo.sprite);
					}
				}
			}
		}

		public void updateDialog(string id, DialogObject @object)
		{
			if (id != "DLG_2010" && id != "DLG_2029")
			{
				return;
			}
			List<string> dialogLines = @object.dialogLines;
			dialogLines.Clear();
			dialogLines.Add("This corpse is saying something custom.");
		}

		private int seed;

		private Dictionary<string, Reward> newItems;

		private bool startedInRando;

		private bool inGame;

		private Logger[] logs;

		public Reward lastReward;

		private Sprite[] customImages;

		private FileIO file;

		private int lastSlotLoaded;

		private string errorOnLoad;

		public Enemizer enemizer;

		private MainConfig fileConfig;

		public MainConfig gameConfig;

		private string[] cutsceneNames;

		private string[] cutsceneFlags;
	}
}
