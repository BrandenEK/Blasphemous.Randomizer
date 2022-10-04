using System;
using Gameplay.GameControllers.Bosses.PietyMonster;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.Movement
{
	public class PietyWalkBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._pietyMonster == null)
			{
				this._pietyMonster = animator.GetComponentInParent<PietyMonster>();
			}
			this._pietyMonster.CanMove = true;
			this._isFirstLoop = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (!this._isFirstLoop)
			{
				this._isFirstLoop = true;
				this._pietyMonster.AnimatorBridge.AllowWalkCameraShake = true;
			}
			bool flag = !animator.GetBool("WALK");
			bool @bool = animator.GetBool("TURN_AROUND");
			if (@bool)
			{
				if (this._pietyMonster.CanMove)
				{
					this._pietyMonster.CanMove = false;
				}
				animator.Play(this._walkToTurn);
			}
			if (stateInfo.normalizedTime <= 0.65f)
			{
				if (flag && !@bool)
				{
					if (this._pietyMonster.CanMove)
					{
						this._pietyMonster.CanMove = false;
					}
					animator.Play(this._walkToIdle1);
				}
			}
			else if (flag && !@bool)
			{
				if (this._pietyMonster.CanMove)
				{
					this._pietyMonster.CanMove = false;
				}
				animator.Play(this._walkToIdle2);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._pietyMonster.CanMove = false;
			this._pietyMonster.AnimatorBridge.AllowWalkCameraShake = false;
		}

		private PietyMonster _pietyMonster;

		private bool _isFirstLoop;

		private readonly int _walkToTurn = Animator.StringToHash("WalkToTurnAround");

		private readonly int _walkToIdle1 = Animator.StringToHash("WalkToIdle_1");

		private readonly int _walkToIdle2 = Animator.StringToHash("WalkToIdle_2");
	}
}
