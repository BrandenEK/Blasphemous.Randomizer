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
			Vector2 a = (this.OwnerEntity.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.15f, a * 0.5f, 10, 0.01f, 0f, default(Vector3), 0.01f, false);
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
			component.DOGradientColor(this.introGradient, this.introBlendDuration);
			component2.DOGradientColor(this.introGradient, this.introBlendDuration);
			component3.DOGradientColor(this.introGradient, this.introBlendDuration);
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
