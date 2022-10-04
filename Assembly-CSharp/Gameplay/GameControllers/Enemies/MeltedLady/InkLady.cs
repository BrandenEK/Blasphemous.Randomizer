using System;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady
{
	public class InkLady : FloatingLady, IDamageable
	{
		public BossHomingLaserAttack Attack { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Attack = base.GetComponentInChildren<BossHomingLaserAttack>();
			this.behaviour = base.GetComponent<EnemyBehaviour>();
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
				this.Attack.Clear();
			}
			else
			{
				this.AnimatorInyector.Hurt();
			}
			this.SleepTimeByHit(hit);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public float BeamAttackTime = 2f;

		private EnemyBehaviour behaviour;
	}
}
