using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.RangeAttack
{
	public class GroundRangeAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._rangeAttack = this._penitent.GetComponentInChildren<RangeAttack>();
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.HSpeed = 0f;
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			this._penitent.PenitentAttack.IsRangeAttacking = true;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			if (this._rangeAttack != null)
			{
				this._rangeAttack.StopCastRangeAttack();
			}
			this._penitent.PenitentAttack.IsRangeAttacking = false;
		}

		private Penitent _penitent;

		private RangeAttack _rangeAttack;
	}
}
