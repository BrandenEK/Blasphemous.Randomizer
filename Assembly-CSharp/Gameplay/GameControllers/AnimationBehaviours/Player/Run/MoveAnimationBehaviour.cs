using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Dust;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Run
{
	public class MoveAnimationBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.speed = 1f;
			animator.SetBool("IS_CLIMBING_LADDER", false);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.IsGrabbingLadder = false;
			if (this._stepDustSpawner == null)
			{
				this._stepDustSpawner = this._penitent.StepDustSpawner;
			}
			this._stepDustSpawner.CurrentStepDustSpawn = this.stepDustType;
		}

		public StepDust.StepDustType stepDustType;

		private StepDustSpawner _stepDustSpawner;

		private Penitent _penitent;
	}
}
