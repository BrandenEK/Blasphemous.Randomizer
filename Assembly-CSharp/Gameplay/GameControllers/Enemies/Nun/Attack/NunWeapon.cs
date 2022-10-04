using System;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Enemies.Nun.Attack
{
	public class NunWeapon : Weapon
	{
		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}
	}
}
