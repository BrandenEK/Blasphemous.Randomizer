using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;

namespace Gameplay.GameControllers.Enemies.ViciousDasher.Attack
{
	public class ViciousDasherAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<ViciousDasherWeapon>();
		}

		private Hit GetHit
		{
			get
			{
				return new Hit
				{
					AttackingEntity = base.EntityOwner.gameObject,
					DamageAmount = base.EntityOwner.Stats.Strength.Final,
					DamageType = DamageArea.DamageType.Normal,
					HitSoundId = this.HitSound
				};
			}
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this.GetHit);
		}
	}
}
