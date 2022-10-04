using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk.Animator
{
	public class PontiffHuskAnimatorBridge : MonoBehaviour
	{
		private void Start()
		{
			this._PontiffHuskRanged = base.GetComponentInParent<PontiffHuskRanged>();
			this._PontiffHuskMelee = base.GetComponentInParent<PontiffHuskMelee>();
		}

		public void PlayChargeAttack()
		{
			if (this._PontiffHuskRanged != null)
			{
				this._PontiffHuskRanged.Audio.ChargeAttack();
			}
			if (this._PontiffHuskMelee != null)
			{
				this._PontiffHuskMelee.Audio.ChargeAttack();
			}
		}

		private PontiffHuskRanged _PontiffHuskRanged;

		private PontiffHuskMelee _PontiffHuskMelee;
	}
}
