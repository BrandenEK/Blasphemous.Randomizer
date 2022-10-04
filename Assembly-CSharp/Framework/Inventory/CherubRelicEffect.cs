using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;

namespace Framework.Inventory
{
	public class CherubRelicEffect : RelicEffect
	{
		public override void OnEquipEffect()
		{
			base.OnEquipEffect();
			this.SetTrapDamage(true);
		}

		public override void OnUnEquipEffect()
		{
			base.OnUnEquipEffect();
			this.SetTrapDamage(false);
		}

		private void SetTrapDamage(bool trapDamage)
		{
			Penitent penitent = Core.Logic.Penitent;
			if (penitent != null)
			{
				CheckTrap componentInChildren = penitent.GetComponentInChildren<CheckTrap>();
				if (componentInChildren)
				{
					componentInChildren.AvoidTrapDamage = trapDamage;
				}
			}
		}
	}
}
