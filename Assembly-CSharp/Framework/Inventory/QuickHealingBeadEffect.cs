using System;
using Framework.Managers;
using UnityEngine;

namespace Framework.Inventory
{
	public class QuickHealingBeadEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			Core.Logic.Penitent.Animator.SetFloat(this.SPEED_PARAM, this.AnimatorSpeed);
			return true;
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
			Core.Logic.Penitent.Animator.SetFloat(this.SPEED_PARAM, 1f);
		}

		[Range(1f, 2f)]
		public float AnimatorSpeed = 1.1f;

		private string SPEED_PARAM = "HEALING_SPEED_MULTIPLIER";
	}
}
