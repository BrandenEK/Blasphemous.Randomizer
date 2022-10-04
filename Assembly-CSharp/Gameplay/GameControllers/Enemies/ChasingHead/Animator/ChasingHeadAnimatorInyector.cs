using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.ChasingHead.Animator
{
	public class ChasingHeadAnimatorInyector : EnemyAnimatorInyector
	{
		public void Hurt()
		{
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void Death()
		{
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void PlayDeath()
		{
			ChasingHead chasingHead = (ChasingHead)this.OwnerEntity;
			chasingHead.Audio.PlayExplode();
		}
	}
}
