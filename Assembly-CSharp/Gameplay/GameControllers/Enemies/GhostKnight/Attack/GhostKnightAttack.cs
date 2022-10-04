using System;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Enemies.GhostKnight.Attack
{
	public class GhostKnightAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._ghostKnight = (GhostKnight)base.EntityOwner;
			base.CurrentEnemyWeapon = base.EntityOwner.GetComponentInChildren<Weapon>();
			this._attackArea = base.EntityOwner.GetComponentInChildren<AttackArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._attackArea.OnStay += this.AttackAreaOnStay;
			this._attackArea.OnExit += this.AttackAreaOnExit;
		}

		private void AttackAreaOnStay(object sender, Collider2DParam collider2DParam)
		{
			if (this._ghostKnight.MotionLerper.IsLerping && !this._attacked)
			{
				this._attacked = true;
				this.CurrentWeaponAttack();
			}
		}

		private void AttackAreaOnExit(object sender, Collider2DParam collider2DParam)
		{
			this._attacked = false;
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			if (base.CurrentEnemyWeapon == null)
			{
				return;
			}
			Hit weapondHit = default(Hit);
			float final = this._ghostKnight.Stats.Strength.Final;
			weapondHit.AttackingEntity = this._ghostKnight.gameObject;
			weapondHit.DamageType = this.DamageType;
			weapondHit.DamageAmount = final;
			weapondHit.Force = this.Force;
			weapondHit.HitSoundId = this.HitSound;
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		private AttackArea _attackArea;

		private bool _attacked;

		private GhostKnight _ghostKnight;
	}
}
