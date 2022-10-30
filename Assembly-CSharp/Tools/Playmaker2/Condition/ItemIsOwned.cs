using System;
using System.Collections.Generic;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the chosen item is available on player inventory.")]
	public class ItemIsOwned : InventoryBase
	{
		public override bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot)
		{
			bool result = false;
			Core.Randomizer.Log("Check item: " + objectIdStting, 0);
			string levelName = Core.LevelManager.currentLevel.LevelName;
			if (ItemIsOwned.itemRedirects.ContainsKey(levelName) && objectIdStting == ItemIsOwned.itemRedirects[levelName])
			{
				return Core.Events.GetFlag("LOCATION_" + objectIdStting);
			}
			if (levelName == "D02Z01S01" && objectIdStting == "QI59")
			{
				return Core.Events.GetFlag("LOCATION_QI59");
			}
			if (levelName == "D01Z02S05" && objectIdStting == "QI66")
			{
				return true;
			}
			if (levelName == "D01Z02S02" && objectIdStting == "QI66")
			{
				return false;
			}
			if (levelName == "D01Z01S07" && (objectIdStting == "QI38" || objectIdStting == "QI39" || objectIdStting == "QI40"))
			{
				return Core.Events.GetFlag("LOCATION_QI31");
			}
			if (levelName == "D01BZ02S01" && (objectIdStting == "QI58" || objectIdStting == "RB05" || objectIdStting == "RB09"))
			{
				return Core.Events.GetFlag("LOCATION_" + objectIdStting);
			}
			if (levelName == "D02BZ02S01" && (objectIdStting == "QI11" || objectIdStting == "RB02" || objectIdStting == "RB37"))
			{
				return Core.Events.GetFlag("LOCATION_" + objectIdStting);
			}
			if (levelName == "D05BZ02S01" && (objectIdStting == "QI49" || objectIdStting == "RB12" || objectIdStting == "QI71"))
			{
				return Core.Events.GetFlag("LOCATION_" + objectIdStting);
			}
			if (levelName == "D01Z04S08" && "RB17RB18RB19".Contains(objectIdStting))
			{
				return Core.Events.GetFlag("LOCATION_RB17");
			}
			if (levelName == "D02Z03S06" || levelName == "D05Z01S02")
			{
				if (objectIdStting == "RB17")
				{
					return !Core.Events.GetFlag("LOCATION_RB18") && (Core.InventoryManager.IsRosaryBeadOwned("RB17") || Core.InventoryManager.IsRosaryBeadOwned("RB18") || Core.InventoryManager.IsRosaryBeadOwned("RB19"));
				}
				if (objectIdStting == "RB18")
				{
					return Core.Events.GetFlag("LOCATION_RB18");
				}
			}
			if (levelName == "D02Z03S17" && "RB24RB25RB26".Contains(objectIdStting))
			{
				return Core.Events.GetFlag("LOCATION_RB24");
			}
			if (levelName == "D17Z01S04" || levelName == "D01Z04S16")
			{
				if (objectIdStting == "RB24")
				{
					return !Core.Events.GetFlag("LOCATION_RB25") && (Core.InventoryManager.IsRosaryBeadOwned("RB24") || Core.InventoryManager.IsRosaryBeadOwned("RB25") || Core.InventoryManager.IsRosaryBeadOwned("RB26"));
				}
				if (objectIdStting == "RB25")
				{
					return Core.Events.GetFlag("LOCATION_RB25");
				}
			}
			switch (objType)
			{
			case InventoryManager.ItemType.Relic:
				result = Core.InventoryManager.IsRelicOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Prayer:
				result = Core.InventoryManager.IsPrayerOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Bead:
				result = Core.InventoryManager.IsRosaryBeadOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Quest:
				result = Core.InventoryManager.IsQuestItemOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Collectible:
				result = Core.InventoryManager.IsCollectibleItemOwned(objectIdStting);
				break;
			case InventoryManager.ItemType.Sword:
				result = Core.InventoryManager.IsSwordOwned(objectIdStting);
				break;
			}
			return result;
		}

		private static Dictionary<string, string> itemRedirects = new Dictionary<string, string>
		{
			{
				"D01Z04S18",
				"QI38"
			},
			{
				"D03Z03S15",
				"QI39"
			},
			{
				"D02Z03S20",
				"QI40"
			},
			{
				"D04BZ03S01",
				"QI60"
			},
			{
				"D02Z03S19",
				"QI61"
			},
			{
				"D05Z01S15",
				"QI62"
			},
			{
				"D08Z01S01",
				"PR09"
			},
			{
				"D01BZ08S01",
				"QI201"
			},
			{
				"D04BZ02S01",
				"QI54"
			},
			{
				"D02Z01S01",
				"QI68"
			},
			{
				"D02Z01S04",
				"QI68"
			}
		};
	}
}
