using System;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Enemies.BellCarrier.Attack
{
	public class BellCarrierWeapon : Weapon
	{
		protected override void OnAwake()
		{
			base.OnAwake();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

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
