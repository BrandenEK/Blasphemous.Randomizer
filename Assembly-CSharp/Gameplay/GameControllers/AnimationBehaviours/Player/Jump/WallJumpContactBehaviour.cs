using System;
using Gameplay.GameControllers.Effects.Player.Dust;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Jump
{
	public class WallJumpContactBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			animator.SetBool("STICK_ON_WALL", true);
			if (this._penitent == null)
			{
				this._penitent = animator.GetComponentInParent<Penitent>();
				this._wallClimbDust = this._penitent.GetComponentInChildren<WallClimbDust>();
			}
			this._penitent.IsStickedOnWall = true;
			this._penitent.AnimatorInyector.ResetStuntByFall();
			this._wallClimbDust.TriggerDust();
		}

		private Penitent _penitent;

		private WallClimbDust _wallClimbDust;
	}
}
