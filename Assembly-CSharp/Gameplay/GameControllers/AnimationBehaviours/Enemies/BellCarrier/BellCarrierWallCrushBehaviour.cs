using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.BellCarrier;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellCarrier
{
	public class BellCarrierWallCrushBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._bellCarrier == null)
			{
				this._bellCarrier = animator.GetComponentInParent<BellCarrier>();
			}
			float num = (this._bellCarrier.Status.Orientation != EntityOrientation.Right) ? 1f : -1f;
			this._bellCarrier.MotionLerper.StartLerping(Vector2.right * num);
			this._bellCarrier.BellCarrierBehaviour.WallHit = true;
			animator.ResetTrigger("WALL_CRUSH");
		}

		private BellCarrier _bellCarrier;
	}
}
