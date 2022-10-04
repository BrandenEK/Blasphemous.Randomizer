using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.FireTrap.AnimStatesScripts
{
	public class ElmFireLightningStateBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (!this._beamLauncher)
			{
				this._beamLauncher = animator.GetComponentInParent<TileableBeamLauncher>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._beamLauncher)
			{
				this._beamLauncher.gameObject.SetActive(false);
			}
		}

		private TileableBeamLauncher _beamLauncher;
	}
}
