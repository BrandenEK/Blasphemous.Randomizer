using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.SubStatesBehaviours
{
	public class LadderClimbingSubStateBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._penitent.IsFallingStunt)
			{
				this._penitent.IsFallingStunt = !this._penitent.IsFallingStunt;
			}
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.VSpeed = 0f;
			this._penitent.PlatformCharacterController.InstantVelocity = Vector3.zero;
			animator.SetBool("AIR_ATTACKING", false);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.DamageArea.IncludeEnemyLayer(false);
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Acceleration = Vector3.zero;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.DamageArea.IncludeEnemyLayer(true);
		}

		private Penitent _penitent;
	}
}
