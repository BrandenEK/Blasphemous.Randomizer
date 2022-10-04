using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Menu
{
	public class MapMenuBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			this._inputAlreadyBlocked = Core.Input.HasBlocker("PLAYER_LOGIC");
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			Core.Input.SetBlocker("PLAYER_LOGIC", this._inputAlreadyBlocked);
		}

		private bool _inputAlreadyBlocked;
	}
}
