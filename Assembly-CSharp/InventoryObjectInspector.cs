using System;
using Framework.Inventory;
using Framework.Managers;

[Serializable]
public class InventoryObjectInspector
{
	public BaseInventoryObject GetInvObject()
	{
		BaseInventoryObject result = null;
		switch (this.type)
		{
		case InventoryManager.ItemType.Relic:
			result = Core.InventoryManager.GetRelic(this.id);
			break;
		case InventoryManager.ItemType.Prayer:
			result = Core.InventoryManager.GetPrayer(this.id);
			break;
		case InventoryManager.ItemType.Bead:
			result = Core.InventoryManager.GetRosaryBead(this.id);
			break;
		case InventoryManager.ItemType.Quest:
			result = Core.InventoryManager.GetQuestItem(this.id);
			break;
		case InventoryManager.ItemType.Collectible:
			result = Core.InventoryManager.GetCollectibleItem(this.id);
			break;
		case InventoryManager.ItemType.Sword:
			result = Core.InventoryManager.GetSword(this.id);
			break;
		}
		return result;
	}

	public InventoryManager.ItemType type = InventoryManager.ItemType.Quest;

	public string id = string.Empty;
}
