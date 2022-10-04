using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes.Logic;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.UI.Widgets;
using UnityEngine;

namespace Framework.Util.CompletionCommandHelper
{
	[CreateAssetMenu(fileName = "Completion List", menuName = "Blasphemous/Completion List", order = 0)]
	public class CompletionAssets : ScriptableObject
	{
		public IEnumerable<string> UnlockBaseGame()
		{
			Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.NEW_GAME);
			this.ClearNGPlusData();
			CompletionAssets.AddBaseObjects(this.relics);
			yield return "Added Relics";
			CompletionAssets.AddBaseObjects(this.beads);
			yield return "Added Beads";
			CompletionAssets.AddBaseObjects(this.prayers);
			yield return "Added Prayers";
			CompletionAssets.AddBaseObjects(this.swordHearts);
			yield return "Added Sword Hearts";
			CompletionAssets.AddBaseObjects(this.collectibles);
			yield return "Added Collectibles";
			this.AddTeleports();
			yield return "Added Teleports";
			this.RevealFullMap();
			yield return "Added Full Map";
			CompletionAssets.UpgradeStat(EntityStats.StatsTypes.Fervour, 6);
			yield return "Upgraded Fervour";
			CompletionAssets.UpgradeStat(EntityStats.StatsTypes.Life, 6);
			yield return "Upgraded Health";
			CompletionAssets.UpgradeStat(EntityStats.StatsTypes.MeaCulpa, 7);
			yield return "Upgraded Mea Culpa";
			this.SetFlag(this.endingAFlag, true);
			yield return "Set Ending A";
			this.SetFlags(this.bossDeadFlags, true);
			yield return "Added Dead Bosses";
			this.SetFlags(this.arenaFlags, true);
			yield return "Added Arenas";
			yield break;
		}

		private void ClearNGPlusData()
		{
			this.SetFlags(this.amanecidasFlags, false);
		}

		public IEnumerable<string> UnlockNGPlus()
		{
			Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS);
			this.RevealFullMap();
			yield return "Revealed NG+ map";
			this.SetFlags(this.amanecidasFlags, true);
			yield return "Added Amanecidas bosses defeated";
			Core.PenitenceManager.MarkPenitenceAsCompleted("PE01");
			yield return "Added Penitence PE01";
			Core.PenitenceManager.MarkPenitenceAsCompleted("PE02");
			yield return "Added Penitence PE02";
			Core.PenitenceManager.MarkPenitenceAsCompleted("PE03");
			yield return "Added Penitence PE03";
			yield break;
		}

		private void SetFlags(IEnumerable<string> flagList, bool v = true)
		{
			foreach (string flag in flagList)
			{
				this.SetFlag(flag, v);
			}
		}

		private void SetFlag(string flag, bool v = true)
		{
			if (!string.IsNullOrEmpty(flag))
			{
				this.RunCommand(string.Format("flag {0} {1}", (!v) ? "clear" : "set", flag), 1);
			}
		}

		private void RevealFullMap()
		{
			this.RunCommand("map reveal all", 1);
		}

		private void AddTeleports()
		{
			foreach (TeleportDestination teleportDestination in this.teleports)
			{
				this.RunCommand(string.Format("teleport unlock {0}", teleportDestination.id), 1);
			}
		}

		private void RunCommand(string command, int times = 1)
		{
			for (int i = 0; i < times; i++)
			{
				ConsoleWidget.Instance.ProcessCommand(command);
			}
		}

		private static void AddBaseObjects(IEnumerable<BaseInventoryObject> objects)
		{
			foreach (BaseInventoryObject baseInventoryObject in objects)
			{
				if (baseInventoryObject)
				{
					if (!CompletionAssets.IsOwned(baseInventoryObject))
					{
						Core.InventoryManager.AddBaseObject(baseInventoryObject);
						ConsoleWidget.Instance.WriteFormat("Added object: {0}", new object[]
						{
							baseInventoryObject.id
						});
					}
					else
					{
						ConsoleWidget.Instance.WriteFormat("Object {0} already owned, ignoring", new object[]
						{
							baseInventoryObject.id
						});
					}
				}
			}
		}

		private static void UpgradeStat(EntityStats.StatsTypes statType, int amount)
		{
			Framework.FrameworkCore.Attributes.Logic.Attribute byType = Core.Logic.Penitent.Stats.GetByType(statType);
			byType.ResetUpgrades();
			for (int i = 0; i < amount; i++)
			{
				byType.Upgrade();
			}
		}

		private static bool IsOwned(BaseInventoryObject o)
		{
			bool result;
			switch (o.GetItemType())
			{
			case InventoryManager.ItemType.Relic:
				result = Core.InventoryManager.IsRelicOwned(o.id);
				break;
			case InventoryManager.ItemType.Prayer:
				result = Core.InventoryManager.IsPrayerOwned(o.id);
				break;
			case InventoryManager.ItemType.Bead:
				result = Core.InventoryManager.IsRosaryBeadOwned(o.id);
				break;
			case InventoryManager.ItemType.Quest:
				result = Core.InventoryManager.IsQuestItemOwned(o.id);
				break;
			case InventoryManager.ItemType.Collectible:
				result = Core.InventoryManager.IsCollectibleItemOwned(o.id);
				break;
			case InventoryManager.ItemType.Sword:
				result = Core.InventoryManager.IsSwordOwned(o.id);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		public Prayer[] prayers;

		public Relic[] relics;

		public Sword[] swordHearts;

		public Framework.Inventory.CollectibleItem[] collectibles;

		public RosaryBead[] beads;

		public TeleportDestination[] teleports;

		public string[] bossDeadFlags;

		public string[] arenaFlags;

		public string endingAFlag = "D07Z01S03_ENDING_A";

		public string[] amanecidasFlags = new string[]
		{
			"SANTOS_AMANECIDA_AXE_DEFEATED",
			"SANTOS_AMANECIDA_BOW_DEFEATED",
			"SANTOS_AMANECIDA_FACCATA_DEFEATED",
			"SANTOS_AMANECIDA_LANCE_DEFEATED"
		};
	}
}
