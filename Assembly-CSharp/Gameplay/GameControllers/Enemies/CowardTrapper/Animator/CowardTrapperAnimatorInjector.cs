using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.CowardTrapper.Animator
{
	public class CowardTrapperAnimatorInjector : EnemyAnimatorInyector
	{
		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void Hurt()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void Scared()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			CowardTrapper cowardTrapper = (CowardTrapper)this.OwnerEntity;
			cowardTrapper.Behaviour.IsRunAway = true;
			base.EntityAnimator.SetTrigger("RUN");
		}

		public void Run()
		{
			CowardTrapper cowardTrapper = (CowardTrapper)this.OwnerEntity;
			cowardTrapper.Behaviour.StartRun();
			base.EntityAnimator.SetBool("RUNNING", true);
		}

		public void StopRun()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("RUNNING", false);
		}

		public void Dispose()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}
	}
}
