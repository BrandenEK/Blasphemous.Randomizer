using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.AshCharger.AI
{
	public class AshChargerBehaviour : EnemyBehaviour
	{
		protected AshCharger AshCharger { get; set; }

		public bool TargetIsDead { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this.AshCharger = (AshCharger)this.Entity;
			this.AshCharger.IsGuarding = true;
			this.AshCharger.StateMachine.SwitchState<AshChargerAppearState>();
			this.AshCharger.OnDeath += this.OnDeath;
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.TargetOnDead));
			this.AshCharger.Target = Core.Logic.Penitent.gameObject;
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
		}

		public override void StopMovement()
		{
			this.AshCharger.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			this.AshCharger.Inputs.HorizontalInput = 0f;
		}

		public bool CanSeeTarget
		{
			get
			{
				return this.AshCharger.VisionCone.CanSeeTarget(this.AshCharger.Target.transform, "Penitent", false);
			}
		}

		public bool CanWalk
		{
			get
			{
				return !this.AshCharger.MotionChecker.HitsBlock && this.AshCharger.MotionChecker.HitsFloor;
			}
		}

		public bool TargetIsOnAttackDistance
		{
			get
			{
				float num = Vector2.Distance(this.AshCharger.transform.position, this.AshCharger.Target.transform.position);
				return num < this.AshCharger.Behaviour.MinAttackDistance;
			}
		}

		public bool TargetIsInFront
		{
			get
			{
				EntityOrientation orientation = this.AshCharger.Status.Orientation;
				Vector3 position = this.AshCharger.transform.position;
				Vector3 position2 = this.AshCharger.Target.transform.position;
				bool flag;
				if (orientation == EntityOrientation.Right)
				{
					flag = (position.x <= position2.x);
				}
				else
				{
					flag = (position.x > position2.x);
				}
				return this.CanSeeTarget && flag;
			}
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		private void OnDeath()
		{
			this.AshCharger.AnimatorInyector.SetSpeed(1f);
			this.dashAttack.StopDash(base.transform, false);
			this.dashAttack.OnDashFinishedEvent -= this.DashAttack_OnDashFinishedEvent;
			this.AshCharger.OnDeath -= this.OnDeath;
			this.AshCharger.StateMachine.enabled = false;
			this.AshCharger.AnimatorInyector.Death();
			this.StopMovement();
		}

		private void TargetOnDead()
		{
			if (Core.Logic.Penitent != null)
			{
				Penitent penitent = Core.Logic.Penitent;
				penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.TargetOnDead));
			}
			this.AshCharger.StateMachine.SwitchState<AshChargerAppearState>();
			this.TargetIsDead = true;
		}

		private float GetDirFromOrientation()
		{
			return (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
		}

		public override void Attack()
		{
			this.SetParticlesOrientation();
			Vector2 direction = Vector2.right * this.GetDirFromOrientation();
			this.dashAttack.OnDashFinishedEvent += this.DashAttack_OnDashFinishedEvent;
			this.dashAttack.OnDashAdvancedEvent += this.DashAttack_OnDashAdvancedEvent;
			this.dashAttack.Dash(base.transform, direction, 12f, 0f, false);
			this.AshCharger.AnimatorInyector.Attack(true);
		}

		private void SetParticlesOrientation()
		{
			this.particles.textureSheetAnimation.flipU = (float)((this.GetDirFromOrientation() != 1f) ? 0 : 1);
		}

		private void DashAttack_OnDashAdvancedEvent(float value)
		{
			float a = 0.5f;
			float b = 3f;
			this.AshCharger.AnimatorInyector.SetSpeed(Mathf.Lerp(a, b, value));
		}

		private void DashAttack_OnDashFinishedEvent()
		{
			this.dashAttack.OnDashFinishedEvent -= this.DashAttack_OnDashFinishedEvent;
			this.AshCharger.Kill();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public BossDashAttack dashAttack;

		public float MinAttackDistance = 2f;

		public float AttackCooldown = 1f;

		public ParticleSystem particles;
	}
}
