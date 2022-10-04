using System;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ChasingHead.Attack
{
	public class ChasingHeadAttack : EnemyAttack
	{
		public AttackArea AttackArea { get; set; }

		public bool EntityAttacked { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.EntityOwner.GetComponentInChildren<Weapon>();
			this.AttackArea = base.EntityOwner.GetComponentInChildren<AttackArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.OnStay += this.AttackAreaOnStay;
			this.AttackArea.OnExit += this.AttackAreaOnExit;
			base.EntityOwner.OnDeath += this.OnDeath;
		}

		private void AttackAreaOnStay(object sender, Collider2DParam e)
		{
			if (this.EntityAttacked)
			{
				return;
			}
			this.EntityAttacked = true;
			this.CurrentWeaponAttack(DamageArea.DamageType.Normal);
			base.EntityOwner.Animator.Play(this.DeathAnim);
		}

		private void OnDeath()
		{
			if (this.AttackArea.WeaponCollider.enabled)
			{
				this.AttackArea.WeaponCollider.enabled = false;
			}
		}

		private void AttackAreaOnExit(object sender, Collider2DParam e)
		{
			if (this.EntityAttacked)
			{
				this.EntityAttacked = !this.EntityAttacked;
			}
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			base.CurrentWeaponAttack(damageType);
			if (base.CurrentEnemyWeapon == null)
			{
				return;
			}
			float final = base.EntityOwner.Stats.Strength.Final;
			Hit weapondHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageType = damageType,
				DamageAmount = final,
				HitSoundId = this.HitSound
			};
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		public readonly int DeathAnim = Animator.StringToHash("Death");
	}
}
