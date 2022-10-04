using System;
using Framework.Managers;
using Gameplay.GameControllers.AnimationBehaviours.Player.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.SubStatesBehaviours
{
	public class HurtSubStateBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))
			{
				animator.Play("Idle");
			}
			if (!this._penitent)
			{
				this._penitent = Core.Logic.Penitent;
				this._throwBack = this._penitent.GetComponentInChildren<ThrowBack>();
			}
			this._penitent.Status.IsHurt = true;
			if (this._startChargingAttackBehaviour == null)
			{
				this._startChargingAttackBehaviour = animator.GetBehaviour<StartChargingAttackBehaviour>();
			}
			this._penitent.Audio.StopLoadingChargedAttack();
			if (this._penitent.IsChargingAttack)
			{
				this._penitent.IsChargingAttack = !this._penitent.IsChargingAttack;
			}
			this._penitent.IsStickedOnWall = false;
			this._penitent.Dash.CrouchAfterDash = false;
			this._penitent.Animator.speed = 1f;
			animator.SetBool(HurtSubStateBehaviour.CrouchAttacking, false);
			this._penitent.Audio.StopUseFlask();
			if (this._throwBack.Casting || animator.GetBool(HurtSubStateBehaviour.Throw))
			{
				return;
			}
			if (!Core.LevelManager.currentLevel.LevelName.Equals("D24Z01S01") || Core.Logic.Penitent.CurrentLife != 1f)
			{
				this._penitent.Physics.EnablePhysics(true);
				this._penitent.Physics.EnableColliders(true);
				this._penitent.Physics.Enable2DCollision(true);
				this._penitent.GrabLadder.EnableClimbLadderAbility(false);
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.IsDashing)
			{
				this._penitent.Dash.StopCast();
			}
			this._penitent.GrabLadder.EnableClimbLadderAbility(false);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.Status.IsHurt = false;
			this._penitent.IsClimbingCliffLede = false;
			this._penitent.IsGrabbingCliffLede = false;
			this._penitent.IsGrabbingLadder = false;
			this._penitent.GrabLadder.EnableClimbLadderAbility(true);
		}

		private Penitent _penitent;

		private StartChargingAttackBehaviour _startChargingAttackBehaviour;

		private ThrowBack _throwBack;

		private static readonly int CrouchAttacking = Animator.StringToHash("CROUCH_ATTACKING");

		private static readonly int Throw = Animator.StringToHash("THROW");
	}
}
