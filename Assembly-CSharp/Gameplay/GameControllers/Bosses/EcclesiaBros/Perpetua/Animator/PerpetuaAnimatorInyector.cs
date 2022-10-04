using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua.Animator
{
	public class PerpetuaAnimatorInyector : EnemyAnimatorInyector
	{
		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void Vanish()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("VANISH");
		}

		public void Appear()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("APPEAR");
		}

		public void Spell()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("SPELL");
		}

		public void ChargeDash(bool activate)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("CHARGE", activate);
		}

		public void Dash()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DASH");
		}

		public void Flap(bool activate)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("FLAP", activate);
		}

		public void AnimationEvent_Dispose()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}
	}
}
