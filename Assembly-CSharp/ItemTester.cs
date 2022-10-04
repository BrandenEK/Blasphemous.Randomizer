using System;
using Framework.Inventory;
using Framework.Managers;
using UnityEngine;

public class ItemTester : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Penitent"))
		{
			Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax();
			Prayer prayer = Core.InventoryManager.GetPrayer(this.AddPrayer);
			Core.InventoryManager.SetPrayerInSlot(0, prayer);
			Core.InventoryManager.RemovePrayer(this.RemovePrayer);
			if (prayer != null)
			{
				prayer.AddDecipher(prayer.decipherMax);
			}
			Relic relic = Core.InventoryManager.GetRelic(this.AddRelic);
			Core.InventoryManager.SetRelicInSlot(0, relic);
			Core.InventoryManager.RemoveRelic(this.RemoveRelic);
		}
	}

	public string AddPrayer;

	public string AddRelic;

	public string RemovePrayer;

	public string RemoveRelic;
}
