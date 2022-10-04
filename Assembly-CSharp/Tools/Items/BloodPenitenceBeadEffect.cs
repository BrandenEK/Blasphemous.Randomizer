using System;
using Framework.Inventory;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Items
{
	public class BloodPenitenceBeadEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			Core.PenitenceManager.AddFlasksPassiveHealthRegen(this.regenFactorIncrease);
			return true;
		}

		protected override void OnRemoveEffect()
		{
			Core.PenitenceManager.AddFlasksPassiveHealthRegen(-this.regenFactorIncrease);
		}

		[SerializeField]
		[BoxGroup("Values", true, false, 0)]
		private float regenFactorIncrease;
	}
}
