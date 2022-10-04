using System;
using System.Collections;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Jump
{
	public class JumpOffBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._enable2DCollisionsLapse == null)
			{
				this._enable2DCollisionsLapse = new WaitForSeconds(0.1f);
			}
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._rootMotion == null)
			{
				this._rootMotion = this._penitent.GetComponentInChildren<RootMotionDriver>();
			}
			if (this._penitent.IsCrouchAttacking)
			{
				this._penitent.IsCrouchAttacking = !this._penitent.IsCrouchAttacking;
			}
			animator.SetBool("CROUCH_ATTACKING", false);
			this._penitent.Status.Invulnerable = true;
			this._penitent.Dash.enabled = false;
			this._penitent.Dash.SetDashSkinCollision();
			this._penitent.PlatformCharacterInput.ResetActions();
			this._penitent.PlatformCharacterInput.ResetInputs();
			this._penitent.PlatformCharacterController.PlatformCharacterPhysics.Velocity = Vector3.zero;
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.startedJumpOff = true;
			this._penitent.jumpOffRoot = new Vector2(this._penitent.transform.position.x, this._rootMotion.transform.position.y);
			if (!this._penitent.IsJumpingOff)
			{
				this._penitent.IsJumpingOff = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.Status.Invulnerable = false;
			this._penitent.DamageArea.EnableEnemyAttack(true);
			this._penitent.Dash.enabled = true;
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			Singleton<Core>.Instance.StartCoroutine(this.Enable2DPhysics());
		}

		private IEnumerator Enable2DPhysics()
		{
			yield return this._enable2DCollisionsLapse;
			this._penitent.Dash.SetDefaultSkinCollision();
			this._penitent.Physics.Enable2DCollision(true);
			yield break;
		}

		private Penitent _penitent;

		private RootMotionDriver _rootMotion;

		private WaitForSeconds _enable2DCollisionsLapse;
	}
}
