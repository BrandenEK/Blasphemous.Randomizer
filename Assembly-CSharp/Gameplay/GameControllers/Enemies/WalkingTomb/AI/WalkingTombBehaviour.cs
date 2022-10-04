using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WalkingTomb.AI
{
	public class WalkingTombBehaviour : EnemyBehaviour
	{
		protected WalkingTomb WalkingTomb { get; set; }

		public bool TargetIsDead { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this.WalkingTomb = (WalkingTomb)this.Entity;
			this.WalkingTomb.IsGuarding = true;
			this.WalkingTomb.StateMachine.SwitchState<WalkingTombWalkState>();
			this.WalkingTomb.OnDeath += this.OnDeath;
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.TargetOnDead));
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			float horizontalInput = (this.WalkingTomb.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.WalkingTomb.Inputs.HorizontalInput = horizontalInput;
			this.WalkingTomb.AnimatorInjector.Attack(false);
			if (!this.CanWalk)
			{
				this.ReverseOrientation();
			}
		}

		public override void StopMovement()
		{
			this.WalkingTomb.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			this.WalkingTomb.Inputs.HorizontalInput = 0f;
		}

		public bool CanSeeTarget
		{
			get
			{
				return this.WalkingTomb.VisionCone.CanSeeTarget(this.WalkingTomb.Target.transform, "Penitent", false);
			}
		}

		public bool CanWalk
		{
			get
			{
				return !this.WalkingTomb.MotionChecker.HitsBlock && this.WalkingTomb.MotionChecker.HitsFloor;
			}
		}

		public bool TargetIsOnAttackDistance
		{
			get
			{
				float num = Vector2.Distance(this.WalkingTomb.transform.position, this.WalkingTomb.Target.transform.position);
				return num < this.WalkingTomb.Behaviour.MinAttackDistance;
			}
		}

		public bool TargetIsInFront
		{
			get
			{
				EntityOrientation orientation = this.WalkingTomb.Status.Orientation;
				Vector3 position = this.WalkingTomb.transform.position;
				Vector3 position2 = this.WalkingTomb.Target.transform.position;
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
			this.WalkingTomb.OnDeath -= this.OnDeath;
			this.WalkingTomb.StateMachine.enabled = false;
			this.StopMovement();
		}

		private void TargetOnDead()
		{
			if (Core.Logic.Penitent != null)
			{
				Penitent penitent = Core.Logic.Penitent;
				penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.TargetOnDead));
			}
			this.WalkingTomb.StateMachine.SwitchState<WalkingTombWalkState>();
			this.TargetIsDead = true;
		}

		public override void Attack()
		{
			this.WalkingTomb.AnimatorInjector.Attack(true);
		}

		public override void Execution()
		{
			base.Execution();
			this.isExecuted = true;
			this.WalkingTomb.Animator.enabled = false;
			this.WalkingTomb.StateMachine.enabled = false;
			this.WalkingTomb.gameObject.layer = LayerMask.NameToLayer("Default");
			this.WalkingTomb.Audio.StopAttack();
			this.WalkingTomb.Animator.Play("Idle");
			this.StopMovement();
			this.WalkingTomb.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.WalkingTomb.Attack.enabled = false;
			this.WalkingTomb.EntExecution.InstantiateExecution();
			if (this.WalkingTomb.EntExecution != null)
			{
				this.WalkingTomb.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this.isExecuted = false;
			this.WalkingTomb.Animator.enabled = true;
			this.WalkingTomb.StateMachine.enabled = true;
			this.WalkingTomb.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this.WalkingTomb.SpriteRenderer.enabled = true;
			this.WalkingTomb.Animator.Play("Idle");
			this.WalkingTomb.CurrentLife = this.WalkingTomb.Stats.Life.Base / 2f;
			this.WalkingTomb.Attack.enabled = true;
			if (this.WalkingTomb.EntExecution != null)
			{
				this.WalkingTomb.EntExecution.enabled = false;
			}
		}

		public override void Damage()
		{
		}

		public float MinAttackDistance = 2f;

		public float AttackCooldown = 1f;

		public bool isExecuted;
	}
}
