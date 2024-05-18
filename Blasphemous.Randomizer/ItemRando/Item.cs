using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

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

		/// <summary>
		/// The display name of the item
		/// </summary>
		public virtual string GetName(bool upgraded)
		{
            InventoryManager inventory = Core.InventoryManager;
            EntityStats stats = Core.Logic.Penitent.Stats;

			return type switch
			{
				0 => inventory.GetBaseObject(id, InventoryManager.ItemType.Bead).caption,
                1 => inventory.GetBaseObject(id, InventoryManager.ItemType.Prayer).caption,
                2 => inventory.GetBaseObject(id, InventoryManager.ItemType.Relic).caption,
                3 => inventory.GetBaseObject(id, InventoryManager.ItemType.Sword).caption,
                4 => inventory.GetBaseObject(id, InventoryManager.ItemType.Collectible).caption,
                5 => inventory.GetBaseObject(id, InventoryManager.ItemType.Quest).caption,
				6 => $"{Main.Randomizer.LocalizationHandler.Localize("chname")} {int.Parse(id.Substring(2))}/38",
                7 => $"{Main.Randomizer.LocalizationHandler.Localize("luname")} {stats.Life.GetUpgrades() + (upgraded ? 1 : 0)}/6",
                8 => $"{Main.Randomizer.LocalizationHandler.Localize("funame")} {stats.Fervour.GetUpgrades() + (upgraded ? 1 : 0)}/6",
                9 => $"{Main.Randomizer.LocalizationHandler.Localize("suname")} {stats.Strength.GetUpgrades() + (upgraded ? 1 : 0)}/7",
                10 => $"{Main.Randomizer.LocalizationHandler.Localize("trname")} ({tearAmount})",
				11 => Core.SkillManager.GetSkill(id).caption.Capitalize(),
				12 when id == "Slide" => Main.Randomizer.LocalizationHandler.Localize("dshnam"),
				12 when id == "WallClimb" => Main.Randomizer.LocalizationHandler.Localize("wclnam"),
				_ => "Error!"
            };
        }

        /// <summary>
        /// The display description of the item
        /// </summary>
        public virtual string GetDescription(bool upgraded)
		{
            InventoryManager inventory = Core.InventoryManager;

            return type switch
            {
                0 => inventory.GetBaseObject(id, InventoryManager.ItemType.Bead).description,
                1 => inventory.GetBaseObject(id, InventoryManager.ItemType.Prayer).description,
                2 => inventory.GetBaseObject(id, InventoryManager.ItemType.Relic).description,
                3 => inventory.GetBaseObject(id, InventoryManager.ItemType.Sword).description,
                4 => inventory.GetBaseObject(id, InventoryManager.ItemType.Collectible).description,
                5 => inventory.GetBaseObject(id, InventoryManager.ItemType.Quest).description,
                6 => Main.Randomizer.LocalizationHandler.Localize("chdesc"),
                7 => Main.Randomizer.LocalizationHandler.Localize("ludesc"),
                8 => Main.Randomizer.LocalizationHandler.Localize("fudesc"),
                9 => Main.Randomizer.LocalizationHandler.Localize("sudesc"),
                10 => Main.Randomizer.LocalizationHandler.Localize("trdesc").Replace("*", tearAmount.ToString()),
                11 => Core.SkillManager.GetSkill(id).description,
                12 when id == "Slide" => Main.Randomizer.LocalizationHandler.Localize("dshdes"),
                12 when id == "WallClimb" => Main.Randomizer.LocalizationHandler.Localize("wcldes"),
                _ => "Error!"
            };
        }

        /// <summary>
        /// The display image of the item
        /// </summary>
        public virtual Sprite GetImage(bool upgraded)
		{
            InventoryManager inventory = Core.InventoryManager;

            return type switch
            {
                0 => inventory.GetBaseObject(id, InventoryManager.ItemType.Bead).picture,
                1 => inventory.GetBaseObject(id, InventoryManager.ItemType.Prayer).picture,
                2 => inventory.GetBaseObject(id, InventoryManager.ItemType.Relic).picture,
                3 => inventory.GetBaseObject(id, InventoryManager.ItemType.Sword).picture,
                4 => inventory.GetBaseObject(id, InventoryManager.ItemType.Collectible).picture,
                5 => inventory.GetBaseObject(id, InventoryManager.ItemType.Quest).picture,
                6 => Main.Randomizer.data.ImageCherub,
                7 => Main.Randomizer.data.ImageHealth,
                8 => Main.Randomizer.data.ImageFervour,
                9 => Main.Randomizer.data.ImageSword,
                10 => inventory.TearsGenericObject.picture,
                11 => Core.SkillManager.GetSkill(id).smallImage,
                12 when id == "Slide" => Main.Randomizer.data.ImageDash,
                12 when id == "WallClimb" => Main.Randomizer.data.ImageWallClimb,
                _ => null
            };
        }
	}
}
