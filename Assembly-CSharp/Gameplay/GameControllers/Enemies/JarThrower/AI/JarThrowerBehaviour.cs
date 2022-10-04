using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.JarThrower.AI
{
	public class JarThrowerBehaviour : EnemyBehaviour
	{
		public JarThrower JarThrower { get; private set; }

		public bool IsHealing { get; set; }

		public bool TargetIsDead { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this.TargetIsDead = false;
			this.JarThrower = (JarThrower)this.Entity;
			this.JarThrower.StateMachine.SwitchState<JarThrowerWanderState>();
			this.JarThrower.OnDeath += this.OnDeath;
			Core.Logic.Penitent.OnDeath += this.PenitentOnDeath;
		}

		public bool TargetSeen
		{
			get
			{
				return this.JarThrower.VisionCone.CanSeeTarget(this.JarThrower.Target.transform, "Penitent", false);
			}
		}

		public void Jump(Vector3 target)
		{
			this.JarThrower.AnimatorInjector.Jump();
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this.JarThrower.IsFalling)
			{
				return;
			}
			if (targetPos.x > this.JarThrower.transform.position.x)
			{
				if (this.JarThrower.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.JarThrower.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.JarThrower.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.JarThrower.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			this.Move(this.WalkSpeed);
			this.JarThrower.AnimatorInjector.Walk(true);
			if (!this.CanWalk)
			{
				this.ReverseOrientation();
			}
		}

		public void Move(float speed)
		{
			float num = (this.JarThrower.Status.Orientation != EntityOrientation.Left) ? 1f : -1f;
			this.JarThrower.Inputs.HorizontalInput = ((!this.JarThrower.IsRunLanding && this.JarThrower.Status.IsGrounded) ? num : 0f);
			this.JarThrower.Controller.MaxWalkingSpeed = speed;
		}

		public bool CanWalk
		{
			get
			{
				return !this.JarThrower.MotionChecker.HitsBlock && this.JarThrower.MotionChecker.HitsFloor;
			}
		}

		public override void Chase(Transform targetPosition)
		{
			this.LookAtTarget(targetPosition.position);
			this.JarThrower.AnimatorInjector.Run(true);
			this.Move(this.RunSpeed);
		}

		public override void Attack()
		{
			this.JarThrower.IsAttacking = true;
			this.JarThrower.AnimatorInjector.Jump();
		}

		public IEnumerator JumpAttackCoroutine()
		{
			this.JarThrower.Inputs.Jump = true;
			yield return new WaitForSeconds(1f);
			this.JarThrower.Inputs.Jump = false;
			yield break;
		}

		public void Death()
		{
			this.JarThrower.StartCoroutine(this.CallAnimatorInyectorDeath());
		}

		private IEnumerator CallAnimatorInyectorDeath()
		{
			while (base.enabled)
			{
				this.JarThrower.AnimatorInjector.Death();
				yield return new WaitForSeconds(1f);
				if (base.enabled)
				{
					this.JarThrower.AnimatorInjector.EntityAnimator.Play("Death");
				}
			}
			yield break;
		}

		public void Healing()
		{
			if (this.JarThrower.Stats.Life.Current < this.JarThrower.Stats.Life.CurrentMax * 0.5f)
			{
				this.IsHealing = true;
				this.JarThrower.AnimatorInjector.Healing();
				this.StopMovement();
				this.JarThrower.Stats.Life.Current += this.JarThrower.Stats.Life.Base * this.HealingRatio / 100f;
			}
		}

		public override void Damage()
		{
		}

		public override void StopMovement()
		{
			this.JarThrower.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			this.JarThrower.Inputs.HorizontalInput = 0f;
			this.JarThrower.AnimatorInjector.Walk(false);
			this.JarThrower.AnimatorInjector.Run(false);
		}

		private void OnDeath()
		{
			this.JarThrower.OnDeath -= this.OnDeath;
			this.JarThrower.StateMachine.enabled = false;
			this.StopMovement();
			this.Death();
		}

		private void PenitentOnDeath()
		{
			Core.Logic.Penitent.OnDeath -= this.PenitentOnDeath;
			this.JarThrower.StateMachine.SwitchState<JarThrowerWanderState>();
			this.TargetIsDead = true;
		}

		public float AttackDistance;

		public float ThrowingDistance;

		public float HealingLapse = 2f;

		[Range(1f, 100f)]
		public float HealingRatio = 25f;

		public float AttackCooldown = 2f;

		public float TimeChasing = 2f;

		public float WalkSpeed = 1.25f;

		public float RunSpeed = 2.5f;

		public float JarProjectileSpeed = 10f;
	}
}
