using Framework.Managers;
using Framework.FrameworkCore;
using Framework.Inventory;
using Gameplay.GameControllers.Entities;

namespace BlasphemousRandomizer.Structures
{
	[System.Serializable]
    public class Item
    {
		public string name;
        public int type;
		public bool progression;
		public int count;

		public int tearAmount
		{
            get
            {
				if (type != 10) return 0;
				int s = name.IndexOf('['), e = name.IndexOf(']');
				return int.Parse(name.Substring(s + 1, e - s - 1));
			}
		}

		public Item(string name, int type, bool progression, int count)
		{
			this.name = name;
			this.type = type;
			this.progression = progression;
			this.count = count;
		}

		public virtual void addToInventory()
        {
			InventoryManager inv = Core.InventoryManager;
			EntityStats stats = Core.Logic.Penitent.Stats;

			switch (type)
			{
				case 0:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(name, InventoryManager.ItemType.Bead));
					if (name == "RB203" && Main.Randomizer.gameConfig.items.startWithWheel)
						inv.SetRosaryBeadInSlot(0, "RB203");
					return;
				case 1:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(name, InventoryManager.ItemType.Prayer)); return;
				case 2:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(name, InventoryManager.ItemType.Relic)); return;
				case 3:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(name, InventoryManager.ItemType.Sword)); return;
				case 4:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(name, InventoryManager.ItemType.Collectible)); return;
				case 5:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(name, InventoryManager.ItemType.Quest)); return;
				case 6:
					Core.Events.SetFlag("RESCUED_CHERUB_" + name.Substring(2), true, false); return;
				case 7:
					stats.Life.Upgrade(); stats.Life.SetToCurrentMax(); return;
				case 8:
					stats.Fervour.Upgrade(); stats.Fervour.SetToCurrentMax(); return;
				case 9:
					stats.Strength.Upgrade(); return;
				case 10:
					stats.Purge.Current += tearAmount; return;
				case 11:
					Core.SkillManager.UnlockSkill(name, true); return;
				default:
					return;
			}
		}

		public virtual RewardInfo getRewardInfo(bool upgraded)
		{
			InventoryManager inventoryManager = Core.InventoryManager;
			EntityStats stats = Core.Logic.Penitent.Stats;

			switch (type)
			{
				case 0:
					BaseInventoryObject baseObject = inventoryManager.GetBaseObject(name, InventoryManager.ItemType.Bead);
					return new RewardInfo(baseObject.caption, baseObject.description, "New rosary bead obtained!", baseObject.picture);
				case 1:
					baseObject = inventoryManager.GetBaseObject(name, InventoryManager.ItemType.Prayer);
					return new RewardInfo(baseObject.caption, baseObject.description, "New prayer obtained!", baseObject.picture);
				case 2:
					baseObject = inventoryManager.GetBaseObject(name, InventoryManager.ItemType.Relic);
					return new RewardInfo(baseObject.caption, baseObject.description, "New relic obtained!", baseObject.picture);
				case 3:
					baseObject = inventoryManager.GetBaseObject(name, InventoryManager.ItemType.Sword);
					return new RewardInfo(baseObject.caption, baseObject.description, "New sword heart obtained!", baseObject.picture);
				case 4:
					baseObject = inventoryManager.GetBaseObject(name, InventoryManager.ItemType.Collectible);
					return new RewardInfo(baseObject.caption, baseObject.description, "New collectible obtained!", baseObject.picture);
				case 5:
					baseObject = inventoryManager.GetBaseObject(name, InventoryManager.ItemType.Quest);
					return new RewardInfo(baseObject.caption, baseObject.description, "New quest item obtained!", baseObject.picture);
				case 6:
					return new RewardInfo("Cherub " + int.Parse(name.Substring(2)) + "/38", "A little floating baby that you rescued from a cage.", "Cherub rescued!", Main.Randomizer.data.randomizerImages[0]);
				case 7:
					return new RewardInfo("Life Upgrade " + (stats.Life.GetUpgrades() + (upgraded ? 1 : 0)) + "/6", "An increase to your maximum health.", "Stat increased!", Main.Randomizer.data.randomizerImages[1]);
				case 8:
					return new RewardInfo("Fervour Upgrade " + (stats.Fervour.GetUpgrades() + (upgraded ? 1 : 0)) + "/6", "An increase to your maximum fervour.", "Stat increased!", Main.Randomizer.data.randomizerImages[2]);
				case 9:
					return new RewardInfo("Mea Culpa Upgrade " + (stats.Strength.GetUpgrades() + (upgraded ? 1 : 0)) + "/7", "An increase to the strength of your sword.", "Stat increased!", Main.Randomizer.data.randomizerImages[3]);
				case 10:
					return new RewardInfo($"Tears of Atonement ({tearAmount})", $"A bundle of {tearAmount} tears.", "Tears acquired!", inventoryManager.TearsGenericObject.picture);
				case 11:
					UnlockableSkill skill = Core.SkillManager.GetSkill(name);
					return new RewardInfo(removeCaps(skill.caption), skill.description, "Skill unlocked!", skill.smallImage);
				default:
					return new RewardInfo("Error!", "You should not see this.", "You should not see this!", null);
			}
		}

		private string removeCaps(string name)
        {
			string[] words = name.ToLower().Split(' ');
			string output = "";
			foreach (string word in words)
				output += char.ToUpper(word[0]) + word.Substring(1) + ' ';
			return output.Trim();
		}
	}
}
