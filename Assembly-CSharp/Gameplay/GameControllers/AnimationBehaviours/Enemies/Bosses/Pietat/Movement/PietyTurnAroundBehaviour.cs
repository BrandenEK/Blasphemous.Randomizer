using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Bosses.PietyMonster;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.Movement
{
	public class PietyTurnAroundBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._pietyMonster == null)
			{
				this._pietyMonster = animator.GetComponentInParent<PietyMonster>();
			}
			if (!this._pietyMonster.EnemyBehaviour.TurningAround)
			{
				this._pietyMonster.EnemyBehaviour.TurningAround = true;
			}
			if (this._pietyMonster.CanMove)
			{
				this._pietyMonster.CanMove = false;
			}
			this._pietatCurrentOrientation = this._pietyMonster.Status.Orientation;
			this._newEntityOrientation = ((this._pietatCurrentOrientation != EntityOrientation.Left) ? EntityOrientation.Left : EntityOrientation.Right);
			this._pietyMonster.SetOrientation(this._newEntityOrientation, false, false);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._pietyMonster.EnemyBehaviour.TurningAround)
			{
				this._pietyMonster.EnemyBehaviour.TurningAround = false;
			}
			this._pietyMonster.SetOrientation(this._newEntityOrientation, true, false);
			animator.SetBool("TURN_AROUND", false);
		}

		private PietyMonster _pietyMonster;

		private EntityOrientation _pietatCurrentOrientation;

		private EntityOrientation _newEntityOrientation;
	}
}
