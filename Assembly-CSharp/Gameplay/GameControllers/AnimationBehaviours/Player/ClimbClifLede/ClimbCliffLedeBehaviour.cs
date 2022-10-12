using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.ClimbClifLede
{
	public class ClimbCliffLedeBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.ResetTrigger("CLIMB_CLIFF_LEDGE");
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.IsGrabbingCliffLede = false;
			this._penitent.Physics.EnableColliders(false);
			this._penitent.transform.position = this.GetRootMotionCliffPosition(this._penitent.RootMotionDrive);
			this._penitent.Status.Unattacable = true;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.cliffLedeClimbingStarted = false;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.Status.Unattacable = false;
			this._penitent.IsClimbingCliffLede = false;
			this._penitent.Physics.EnablePhysics(true);
			this._penitent.Physics.EnableColliders(true);
		}

		private Vector3 GetRootMotionCliffPosition(Vector3 rootMotion)
		{
			Vector3 result = new Vector3(rootMotion.x, this._penitent.GrabCliffLede.CliffColliderBoundaries.max.y - 0.03125f, rootMotion.z);
			return result;
		}

		private Penitent _penitent;
	}
}
