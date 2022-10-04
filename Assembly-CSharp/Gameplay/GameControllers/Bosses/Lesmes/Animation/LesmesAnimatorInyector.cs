using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Bosses.Lesmes.Animation
{
	public class LesmesAnimatorInyector : EnemyAnimatorInyector
	{
		public void Throw()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("THROW");
		}

		public void Dash(bool state)
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("DASH", state);
		}

		public void TeleportIn()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TELEPORT_IN");
		}

		public void TeleportOut()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TELEPORT_OUT");
		}

		public void BigDashPreparation()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("PREPARATION");
		}

		public void Plunge(bool state)
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("PLUNGE", state);
		}

		public void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void AttackAnimationEvent()
		{
		}

		public void ResetCoolDownAttack()
		{
			LesmesBehaviour componentInChildren = this.OwnerEntity.GetComponentInChildren<LesmesBehaviour>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetCoolDown();
			}
		}
	}
}
