using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.CauldronNun.Animator
{
	public class CauldronNunAnimatorInyector : EnemyAnimatorInyector
	{
		private CauldronNun CauldronNun { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.CauldronNun = (CauldronNun)this.OwnerEntity;
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
			if (this.CauldronNun)
			{
				this.CauldronNun.Audio.PlayDeath();
			}
		}

		public void PullChain()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("CAST");
			this.CauldronNun.Audio.PlayCall();
		}

		public void Hurt()
		{
		}

		public void RingAnimationEvent()
		{
			if (this.CauldronNun == null)
			{
				return;
			}
			this.CauldronNun.Behaviour.TriggerAllTraps();
		}
	}
}
