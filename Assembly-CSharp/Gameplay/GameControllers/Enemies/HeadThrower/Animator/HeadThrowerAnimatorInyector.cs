using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.HeadThrower.Animator
{
	public class HeadThrowerAnimatorInyector : EnemyAnimatorInyector
	{
		public void Damage()
		{
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void Death()
		{
			base.EntityAnimator.SetTrigger("DEATH");
		}
	}
}
