using Blasphemous.Randomizer.Notifications;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Entities;

namespace Blasphemous.Randomizer.Items;

public class Item
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Hint { get; set; }
    public int Type { get; set; }
    public bool Progrssion { get; set; }
    public int Count { get; set; }

    public int TearAmount
    {
        get
        {
            if (Type != 10)
                return 0;

            int s = Id.IndexOf('['), e = Id.IndexOf(']');
            return int.Parse(Id.Substring(s + 1, e - s - 1));
        }
    }

    public bool UseDefaultImageScaling => Type >= 0 && Type <= 5 || Type == 10 || Type == 11;

    public virtual void AddToInventory()
    {
        InventoryManager inv = Core.InventoryManager;
        EntityStats stats = Core.Logic.Penitent.Stats;
        Core.Events.SetFlag("ITEM_" + Id, true, false);

        switch (Type)
        {
            case 0:
                inv.AddBaseObjectOrTears(inv.GetBaseObject(Id, InventoryManager.ItemType.Bead));
                if (Id == "RB203" && Main.Randomizer.CurrentSettings.StartWithWheel)
                    inv.SetRosaryBeadInSlot(0, "RB203");
                return;
            case 1:
                inv.AddBaseObjectOrTears(inv.GetBaseObject(Id, InventoryManager.ItemType.Prayer)); return;
            case 2:
                inv.AddBaseObjectOrTears(inv.GetBaseObject(Id, InventoryManager.ItemType.Relic)); return;
            case 3:
                inv.AddBaseObjectOrTears(inv.GetBaseObject(Id, InventoryManager.ItemType.Sword)); return;
            case 4:
                inv.AddBaseObjectOrTears(inv.GetBaseObject(Id, InventoryManager.ItemType.Collectible)); return;
            case 5:
                inv.AddBaseObjectOrTears(inv.GetBaseObject(Id, InventoryManager.ItemType.Quest)); return;
            case 6:
                Core.Events.SetFlag("RESCUED_CHERUB_" + Id.Substring(2), true, false); return;
            case 7:
                stats.Life.Upgrade(); stats.Life.SetToCurrentMax(); return;
            case 8:
                stats.Fervour.Upgrade(); stats.Fervour.SetToCurrentMax(); return;
            case 9:
                stats.Strength.Upgrade(); return;
            case 10:
                stats.Purge.Current += TearAmount; return;
            case 11:
                Core.SkillManager.UnlockSkill(Id, true); return;
            case 12:
                return; // Do nothing, just relies on item flag
            default:
                return;
        }
    }

    public virtual RewardInfo GetRewardInfo(bool upgraded)
    {
        InventoryManager inventoryManager = Core.InventoryManager;
        EntityStats stats = Core.Logic.Penitent.Stats;

        switch (Type)
        {
            case 0:
                BaseInventoryObject baseObject = inventoryManager.GetBaseObject(Id, InventoryManager.ItemType.Bead);
                return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("bead"), baseObject.picture);
            case 1:
                baseObject = inventoryManager.GetBaseObject(Id, InventoryManager.ItemType.Prayer);
                return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("prayer"), baseObject.picture);
            case 2:
                baseObject = inventoryManager.GetBaseObject(Id, InventoryManager.ItemType.Relic);
                return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("relic"), baseObject.picture);
            case 3:
                baseObject = inventoryManager.GetBaseObject(Id, InventoryManager.ItemType.Sword);
                return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("heart"), baseObject.picture);
            case 4:
                baseObject = inventoryManager.GetBaseObject(Id, InventoryManager.ItemType.Collectible);
                return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("bone"), baseObject.picture);
            case 5:
                baseObject = inventoryManager.GetBaseObject(Id, InventoryManager.ItemType.Quest);
                return new RewardInfo(baseObject.caption, baseObject.description, Main.Randomizer.LocalizationHandler.Localize("quest"), baseObject.picture);
            case 6:
                return new RewardInfo(
                    $"{Main.Randomizer.LocalizationHandler.Localize("chname")} {int.Parse(Id.Substring(2))}/38",
                    Main.Randomizer.LocalizationHandler.Localize("chdesc"),
                    Main.Randomizer.LocalizationHandler.Localize("chnot"),
                    Main.Randomizer.DataHandler.ImageCherub);
            case 7:
                return new RewardInfo(
                    $"{Main.Randomizer.LocalizationHandler.Localize("luname")} {stats.Life.GetUpgrades() + (upgraded ? 1 : 0)}/6",
                    Main.Randomizer.LocalizationHandler.Localize("ludesc"),
                    Main.Randomizer.LocalizationHandler.Localize("stnot"),
                    Main.Randomizer.DataHandler.ImageHealth);
            case 8:
                return new RewardInfo(
                    $"{Main.Randomizer.LocalizationHandler.Localize("funame")} {stats.Fervour.GetUpgrades() + (upgraded ? 1 : 0)}/6",
                    Main.Randomizer.LocalizationHandler.Localize("fudesc"),
                    Main.Randomizer.LocalizationHandler.Localize("stnot"),
                    Main.Randomizer.DataHandler.ImageFervour);
            case 9:
                return new RewardInfo(
                    $"{Main.Randomizer.LocalizationHandler.Localize("suname")} {stats.Strength.GetUpgrades() + (upgraded ? 1 : 0)}/7",
                    Main.Randomizer.LocalizationHandler.Localize("sudesc"),
                    Main.Randomizer.LocalizationHandler.Localize("stnot"),
                    Main.Randomizer.DataHandler.ImageSword);
            case 10:
                return new RewardInfo(
                    $"{Main.Randomizer.LocalizationHandler.Localize("trname")} ({TearAmount})",
                    Main.Randomizer.LocalizationHandler.Localize("trdesc").Replace("*", TearAmount.ToString()),
                    Main.Randomizer.LocalizationHandler.Localize("trnot"),
                    inventoryManager.TearsGenericObject.picture);
            case 11:
                UnlockableSkill skill = Core.SkillManager.GetSkill(Id);
                return new RewardInfo(
                    skill.caption.Capitalize(),
                    skill.description,
                    Main.Randomizer.LocalizationHandler.Localize("sknot"),
                    skill.smallImage);
            case 12:
                if (Id == "Slide") 
                    return new RewardInfo(
                        Main.Randomizer.LocalizationHandler.Localize("dshnam"),
                        Main.Randomizer.LocalizationHandler.Localize("dshdes"),
                        Main.Randomizer.LocalizationHandler.Localize("ablnot"),
                        Main.Randomizer.DataHandler.ImageDash);
                if (Id == "WallClimb")
                    return new RewardInfo(
                        Main.Randomizer.LocalizationHandler.Localize("wclnam"),
                        Main.Randomizer.LocalizationHandler.Localize("wcldes"),
                        Main.Randomizer.LocalizationHandler.Localize("ablnot"), 
                        Main.Randomizer.DataHandler.ImageWallClimb);
                return new RewardInfo("Error!", "You should not see this.", "You should not see this!", null);
            default:
                return new RewardInfo("Error!", "You should not see this.", "You should not see this!", null);
        }
    }
}
