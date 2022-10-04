using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.Attack
{
	public class NewFlagellantAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<NewFlagellantWeapon>();
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this.GetHit);
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
	}
}
