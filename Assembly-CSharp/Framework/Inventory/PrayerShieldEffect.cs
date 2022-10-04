using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;

namespace Framework.Inventory
{
	public class PrayerShieldEffect : ObjectEffect_Stat
	{
		protected override bool OnApplyEffect()
		{
			this._owner = Core.Logic.Penitent;
			ShieldSystemPrayer shieldPrayer = this._owner.GetComponentInChildren<PrayerUse>().shieldPrayer;
			shieldPrayer.InstantiateShield();
			return base.OnApplyEffect();
		}

		protected override void OnRemoveEffect()
		{
			this._owner = Core.Logic.Penitent;
			PrayerUse componentInChildren = this._owner.GetComponentInChildren<PrayerUse>();
			if (componentInChildren && componentInChildren.shieldPrayer)
			{
				componentInChildren.shieldPrayer.DisposeShield();
			}
			base.OnRemoveEffect();
		}

		private Penitent _owner;
	}
}
