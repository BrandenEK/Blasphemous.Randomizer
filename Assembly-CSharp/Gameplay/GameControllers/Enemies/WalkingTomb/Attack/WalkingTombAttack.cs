using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;

namespace Gameplay.GameControllers.Enemies.WalkingTomb.Attack
{
	public class WalkingTombAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponent<WalkingTombWeapon>();
		}

		private Hit GetHit
		{
			get
			{
				return new Hit
				{
					HitSoundId = this.HitSound,
					AttackingEntity = base.EntityOwner.gameObject,
					DamageAmount = base.EntityOwner.Stats.Strength.Final,
					DamageElement = DamageArea.DamageElement.Normal,
					DamageType = this.DamageType,
					Force = this.Force,
					Unnavoidable = true
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
