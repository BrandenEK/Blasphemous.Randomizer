using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Acolyte.Animator
{
	public class AcolyteAnimatorInyector : EnemyAnimatorInyector
	{
		public void Idle()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.SetBool("ATTACK", false);
			base.EntityAnimator.SetBool("CHASING", false);
		}

		public void Wander()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", true);
			base.EntityAnimator.SetBool("ATTACK", false);
			base.EntityAnimator.SetBool("CHASING", false);
		}

		public void Chase(Transform targetPosition)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			if (!this._chasing)
			{
				this._chasing = true;
			}
			base.EntityAnimator.SetBool("CHASING", true);
			base.EntityAnimator.SetBool("ATTACK", false);
			base.EntityAnimator.SetBool("WALK", false);
		}

		public void Attack(bool isGrounded = true)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.SetBool("CHASING", false);
			base.EntityAnimator.SetBool("ATTACK", isGrounded);
		}

		public void Damage()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void ParryReaction()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("CHASING", false);
			base.EntityAnimator.SetBool("ATTACK", false);
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.Play("ParryReaction");
		}

		public void Overthrow()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("OVERTHROW");
		}

		public void StopChasing()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			if (this._chasing)
			{
				this._chasing = !this._chasing;
				base.EntityAnimator.SetBool("CHASING", false);
				base.EntityAnimator.Play("Stop Running");
			}
		}

		public void Dead()
		{
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void Grounded(bool isGrounded)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("GROUNDED", isGrounded);
		}

		private bool _chasing;
	}
}
