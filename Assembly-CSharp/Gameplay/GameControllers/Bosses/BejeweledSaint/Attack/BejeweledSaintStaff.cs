using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.Attack
{
	public class BejeweledSaintStaff : Weapon
	{
		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
			if (BejeweledSaintStaff.OnSucceedHit != null)
			{
				BejeweledSaintStaff.OnSucceedHit();
			}
		}

		public static Core.SimpleEvent OnSucceedHit;
	}
}
