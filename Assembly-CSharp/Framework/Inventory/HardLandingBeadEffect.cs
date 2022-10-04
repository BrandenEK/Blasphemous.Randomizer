using System;
using Framework.Managers;
using UnityEngine;

namespace Framework.Inventory
{
	public class HardLandingBeadEffect : ObjectEffect_Stat
	{
		private Animator PenitentAnimator { get; set; }

		protected override bool OnApplyEffect()
		{
			this.PenitentAnimator = Core.Logic.Penitent.Animator;
			return base.OnApplyEffect();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.PenitentAnimator == null)
			{
				return;
			}
			if (this.PenitentAnimator.GetCurrentAnimatorStateInfo(0).IsName(this.AccAnimation) && Math.Abs(this.PenitentAnimator.speed - this.AnimatorNormalizedSpeed) > Mathf.Epsilon)
			{
				this.PenitentAnimator.speed = this.AnimatorNormalizedSpeed;
			}
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
			if (this.PenitentAnimator != null)
			{
				this.PenitentAnimator.speed = this._prevAnimatorSpeed;
			}
		}

		private float _prevAnimatorSpeed = 1f;

		[SerializeField]
		protected string AccAnimation;

		[Range(1f, 10f)]
		public float AnimatorNormalizedSpeed = 1.25f;
	}
}
