using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class IsidoraAnimatorInyector : EnemyAnimatorInyector
	{
		internal void CheckFlagAnimationSpeed()
		{
			if (this.resetAnimationSpeedFlag)
			{
				this.ResetAnimationSpeed();
			}
		}

		public void SetFireScythe(bool on)
		{
			this.fireScythe = on;
			if (this.vfxSprite == null)
			{
				this.vfxSprite = this.vfxAnimator.GetComponentInChildren<SpriteRenderer>();
			}
			this.vfxSprite.enabled = on;
		}

		public bool IsScytheOnFire()
		{
			return this.fireScythe;
		}

		public void PlayDeath()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.ResetAll();
			this.SetDualBool(IsidoraAnimatorInyector.B_Death, true);
		}

		public void StopDeath()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			this.SetDualBool(IsidoraAnimatorInyector.B_Death, false);
		}

		public void PlaySlashAttack()
		{
			this.SetDualTrigger(IsidoraAnimatorInyector.T_Slash);
		}

		public void StopSlashAttack()
		{
			this.ResetDualTrigger(IsidoraAnimatorInyector.T_Slash);
		}

		public void PlayRisingSlash()
		{
			this.SetDualTrigger(IsidoraAnimatorInyector.T_Rising);
		}

		public void StopRisingSlash()
		{
			this.ResetDualTrigger(IsidoraAnimatorInyector.T_Rising);
		}

		public void SetTwirl(bool twirl)
		{
			if (!twirl)
			{
				this.resetAnimationSpeedFlag = !twirl;
			}
			this.SetDualBool(IsidoraAnimatorInyector.B_Twirl, twirl);
		}

		public void SetCasting(bool active)
		{
			this.SetDualBool(IsidoraAnimatorInyector.B_Cast, active);
		}

		public void SetHidden(bool hidden)
		{
			this.SetDualBool(IsidoraAnimatorInyector.B_Hide, hidden);
		}

		public void SetAttackAnticipation(bool hold)
		{
			this.SetDualBool(IsidoraAnimatorInyector.B_Hold, hold);
		}

		public void SetFadeSlash(bool fade)
		{
			this.SetDualBool(IsidoraAnimatorInyector.B_Fade, fade);
		}

		public void ResetAll()
		{
			this.ResetDualTrigger(IsidoraAnimatorInyector.T_Slash);
			this.ResetDualTrigger(IsidoraAnimatorInyector.T_Rising);
			this.SetDualBool(IsidoraAnimatorInyector.B_Cast, false);
			this.SetDualBool(IsidoraAnimatorInyector.B_Hide, false);
			this.SetDualBool(IsidoraAnimatorInyector.B_Hold, false);
			this.SetDualBool(IsidoraAnimatorInyector.B_Twirl, false);
			this.SetDualBool(IsidoraAnimatorInyector.B_Fade, false);
			this.ResetAnimationSpeed();
		}

		private void SetAnimatorSpeed(float s)
		{
			this.mainAnimator.speed = s;
			this.vfxAnimator.speed = s;
			this.debugAnimatorSpeed = s;
		}

		private float GetAnimatorSpeed()
		{
			return this.mainAnimator.speed;
		}

		public void Accelerate(float seconds)
		{
			this.resetAnimationSpeedFlag = false;
			this.EaseTwirl(this.GetAnimatorSpeed(), this.maxSpeed, seconds, 8);
		}

		public void Decelerate(float seconds)
		{
			this.resetAnimationSpeedFlag = false;
			this.EaseTwirl(this.GetAnimatorSpeed(), this.minSpeed, seconds, 8);
		}

		public void EaseTwirl(float minVal, float maxVal, float duration, Ease easingFunction)
		{
			if (this.easeTween != null)
			{
				TweenExtensions.Kill(this.easeTween, false);
			}
			this.SetAnimatorSpeed(minVal);
			this.easeTween = TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To(new DOGetter<float>(this.GetAnimatorSpeed), new DOSetter<float>(this.SetAnimatorSpeed), maxVal, duration), easingFunction);
		}

		public void ResetAnimationSpeed()
		{
			if (this.easeTween != null)
			{
				TweenExtensions.Kill(this.easeTween, false);
			}
			this.resetAnimationSpeedFlag = false;
			this.SetAnimatorSpeed(1f);
		}

		public float GetVanishAnimationDuration()
		{
			return 1.4f;
		}

		private void ResetDualTrigger(int name)
		{
			this.mainAnimator.ResetTrigger(name);
			this.vfxAnimator.ResetTrigger(name);
		}

		private void SetDualTrigger(int name)
		{
			this.mainAnimator.SetTrigger(name);
			this.vfxAnimator.SetTrigger(name);
		}

		private void SetDualBool(int name, bool value)
		{
			this.mainAnimator.SetBool(name, value);
			this.vfxAnimator.SetBool(name, value);
		}

		private static readonly int T_Slash = Animator.StringToHash("SLASH");

		private static readonly int T_Rising = Animator.StringToHash("RISING");

		private static readonly int T_Orb = Animator.StringToHash("ORB_COLLECTED");

		private static readonly int B_Death = Animator.StringToHash("DEATH");

		private static readonly int B_Hold = Animator.StringToHash("HOLD");

		private static readonly int B_Twirl = Animator.StringToHash("TWIRL");

		private static readonly int B_Hide = Animator.StringToHash("HIDING");

		private static readonly int B_Cast = Animator.StringToHash("CASTING");

		private static readonly int B_Fade = Animator.StringToHash("FADE_SLASH");

		private bool fireScythe;

		public Animator mainAnimator;

		public Animator vfxAnimator;

		private SpriteRenderer vfxSprite;

		public bool resetAnimationSpeedFlag;

		private float minSpeed = 0.5f;

		private float maxSpeed = 2f;

		public float debugAnimatorSpeed;

		private Tween easeTween;

		private const float VANISH_ANIMATION_DURATION = 1.4f;
	}
}
