using System;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Snake
{
	public class SnakeWeapon : Weapon
	{
		public override void Attack(Hit weaponHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weaponHit);
		}

		private void LateUpdate()
		{
			base.transform.localScale = Vector3.one;
		}

		public override void OnHit(Hit weaponHit)
		{
		}
	}
}
