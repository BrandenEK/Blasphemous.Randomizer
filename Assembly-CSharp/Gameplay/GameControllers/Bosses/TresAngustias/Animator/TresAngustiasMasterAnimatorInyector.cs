using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Bosses.TresAngustias.Animator
{
	public class TresAngustiasMasterAnimatorInyector : EnemyAnimatorInyector
	{
		public void Disappear()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DISAPPEAR");
		}

		public void Divide()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DIVIDE");
		}

		public void Merge()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("MERGE");
		}
	}
}
