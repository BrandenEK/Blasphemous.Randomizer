using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Dead
{
	public class PlayerDeathSpikeBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._penitent.IsImpaled)
			{
				this._penitent.IsImpaled = !this._penitent.IsImpaled;
			}
		}

		private Penitent _penitent;
	}
}
