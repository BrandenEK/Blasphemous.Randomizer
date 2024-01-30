using Blasphemous.Randomizer.Notifications;
using Framework.Managers;

namespace Blasphemous.Randomizer.Items;

public class ProgressiveItem : Item
{
    public string[] Items { get; set; }
    public bool RemovePrevious { get; set; }

    public override void AddToInventory()
    {
        Item itemToAdd = GetItemLevel(true);
        itemToAdd.AddToInventory();
        if (RemovePrevious)
            RemoveItem();
    }

    public override RewardInfo GetRewardInfo(bool upgraded)
    {
        return GetItemLevel(upgraded).GetRewardInfo(false);
    }

    public Item GetItemLevel(bool upgraded)
    {
        int level = GetCurrentLevel() + (upgraded ? 1 : 0);
        if (level < 0 || level >= Items.Length)
        {
            Main.Randomizer.LogError("Invalid tier of progressive item!");
            if (level < 0)
                level = 0;
            else if (level >= Items.Length)
                level = Items.Length - 1;
        }

        // Change to search in dictionary.  (Would have to make cherubs a regular item)
        return new Item()
        {
            Id = Items[level],
            Name = "",
            Hint = "",
            Type = Type,
            Progrssion = false,
            Count = 0
        };
    }

    private int GetCurrentLevel()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (!Core.Events.GetFlag("ITEM_" + Items[i]))
            {
                Main.Randomizer.Log("Current progressive tier: " + (i - 1));
                return i - 1;
            }
        }
        Main.Randomizer.Log("Current progressive tier: " + (Items.Length - 1));
        return Items.Length - 1;
    }

    private void RemoveItem()
    {
        int level = GetCurrentLevel() - 1;
        if (level >= 0 && level < Items.Length)
        {
            Main.Randomizer.Log("Removing item: " + Items[level]);
            if (Type == 5)
                Core.InventoryManager.RemoveQuestItem(Items[level]);
            else if (Type == 0)
                Core.InventoryManager.RemoveRosaryBead(Items[level]);
            else
                Main.Randomizer.LogDisplay($"Item type {Type} can not be removed!");
        }
    }
}
