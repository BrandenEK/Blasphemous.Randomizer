using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.ClimbLadder
{
	public class GrabLadderDownBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._rootMotionDriver = this._penitent.GetComponentInChildren<RootMotionDriver>();
			}
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			this._penitent.IsJumpingOff = false;
			this._penitent.IsGrabbingLadder = true;
			this._penitent.GrabLadder.SetClimbingSpeed(0f);
			this._penitent.DamageArea.EnableEnemyAttack(false);
			if (this._penitent.IsCrouched)
			{
				this._penitent.IsCrouched = !this._penitent.IsCrouched;
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime > 0.9f && !this._penitent.StartingGoingDownLadders)
			{
				this._penitent.IsClimbingLadder = true;
				this._penitent.DamageArea.EnableEnemyAttack(true);
				this._penitent.GrabLadder.SetClimbingSpeed(2.25f);
				this.SetRootMotionPosition(delegate
				{
					Core.Input.SetBlocker("PLAYER_LOGIC", false);
					animator.Play("ladder_going_down");
				});
			}
		}

		protected void SetRootMotionPosition(Action callback = null)
		{
			this._penitent.RootMotionDrive = new Vector2(this._rootMotionDriver.transform.position.x, this._rootMotionDriver.transform.position.y);
			this._penitent.transform.position = this._penitent.RootMotionDrive;
			this._penitent.StartingGoingDownLadders = true;
			if (callback != null)
			{
				callback();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (Core.Input.HasBlocker("PLAYER_LOGIC"))
			{
				Core.Input.SetBlocker("PLAYER_LOGIC", false);
			}
		}

		private Penitent _penitent;

		private RootMotionDriver _rootMotionDriver;
	}
}
