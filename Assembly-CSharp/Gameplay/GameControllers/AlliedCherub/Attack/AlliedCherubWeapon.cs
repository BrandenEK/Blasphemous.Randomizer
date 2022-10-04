using System;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.AlliedCherub.Attack
{
	public class AlliedCherubWeapon : Weapon
	{
		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
			AlliedCherub alliedCherub = (AlliedCherub)this.WeaponOwner;
			alliedCherub.Store();
		}
	}
}
