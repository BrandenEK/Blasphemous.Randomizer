using System;
using System.Diagnostics;
using BezierSplines;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.MasterAnguish.Audio;
using Gameplay.GameControllers.Enemies.SingleAnguish.Animator;
using Gameplay.GameControllers.Entities;
using Maikel.SteeringBehaviors;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.TresAngustias
{
	public class SingleAnguish : Enemy, IDamageable
	{
		public SingleAnguishBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public SingleAnguishAnimatorInyector AnimatorInyector { get; private set; }

		public AutonomousAgent autonomousAgent { get; private set; }

		public SingleAnguishAudio Audio { get; private set; }

		public Arrive arriveBehaviour { get; private set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<SingleAnguish, Hit> OnSingleAnguishDamaged;

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponent<SingleAnguishBehaviour>();
			this.Audio = base.GetComponentInChildren<SingleAnguishAudio>();
			this.arriveBehaviour = base.GetComponent<Arrive>();
			this.autonomousAgent = base.GetComponent<AutonomousAgent>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<SingleAnguishAnimatorInyector>();
			this.shaderEffects = base.GetComponentInChildren<MasterShaderEffects>();
		}

		protected override void OnFixedUpdated()
		{
			base.OnFixedUpdated();
			this.SetSortingLayerFromY();
		}

		private void SetSortingLayerFromY()
		{
			base.SpriteRenderer.sortingOrder = -(int)base.transform.position.y;
		}

		public void SetPath(BezierSpline s)
		{
			this.Behaviour = base.GetComponent<SingleAnguishBehaviour>();
			this.Behaviour.SetPath(s);
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

		public void Damage(Hit hit)
		{
			if (this.OnSingleAnguishDamaged != null)
			{
				this.OnSingleAnguishDamaged(this, hit);
			}
			this.shaderEffects.DamageEffectBlink(0f, 0.2f, null);
			this.SleepTimeByHit(hit);
		}

		public override void Kill()
		{
			base.Kill();
			UnityEngine.Debug.Log("KILL OVERRIDE");
			this.DamageArea.TakeDamage(new Hit
			{
				DamageAmount = 9999f
			}, false);
			if (this.Status.Dead)
			{
				this.Behaviour.Death();
			}
			else
			{
				this.Behaviour.Damage();
			}
		}

		public Vector3 GetPosition()
		{
			throw new NotImplementedException();
		}

		public MasterShaderEffects shaderEffects;
	}
}
