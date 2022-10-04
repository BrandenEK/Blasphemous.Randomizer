using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint
{
	public class BsHolderManager : MonoBehaviour
	{
		private void Start()
		{
			BejeweledSaintHolder.OnHolderCollapse = (Core.SimpleEvent)Delegate.Combine(BejeweledSaintHolder.OnHolderCollapse, new Core.SimpleEvent(this.OnHolderCollapse));
		}

		private void OnHolderCollapse()
		{
			this.CollapsedHoldersAmount++;
			if (this.CollapsedHoldersAmount < this.holdersToFall)
			{
				return;
			}
			this.CollapsedHoldersAmount = 0;
			this.EnableHoldersDamageArea(false);
			if (this.OnBossCollapse != null)
			{
				this.OnBossCollapse();
			}
		}

		public void SortDamageable()
		{
		}

		public void SortRealHolder()
		{
		}

		public void SetDefaultLocalPositions()
		{
			foreach (BejeweledSaintHolder bejeweledSaintHolder in this.Holders)
			{
				bejeweledSaintHolder.SetDefaultLocalPosition();
			}
		}

		public void HealHolders()
		{
			foreach (BejeweledSaintHolder bejeweledSaintHolder in this.Holders)
			{
				bejeweledSaintHolder.Heal();
			}
		}

		public void EnableHoldersDamageArea(bool enableDamageArea)
		{
			foreach (BejeweledSaintHolder bejeweledSaintHolder in this.Holders)
			{
				bejeweledSaintHolder.EnableDamageArea(enableDamageArea);
			}
		}

		private void OnDestroy()
		{
			BejeweledSaintHolder.OnHolderCollapse = (Core.SimpleEvent)Delegate.Remove(BejeweledSaintHolder.OnHolderCollapse, new Core.SimpleEvent(this.OnHolderCollapse));
		}

		public Core.SimpleEvent OnBossCollapse;

		public int CollapsedHoldersAmount;

		public int holdersToFall = 3;

		public BejeweledSaintHolder[] Holders;
	}
}
