using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GoldenCorpse.Animator
{
	public class GoldenCorpseAnimatorInyector : EnemyAnimatorInyector
	{
		public void Walk()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			if (!base.EntityAnimator.GetBool("WALK"))
			{
				base.EntityAnimator.SetBool("WALK", true);
			}
		}

		public void StopWalk()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			if (base.EntityAnimator.GetBool("WALK"))
			{
				base.EntityAnimator.SetBool("WALK", false);
			}
		}

		public void PlayAwaken()
		{
			base.EntityAnimator.SetBool("AWAKE", true);
		}

		public void PlaySleep()
		{
			base.EntityAnimator.SetBool("AWAKE", false);
		}

		public void TurnAround()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TURN");
		}

		public void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void PlayRandomSleepAnim()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			int num = UnityEngine.Random.Range(0, this.totalAnimationVariants);
			base.EntityAnimator.SetInteger("ID", num);
			base.EntityAnimator.Play("Sleep" + (num + 1));
		}

		public void PlayCrouchedAnim()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetInteger("ID", 0);
			base.EntityAnimator.SetBool("AWAKE", false);
		}

		public void PlayFreezeAnim()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetInteger("ID", 1);
			base.EntityAnimator.SetBool("AWAKE", false);
		}

		public void FreezeAnimation()
		{
			if (base.EntityAnimator.speed > 0.1f)
			{
				this.origAnimationSpeed = base.EntityAnimator.speed;
				base.EntityAnimator.speed = 0.01f;
			}
		}

		public void UnFreezeAnimation()
		{
			if (base.EntityAnimator.speed < 0.1f)
			{
				base.EntityAnimator.speed = this.origAnimationSpeed;
			}
		}

		private int totalAnimationVariants = 2;

		public float origAnimationSpeed = 1f;
	}
}
