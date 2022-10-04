using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy.Animator
{
	public class PatrollingFlyingEnemyAnimatorInyector : EnemyAnimatorInyector
	{
		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		[TutorialId]
		public string TutorialId;

		public bool isCherub;
	}
}
