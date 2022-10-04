using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.Fool.Animator
{
	public class FoolAnimatorInyector : EnemyAnimatorInyector
	{
		public void Walk()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			if (!base.EntityAnimator.GetBool("WALK"))
			{
				base.EntityAnimator.SetBool("WALK", true);
			}
		}

		public void StopWalk()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			if (base.EntityAnimator.GetBool("WALK"))
			{
				base.EntityAnimator.SetBool("WALK", false);
			}
		}

		public void TurnAround()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TURN");
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}
	}
}
