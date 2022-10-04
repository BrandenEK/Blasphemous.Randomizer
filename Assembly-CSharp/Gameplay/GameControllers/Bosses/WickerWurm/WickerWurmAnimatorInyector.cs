using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Bosses.WickerWurm
{
	public class WickerWurmAnimatorInyector : EnemyAnimatorInyector
	{
		public void PlayAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("ATTACK");
		}

		public void PlayGrowl()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("GROWL");
		}

		public void SetOpen(bool open)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("OPEN", open);
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("ATTACK");
		}
	}
}
