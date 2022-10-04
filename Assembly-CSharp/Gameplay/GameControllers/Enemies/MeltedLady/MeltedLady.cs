using System;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady
{
	public class MeltedLady : FloatingLady, IDamageable
	{
		public BossInstantProjectileAttack Attack { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Attack = base.GetComponentInChildren<BossInstantProjectileAttack>();
			this.behaviour = base.GetComponent<EnemyBehaviour>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Attack.SetDamage((int)this.Stats.Strength.Final);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.behaviour.enabled)
			{
				this.behaviour.enabled = true;
			}
		}

		public void Damage(Hit hit)
		{
			base.DamageArea.TakeDamage(hit, false);
			base.ColorFlash.TriggerColorFlash();
			if (this.Status.Dead)
			{
				base.DamageArea.DamageAreaCollider.enabled = false;
				this.AnimatorInyector.Death();
			}
			else
			{
				this.AnimatorInyector.Hurt();
				base.Audio.Hurt();
			}
			this.SleepTimeByHit(hit);
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

		private EnemyBehaviour behaviour;
	}
}
