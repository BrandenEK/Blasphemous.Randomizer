using System;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Bosses.PontiffHusk
{
	public class PontiffHuskBossWeapon : Weapon
	{
		public override void Attack(Hit weaponHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weaponHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}
	}
}
