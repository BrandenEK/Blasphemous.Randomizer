using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;

namespace Framework.Inventory
{
	public class PrayerAlliedCherubEffect : ObjectEffect_Stat
	{
		protected override bool OnApplyEffect()
		{
			this._owner = Core.Logic.Penitent;
			PrayerUse componentInChildren = this._owner.GetComponentInChildren<PrayerUse>();
			if (!componentInChildren)
			{
				return base.OnApplyEffect();
			}
			AlliedCherubPrayer cherubPrayer = componentInChildren.cherubPrayer;
			if (cherubPrayer)
			{
				cherubPrayer.InstantiateCherubs();
			}
			return base.OnApplyEffect();
		}

		protected override void OnRemoveEffect()
		{
			this._owner = Core.Logic.Penitent;
			PrayerUse componentInChildren = this._owner.GetComponentInChildren<PrayerUse>();
			if (!componentInChildren)
			{
				base.OnRemoveEffect();
			}
			AlliedCherubPrayer cherubPrayer = componentInChildren.cherubPrayer;
			if (cherubPrayer)
			{
				cherubPrayer.DisposeCherubs();
			}
			base.OnRemoveEffect();
		}

		private Penitent _owner;
	}
}
