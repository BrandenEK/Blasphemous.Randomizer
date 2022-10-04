using System;
using DamageEffect;
using Gameplay.GameControllers.Bosses.WickerWurm;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BlindBaby
{
	public class WickerWurmHeart : Enemy, IDamageable
	{
		public EnemyDamageArea DamageArea { get; private set; }

		public WickerWurmAnimatorInyector AnimatorInyector { get; private set; }

		public DamageEffectScript damageEffect { get; private set; }

		private AttackArea attackArea { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<WickerWurmAnimatorInyector>();
			this.damageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.attackArea = base.GetComponentInChildren<AttackArea>();
			if (this.ownerWurm)
			{
				this.ownerWurm.OnDeath += this.OnWurmDeath;
			}
		}

		private void DamageFlash()
		{
			this.damageEffect.Blink(0f, 0.1f);
		}

		public void Damage(Hit hit)
		{
			this.DamageFlash();
			this.ownerWurm.Damage(hit);
		}

		private void OnWurmDeath()
		{
			if (this.attackArea)
			{
				this.attackArea.WeaponCollider.enabled = false;
			}
			if (this.ownerWurm)
			{
				this.ownerWurm.OnDeath -= this.OnWurmDeath;
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			throw new NotImplementedException();
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}

		public AnimationCurve slowTimeCurve;

		public WickerWurm ownerWurm;
	}
}
