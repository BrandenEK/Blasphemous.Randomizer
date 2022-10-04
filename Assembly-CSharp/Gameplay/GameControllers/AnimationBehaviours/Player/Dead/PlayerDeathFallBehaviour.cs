using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Dead
{
	public class PlayerDeathFallBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._penitent.Stats.Life.Current > 0f)
			{
				this._penitent.Kill();
			}
		}

		private Penitent _penitent;
	}
}
