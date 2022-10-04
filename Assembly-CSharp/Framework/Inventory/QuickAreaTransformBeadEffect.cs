using System;
using Framework.Managers;
using UnityEngine;

namespace Framework.Inventory
{
	public class QuickAreaTransformBeadEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			QuickAreaTransformBeadEffect.SetAuraTransformSpeed(this.AuraTransformAnimationSpeed);
			return true;
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
			QuickAreaTransformBeadEffect.SetAuraTransformSpeed(1f);
		}

		private static void SetAuraTransformSpeed(float animationSpeed)
		{
			if (animationSpeed < 0f)
			{
				return;
			}
			Animator animator = Core.Logic.Penitent.Animator;
			if (animator)
			{
				animator.SetFloat("PRAYER_SPEED_MULTIPLIER", animationSpeed);
			}
		}

		[Range(1f, 2f)]
		public float AuraTransformAnimationSpeed = 1.5f;
	}
}
