using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.Flagellant.Animator
{
	public class FlagellantAnimatorInyector : EnemyAnimatorInyector
	{
		public void Grounded(bool isGrounded)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("GROUNDED", isGrounded);
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

		public void Attack(bool isGrounded)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.SetBool("CHASING", false);
			base.EntityAnimator.SetBool("ATTACK", isGrounded);
		}

		public void Chase(bool isGrounded)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("ATTACK", false);
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.SetBool("CHASING", true);
			base.EntityAnimator.SetBool("GROUNDED", isGrounded);
		}

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

		public void Fall()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
		}

		public void DamageImpact()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void Hurt()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("CHASING", false);
			base.EntityAnimator.SetBool("WALK", false);
		}

		public void ParryReaction()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("ATTACK", false);
			base.EntityAnimator.SetBool("CHASING", false);
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

		public void Stunt()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.Play("Stunt");
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.Play("Death");
		}
	}
}
