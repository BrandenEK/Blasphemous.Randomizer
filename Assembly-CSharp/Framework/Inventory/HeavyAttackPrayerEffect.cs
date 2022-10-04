using System;
using Framework.Managers;

namespace Framework.Inventory
{
	public class HeavyAttackPrayerEffect : ObjectEffect_Stat
	{
		protected override bool OnApplyEffect()
		{
			Core.Logic.Penitent.PenitentAttack.IsHeavyAttackPrayerEquipped = true;
			this.prevLevel = Core.Logic.Penitent.PenitentAttack.CurrentLevel;
			Core.Logic.Penitent.PenitentAttack.CurrentLevel = 2;
			return base.OnApplyEffect();
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
			Core.Logic.Penitent.PenitentAttack.IsHeavyAttackPrayerEquipped = false;
			Core.Logic.Penitent.PenitentAttack.CurrentLevel = this.prevLevel;
		}

		private int prevLevel;
	}
}
