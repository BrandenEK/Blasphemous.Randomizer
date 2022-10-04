using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.SubStatesBehaviours
{
	public class AirAttackSubStateBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.GrabLadder.EnableClimbLadderAbility(false);
			this._penitent.AnimatorInyector.RaiseAttackEvent();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.GrabLadder.EnableClimbLadderAbility(true);
			this._penitent.PenitentAttackAnimations.CloseAttackWindow();
			this._penitent.PenitentAttack.ClearHitEntityList();
		}

		private Penitent _penitent;
	}
}
