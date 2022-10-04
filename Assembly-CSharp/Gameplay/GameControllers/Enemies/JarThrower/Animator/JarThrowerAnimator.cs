using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.JarThrower.Animator
{
	public class JarThrowerAnimator : EnemyAnimatorInyector
	{
		public JarThrower JarThrower { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.JarThrower = (JarThrower)this.OwnerEntity;
			if (this.JarThrower)
			{
				this.JarThrower.JumpAttack.OnJumpLanded += this.OnJumpLanded;
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.JarThrower.Animator.SetBool("GROUNDED", this.JarThrower.Status.IsGrounded);
			if (this.JarThrower.IsAttacking && this.JarThrower.Controller.PlatformCharacterPhysics.VSpeed < -0.1f && !this.JarThrower.Status.IsGrounded)
			{
				this.JarThrower.Animator.SetBool("REACH_MAX_HEIGHT", true);
			}
		}

		public void Walk(bool walk = true)
		{
			this.JarThrower.Animator.SetBool("WALK", walk);
			this.JarThrower.Animator.ResetTrigger("JUMP");
			if (walk)
			{
				this.Run(false);
			}
		}

		public void Run(bool run = true)
		{
			this.JarThrower.Animator.SetBool("RUN", run);
			this.JarThrower.Animator.ResetTrigger("JUMP");
			if (run)
			{
				this.Walk(false);
			}
		}

		public void Death()
		{
			this.JarThrower.Animator.SetBool("DEATH", true);
			if (this.JarThrower)
			{
				this.JarThrower.JumpAttack.OnJumpLanded -= this.OnJumpLanded;
			}
		}

		public void Healing()
		{
			this.JarThrower.Animator.SetTrigger("DRINK");
		}

		public void Jump()
		{
			if (this.JarThrower.IsAttacking)
			{
				this.JumpAttack();
			}
			else
			{
				this.JumpChase();
			}
		}

		private void JumpChase()
		{
			this.JarThrower.JumpAttack.OnJumpAdvancedEvent += this.JumpAttackOnOnJumpAdvancedEvent;
			this.JarThrower.Animator.SetTrigger("JUMP");
			this.JarThrower.Animator.SetBool("REACH_MAX_HEIGHT", false);
			this.JarThrower.Behaviour.LookAtTarget(this.JarThrower.Target.transform.position);
		}

		private void OnJumpLanded()
		{
			this.JarThrower.Animator.SetBool("ATTACKING", false);
			this.JarThrower.Animator.SetBool("REACH_MAX_HEIGHT", false);
			if (!this.JarThrower.Animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
			{
				this.JarThrower.Animator.Play("Landing");
			}
		}

		private void JumpAttack()
		{
			this.JarThrower.Animator.SetTrigger("JUMP");
			this.JarThrower.Animator.SetBool("REACH_MAX_HEIGHT", false);
			this.JarThrower.Animator.SetBool("ATTACKING", true);
			this.JarThrower.Behaviour.LookAtTarget(this.JarThrower.Target.transform.position);
		}

		public void ResetJumpAttack()
		{
			this.JarThrower.Animator.SetBool("ATTACKING", false);
			this.JarThrower.IsAttacking = false;
		}

		public void ThrowJar()
		{
			this.JarThrower.IsFalling = true;
			Vector3 targetPosition = this.GetTargetPosition(this.JarThrower.Target.transform.position);
			Vector3 direction = targetPosition - this.JarThrower.JarAttack.projectileSource.position;
			this.JarThrower.JarAttack.SetProjectileWeaponDamage((int)this.JarThrower.Stats.Strength.Final);
			StraightProjectile straightProjectile = this.JarThrower.JarAttack.Shoot(targetPosition);
			straightProjectile.Init(direction, this.JarThrower.Behaviour.JarProjectileSpeed);
		}

		private Vector3 GetTargetPosition(Vector3 targetPosition)
		{
			Vector3 result = targetPosition;
			Vector3 position = this.JarThrower.JarAttack.projectileSource.position;
			if (this.JarThrower.Status.Orientation == EntityOrientation.Left)
			{
				if (result.x > position.x)
				{
					result.x = position.x;
				}
			}
			else if (result.x < position.x)
			{
				result.x = position.x;
			}
			return result;
		}

		public void UseJumpCurve()
		{
			if (this.JarThrower.IsAttacking)
			{
				base.StartCoroutine(this.JarThrower.Behaviour.JumpAttackCoroutine());
			}
			else
			{
				this.JarThrower.Behaviour.LookAtTarget(this.JarThrower.Target.transform.position);
				this.JarThrower.JumpAttack.Use(this.JarThrower.transform, this.JarThrower.Target.transform.position);
			}
		}

		private void JumpAttackOnOnJumpAdvancedEvent(Vector2 obj)
		{
			if (obj.y >= 0.1f)
			{
				return;
			}
			this.JarThrower.Animator.SetBool("REACH_MAX_HEIGHT", true);
			this.JarThrower.JumpAttack.OnJumpAdvancedEvent -= this.JumpAttackOnOnJumpAdvancedEvent;
		}

		public void DisableEntity()
		{
			this.JarThrower.gameObject.SetActive(false);
			this.JarThrower.Behaviour.enabled = false;
		}
	}
}
