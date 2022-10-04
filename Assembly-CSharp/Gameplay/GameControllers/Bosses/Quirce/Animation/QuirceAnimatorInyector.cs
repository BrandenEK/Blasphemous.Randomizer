using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Bosses.Quirce.Animation
{
	public class QuirceAnimatorInyector : EnemyAnimatorInyector
	{
		public void Throw()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("THROW");
		}

		public void Spiral(bool on)
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("SPIRAL", on);
		}

		public void Dash(bool state)
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("DASH", state);
		}

		public void Hurt()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void ResetTeleport()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger("TELEPORT_OUT");
		}

		public void ResetHurt()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger("HURT");
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

		public void SetToss(bool toss)
		{
			base.EntityAnimator.SetBool("TOSS", toss);
		}

		public void BigDashPreparation()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("PREPARATION");
		}

		public void Landing()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("LANDING");
		}

		public void TeleportInSword()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TELEPORT_IN_TO_SWORD");
		}

		public void TeleportOutSword()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TELEPORT_OUT_TO_SWORD");
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
			QuirceBehaviour componentInChildren = this.OwnerEntity.GetComponentInChildren<QuirceBehaviour>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetCoolDown();
			}
		}
	}
}
