using Framework.Managers;
using Framework.FrameworkCore;
using Framework.Inventory;
using Gameplay.GameControllers.Entities;
using Blasphemous.Randomizer.Notifications;

namespace Blasphemous.Randomizer.ItemRando
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

		public bool UseDefaultImageScaling => type >= 0 && type <= 5 || type == 10 || type == 11;

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
					if (id == "RB203" && Main.Randomizer.GameSettings.StartWithWheel)
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
				case 12:
					return; // Do nothing, just relies on item flag
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
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("bead"), baseObject.picture);
				case 1:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Prayer);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("prayer"), baseObject.picture);
				case 2:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Relic);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("relic"), baseObject.picture);
				case 3:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Sword);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("heart"), baseObject.picture);
				case 4:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Collectible);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("bone"), baseObject.picture);
				case 5:
					baseObject = inventoryManager.GetBaseObject(id, InventoryManager.ItemType.Quest);
					return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("quest"), baseObject.picture);
				case 6:
					return new RewardInfo($"{Main.Randomizer.LocalizationHandler.Localize("chname")} {int.Parse(id.Substring(2))}/38", Main.Randomizer.LocalizationHandler.Localize("chdesc"), Main.Randomizer.LocalizationHandler.Localize("chnot"), Main.Randomizer.data.ImageCherub);
				case 7:
					return new RewardInfo($"{Main.Randomizer.LocalizationHandler.Localize("luname")} {stats.Life.GetUpgrades() + (upgraded ? 1 : 0)}/6", Main.Randomizer.LocalizationHandler.Localize("ludesc"), Main.Randomizer.LocalizationHandler.Localize("stnot"), Main.Randomizer.data.ImageHealth);
				case 8:
					return new RewardInfo($"{Main.Randomizer.LocalizationHandler.Localize("funame")} {stats.Fervour.GetUpgrades() + (upgraded ? 1 : 0)}/6", Main.Randomizer.LocalizationHandler.Localize("fudesc"), Main.Randomizer.LocalizationHandler.Localize("stnot"), Main.Randomizer.data.ImageFervour);
				case 9:
					return new RewardInfo($"{Main.Randomizer.LocalizationHandler.Localize("suname")} {stats.Strength.GetUpgrades() + (upgraded ? 1 : 0)}/7", Main.Randomizer.LocalizationHandler.Localize("sudesc"), Main.Randomizer.LocalizationHandler.Localize("stnot"), Main.Randomizer.data.ImageSword);
				case 10:
					return new RewardInfo($"{Main.Randomizer.LocalizationHandler.Localize("trname")} ({tearAmount})", Main.Randomizer.LocalizationHandler.Localize("trdesc").Replace("*", tearAmount.ToString()), Main.Randomizer.LocalizationHandler.Localize("trnot"), inventoryManager.TearsGenericObject.picture);
				case 11:
					UnlockableSkill skill = Core.SkillManager.GetSkill(id);
					return new RewardInfo(skill.caption.Capitalize(), skill.description, Main.Randomizer.LocalizationHandler.Localize("sknot"), skill.smallImage);
				case 12:
					if (id == "Slide") return new RewardInfo(Main.Randomizer.LocalizationHandler.Localize("dshnam"), Main.Randomizer.LocalizationHandler.Localize("dshdes"), Main.Randomizer.LocalizationHandler.Localize("ablnot"), Main.Randomizer.data.ImageDash);
					if (id == "WallClimb") return new RewardInfo(Main.Randomizer.LocalizationHandler.Localize("wclnam"), Main.Randomizer.LocalizationHandler.Localize("wcldes"), Main.Randomizer.LocalizationHandler.Localize("ablnot"), Main.Randomizer.data.ImageWallClimb);
					return new RewardInfo("Error!", "You should not see this.", "You should not see this!", null);
				default:
					return new RewardInfo("Error!", "You should not see this.", "You should not see this!", null);
			}
		}
	}
}
