using System;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Enemies.ChasingHead.Attack
{
	public class ChasingHeadWeapon : Weapon
	{
		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
			this.DisableDamageArea();
		}

		private void DisableDamageArea()
		{
			ChasingHead chasingHead = (ChasingHead)this.WeaponOwner;
			chasingHead.DamageArea.DamageAreaCollider.enabled = false;
		}
	}
}
