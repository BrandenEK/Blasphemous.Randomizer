using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Snake
{
	public class SnakeAnimatorInyector : EnemyAnimatorInyector
	{
		public void PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS intention)
		{
			if (intention != SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_BITE)
			{
				if (intention != SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_CAST)
				{
					if (intention == SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_SUMMON_SPIKES)
					{
						this.SetHeadDualBool(SnakeAnimatorInyector.B_SummonSpikes, true);
					}
				}
				else
				{
					this.SetHeadDualBool(SnakeAnimatorInyector.B_Cast, true);
				}
			}
			else
			{
				this.SetHeadDualBool(SnakeAnimatorInyector.B_Bite, true);
			}
		}

		private void GetBackgroundAnimationReference()
		{
			if (this.bgAnimator == null)
			{
				this.bgAnimator = Object.FindObjectOfType<SnakeBackgroundAnimator>();
			}
		}

		public void BackgroundAnimationSetActive(bool active)
		{
			this.GetBackgroundAnimationReference();
			if (this.bgAnimator != null)
			{
				this.bgAnimator.Activate(active);
			}
		}

		public void BackgroundAnimationSetSpeed(float spd, float seconds = 0.5f)
		{
			this.GetBackgroundAnimationReference();
			if (this.bgAnimator != null)
			{
				this.bgAnimator.LerpSpeed(spd, seconds);
			}
		}

		public void StopOpenMouth()
		{
			this.SetHeadDualBool(SnakeAnimatorInyector.B_Bite, false);
			this.SetHeadDualBool(SnakeAnimatorInyector.B_Cast, false);
			this.SetHeadDualBool(SnakeAnimatorInyector.B_SummonSpikes, false);
		}

		public void PlayCloseMouth()
		{
			this.SetHeadDualBool(SnakeAnimatorInyector.B_Close, true);
		}

		public void StopCloseMouth()
		{
			this.SetHeadDualBool(SnakeAnimatorInyector.B_Close, false);
		}

		public void SetCounterBite()
		{
			this.SetHeadDualTrigger(SnakeAnimatorInyector.T_CounterBite);
		}

		public void PlayDeath()
		{
			this.SetHeadDualTrigger(SnakeAnimatorInyector.T_Death);
		}

		public void PlayDeathBite()
		{
			this.SetHeadDualBool(SnakeAnimatorInyector.B_DeathBite, true);
		}

		public void StopDeathBite()
		{
			this.SetHeadDualBool(SnakeAnimatorInyector.B_DeathBite, false);
		}

		public void ResetAll()
		{
			this.SetHeadDualBool(SnakeAnimatorInyector.B_Bite, false);
			this.SetHeadDualBool(SnakeAnimatorInyector.B_Close, false);
			this.SetHeadDualBool(SnakeAnimatorInyector.B_SummonSpikes, false);
			this.SetHeadDualBool(SnakeAnimatorInyector.B_Cast, false);
			this.ResetHeadDualTrigger(SnakeAnimatorInyector.T_CounterBite);
		}

		private void ResetHeadDualTrigger(int name)
		{
			this.LeftHeadAnimator.ResetTrigger(name);
			this.RightHeadAnimator.ResetTrigger(name);
		}

		private void SetHeadDualTrigger(int name)
		{
			this.LeftHeadAnimator.SetTrigger(name);
			this.RightHeadAnimator.SetTrigger(name);
		}

		private void SetHeadDualBool(int name, bool value)
		{
			this.LeftHeadAnimator.SetBool(name, value);
			this.RightHeadAnimator.SetBool(name, value);
		}

		public void SetHeadDualSpeed(float speed)
		{
			this.LeftHeadAnimator.speed = speed;
			this.RightHeadAnimator.speed = speed;
		}

		private static readonly int B_Bite = Animator.StringToHash("BITE");

		private static readonly int B_Cast = Animator.StringToHash("CAST");

		private static readonly int B_SummonSpikes = Animator.StringToHash("SUMMON_SPIKES");

		private static readonly int B_Close = Animator.StringToHash("CLOSE");

		private static readonly int B_DeathBite = Animator.StringToHash("DEATH_BITE");

		private static readonly int T_CounterBite = Animator.StringToHash("COUNTER_BITE");

		private static readonly int T_Death = Animator.StringToHash("DEATH");

		public Animator LeftHeadAnimator;

		public Animator RightHeadAnimator;

		private SnakeBackgroundAnimator bgAnimator;

		public enum OPEN_MOUTH_INTENTIONS
		{
			TO_BITE,
			TO_CAST,
			TO_SUMMON_SPIKES
		}
	}
}
