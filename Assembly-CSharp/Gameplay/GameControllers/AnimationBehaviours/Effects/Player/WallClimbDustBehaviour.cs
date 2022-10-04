using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Dust;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Effects.Player
{
	public class WallClimbDustBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._wallClimbDust = this._penitent.GetComponentInChildren<WallClimbDust>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._wallClimbDust.StoreClimbDust(animator.gameObject);
		}

		private Penitent _penitent;

		private WallClimbDust _wallClimbDust;
	}
}
