using System;
using Framework.Managers;
using UnityEngine;

namespace Framework.Inventory
{
	public class Prayer : EquipableInventoryObject
	{
		public Prayer()
		{
			this.ResetDecipher();
		}

		public override InventoryManager.ItemType GetItemType()
		{
			return InventoryManager.ItemType.Prayer;
		}

		public int CurrentDecipher { get; private set; }

		public override bool IsDeciphered()
		{
			return true;
		}

		public override bool IsDecipherable()
		{
			return true;
		}

		public float EffectTime { get; private set; }

		public void Awake()
		{
			foreach (ObjectEffect objectEffect in base.GetComponents<ObjectEffect>())
			{
				if (objectEffect.effectType == ObjectEffect.EffectType.OnUse && objectEffect.LimitTime && objectEffect.EffectTime > this.EffectTime)
				{
					this.EffectTime = objectEffect.EffectTime;
				}
			}
			Debug.Log(string.Concat(new object[]
			{
				"Prayer ",
				this.id,
				"  Time:",
				this.EffectTime
			}));
		}

		public void ResetDecipher()
		{
			this.CurrentDecipher = 0;
		}

		public void AddDecipher(int number = 1)
		{
			Core.Metrics.CustomEvent("DECIPHER_FERVOUR", string.Empty, (float)number);
			this.CurrentDecipher += number;
			if (this.CurrentDecipher > this.decipherMax)
			{
				this.CurrentDecipher = this.decipherMax;
			}
		}

		public int decipherMax;

		public int fervourNeeded = 20;

		public Prayer.PrayerType prayerType;

		public static class Id
		{
			public const string DistressingSaeta = "PR04";
		}

		public enum PrayerType
		{
			Laments,
			Thanksgiving,
			Hymn
		}
	}
}
