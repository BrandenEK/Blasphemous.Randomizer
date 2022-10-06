using System;
using System.Diagnostics;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffGiant.Animator
{
	public class PontiffGiantAnimatorInyector : EnemyAnimatorInyector
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PontiffGiantAnimatorInyector, Vector2> OnSpinProjectilePoint;

		public void AnimationEvent_LightScreenShake()
		{
			Vector2 vector = (this.OwnerEntity.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.15f, vector * 0.5f, 10, 0.01f, 0f, default(Vector3), 0.01f, false);
		}

		public void AnimationEvent_HeavyScreenShake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.3f, Vector3.up * 3f, 60, 0.01f, 0f, default(Vector3), 0.01f, false);
		}

		public void IntroBlend()
		{
			SpriteRenderer component = this.headAnimator.GetComponent<SpriteRenderer>();
			SpriteRenderer component2 = this.faceAnimator.GetComponent<SpriteRenderer>();
			SpriteRenderer component3 = this.bodyAnimator.GetComponent<SpriteRenderer>();
			ShortcutExtensions43.DOGradientColor(component, this.introGradient, this.introBlendDuration);
			ShortcutExtensions43.DOGradientColor(component2, this.introGradient, this.introBlendDuration);
			ShortcutExtensions43.DOGradientColor(component3, this.introGradient, this.introBlendDuration);
		}

		public void Death()
		{
			this.faceAnimator.SetTrigger("DEATH");
		}

		public void Open(bool open)
		{
			this.headAnimator.SetBool("OPEN", open);
			this.faceAnimator.SetBool("OPEN", open);
			this.bodyAnimator.SetBool("OPEN", open);
		}

		public Animator headAnimator;

		public Animator faceAnimator;

		public Animator bodyAnimator;

		public Gradient introGradient;

		public float introBlendDuration = 2f;
	}
}
