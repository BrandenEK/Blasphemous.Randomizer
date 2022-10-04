using System;
using DamageEffect;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua.Animator;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Maikel.SteeringBehaviors;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua
{
	public class Perpetua : Enemy, IDamageable
	{
		public PerpetuaBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public PerpetuaAnimatorInyector AnimatorInyector { get; private set; }

		public PerpetuaAudio Audio { get; private set; }

		public DamageEffectScript damageEffect { get; private set; }

		public AutonomousAgent autonomousAgent { get; private set; }

		public Arrive arriveBehaviour { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Behaviour = base.GetComponent<PerpetuaBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<PerpetuaAnimatorInyector>();
			this.Audio = base.GetComponentInChildren<PerpetuaAudio>();
			this.damageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.arriveBehaviour = base.GetComponent<Arrive>();
			this.autonomousAgent = base.GetComponent<AutonomousAgent>();
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

		public void DamageFlash()
		{
			this.damageEffect.Blink(0f, 0.1f);
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead || this.GuardHit(hit) || Core.Logic.Penitent.Stats.Life.Current <= 0f)
			{
				return;
			}
			this.DamageArea.TakeDamage(hit, false);
			this.DamageFlash();
			if (this.Status.Dead)
			{
				Core.Logic.ScreenFreeze.Freeze(0.1f, 2f, 0f, this.slowTimeCurve);
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
			}
			else
			{
				this.Behaviour.Damage();
				this.SleepTimeByHit(hit);
			}
		}

		public Vector3 GetPosition()
		{
			throw new NotImplementedException();
		}

		public PerpetuaPoints PerpetuaPoints;

		public AnimationCurve slowTimeCurve;
	}
}
