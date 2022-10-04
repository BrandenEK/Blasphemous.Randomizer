using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Bishop;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bishop
{
	public class BishopTurnBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._bishop == null)
			{
				this._bishop = animator.GetComponentInParent<Bishop>();
			}
			this._prevOrientation = this._bishop.Status.Orientation;
			this._bishop.EnemyBehaviour.TurningAround = true;
			EntityOrientation orientation = (this._prevOrientation != EntityOrientation.Left) ? EntityOrientation.Left : EntityOrientation.Right;
			this._bishop.SetOrientation(orientation, false, false);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._bishop.SetOrientation(this._bishop.Status.Orientation, true, false);
			this._bishop.EnemyBehaviour.TurningAround = false;
		}

		private Bishop _bishop;

		private EntityOrientation _prevOrientation;
	}
}
