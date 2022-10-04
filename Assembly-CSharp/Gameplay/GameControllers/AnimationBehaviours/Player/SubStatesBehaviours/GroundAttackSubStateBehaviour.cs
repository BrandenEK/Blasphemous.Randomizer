using System;
using System.Collections;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.SubStatesBehaviours
{
	public class GroundAttackSubStateBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = animator.GetComponentInParent<Penitent>();
			}
			Singleton<Core>.Instance.StartCoroutine(this.DisableClimbAbility());
			this.RaiseAttackEvent(animator);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			animator.speed = ((this._penitent.PenitentAttack.AttackSpeed <= 1f) ? 1f : this._penitent.PenitentAttack.AttackSpeed);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			animator.speed = 1f;
			this._penitent.PenitentAttackAnimations.CloseAttackWindow();
			this._penitent.PenitentAttack.ClearHitEntityList();
		}

		private IEnumerator DisableClimbAbility()
		{
			if (this._isRunningClimbCoroutine)
			{
				yield break;
			}
			this._isRunningClimbCoroutine = true;
			this._penitent.GrabLadder.EnableClimbLadderAbility(false);
			yield return this._climbWaiting;
			this._penitent.GrabLadder.EnableClimbLadderAbility(true);
			this._isRunningClimbCoroutine = false;
			yield break;
		}

		private void RaiseAttackEvent(Animator animator)
		{
			if (!this.IsRunningParry(animator) && !this.IsRunningGuardSlide(animator))
			{
				this._penitent.AnimatorInyector.RaiseAttackEvent();
			}
		}

		private bool IsRunningParry(Animator animator)
		{
			Parry parry = Core.Logic.Penitent.Parry;
			bool flag = animator.GetCurrentAnimatorStateInfo(0).IsName("ParryFailed") || parry.IsRunningParryAnim;
			bool casting = Core.Logic.Penitent.Parry.Casting;
			return flag || casting;
		}

		private bool IsRunningGuardSlide(Animator animator)
		{
			return animator.GetCurrentAnimatorStateInfo(0).IsName("GuardToIdle") || animator.GetCurrentAnimatorStateInfo(0).IsName("GuardSlide");
		}

		private Penitent _penitent;

		private bool _isRunningClimbCoroutine;

		private WaitForSeconds _climbWaiting = new WaitForSeconds(0.42f);
	}
}
