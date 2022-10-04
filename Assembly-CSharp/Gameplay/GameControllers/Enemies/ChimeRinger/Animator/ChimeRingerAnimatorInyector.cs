using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.ChimeRinger.Animator
{
	public class ChimeRingerAnimatorInyector : EnemyAnimatorInyector
	{
		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
			if (this.OwnerEntity == null)
			{
				return;
			}
			ChimeRinger chimeRinger = (ChimeRinger)this.OwnerEntity;
			chimeRinger.Audio.PlayDeath();
		}

		public void Ring()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("CAST");
		}

		public void Hurt()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger("DEATH");
			base.EntityAnimator.ResetTrigger("CAST");
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void RingAnimationEvent()
		{
			if (this.OwnerEntity == null)
			{
				return;
			}
			ChimeRinger chimeRinger = (ChimeRinger)this.OwnerEntity;
			chimeRinger.Behaviour.TriggerAllTraps();
			chimeRinger.Audio.PlayCall();
		}
	}
}
