using Framework.Managers;
using Framework.FrameworkCore;
using Framework.Inventory;
using Gameplay.GameControllers.Entities;
using BlasphemousRandomizer.Notifications;

namespace BlasphemousRandomizer.ItemRando
{
	[System.Serializable]
    public class Item
    {
		public string id;
		public string name;
		public string hint;
        public int type;
		public bool progression;
		public int count;

		public int tearAmount
		{
            get
            {
				if (type != 10) return 0;
				int s = id.IndexOf('['), e = id.IndexOf(']');
				return int.Parse(id.Substring(s + 1, e - s - 1));
			}
		}

		public Item(string id, string name, string hint, int type, bool progression, int count)
		{
			this.id = id;
			this.name = name;
			this.hint = hint;
			this.type = type;
			this.progression = progression;
			this.count = count;
		}

		public virtual void addToInventory()
        {
			InventoryManager inv = Core.InventoryManager;
			EntityStats stats = Core.Logic.Penitent.Stats;
			Core.Events.SetFlag("ITEM_" + id, true, false);

			switch (type)
			{
				case 0:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(id, InventoryManager.ItemType.Bead));
					if (id == "RB203" && Main.Randomizer.gameConfig.StartWithWheel)
						inv.SetRosaryBeadInSlot(0, "RB203");
					return;
				case 1:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(id, InventoryManager.ItemType.Prayer)); return;
				case 2:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(id, InventoryManager.ItemType.Relic)); return;
				case 3:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(id, InventoryManager.ItemType.Sword)); return;
				case 4:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(id, InventoryManager.ItemType.Collectible)); return;
				case 5:
					inv.AddBaseObjectOrTears(inv.GetBaseObject(id, InventoryManager.ItemType.Quest)); return;
				case 6:
					Core.Events.SetFlag("RESCUED_CHERUB_" + id.Substring(2), true, false); return;
				case 7:
					stats.Life.Upgrade(); stats.Life.SetToCurrentMax(); return;
				case 8:
					stats.Fervour.Upgrade(); stats.Fervour.SetToCurrentMax(); return;
				case 9:
					stats.Strength.Upgrade(); return;
				case 10:
					stats.Purge.Current += tearAmount; return;
				case 11:
					Core.SkillManager.UnlockSkill(id, true); return;
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
					BaseInventoryObject baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Bead);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.Localize("bead"), baseObject.picture);
				case 1:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Prayer);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.Localize("prayer"), baseObject.picture);
				case 2:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Relic);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.Localize("relic"), baseObject.picture);
				case 3:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Sword);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.Localize("heart"), baseObject.picture);
				case 4:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Collectible);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.Localize("bone"), baseObject.picture);
				case 5:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Quest);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.Localize("quest"), baseObject.picture);
				case 6:
					return new RewardInfo($"{Main.Randomizer.Localize("chname")} {int.Parse(id.Substring(2))}/38", Main.Randomizer.Localize("chdesc"), Main.Randomizer.Localize("chnot"), Main.Randomizer.data.randomizerImages[0]);
				case 7:
					return new RewardInfo($"{Main.Randomizer.Localize("luname")} {stats.Life.GetUpgrades() + (upgraded ? 1 : 0)}/6", Main.Randomizer.Localize("ludesc"), Main.Randomizer.Localize("stnot"), Main.Randomizer.data.randomizerImages[1]);
				case 8:
					return new RewardInfo($"{Main.Randomizer.Localize("funame")} {stats.Fervour.GetUpgrades() + (upgraded ? 1 : 0)}/6", Main.Randomizer.Localize("fudesc"), Main.Randomizer.Localize("stnot"), Main.Randomizer.data.randomizerImages[2]);
				case 9:
					return new RewardInfo($"{Main.Randomizer.Localize("suname")} {stats.Strength.GetUpgrades() + (upgraded ? 1 : 0)}/7", Main.Randomizer.Localize("sudesc"), Main.Randomizer.Localize("stnot"), Main.Randomizer.data.randomizerImages[3]);
				case 10:
					return new RewardInfo($"{Main.Randomizer.Localize("trname")} ({tearAmount})", Main.Randomizer.Localize("trdesc").Replace("*", tearAmount.ToString()), Main.Randomizer.Localize("trnot"), inventoryManager.TearsGenericObject.picture);
				case 11:
					UnlockableSkill skill = Core.SkillManager.GetSkill(id);
					return new RewardInfo(removeCaps(skill.caption), skill.description, Main.Randomizer.Localize("sknot"), skill.smallImage);
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
