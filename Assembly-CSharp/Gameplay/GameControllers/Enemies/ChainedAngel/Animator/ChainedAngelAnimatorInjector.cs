using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.ChainedAngel.Animator
{
	public class ChainedAngelAnimatorInjector : EnemyAnimatorInyector
	{
		public void Death()
		{
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void AnimationEvent_Dispose()
		{
			this.OwnerEntity.transform.parent.gameObject.SetActive(false);
		}
	}
}
