using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.ClimbLadder
{
	public class ReleaseTopLadderBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.Physics.EnablePhysics(false);
			animator.speed = 1f;
			this._penitent.DamageArea.EnableEnemyAttack(false);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.Physics.EnablePhysics(true);
			Vector2 v = new Vector2(this._penitent.transform.position.x, this._penitent.transform.position.y - this._penitent.PlatformCharacterController.GroundDist);
			this._penitent.transform.position = v;
			this._penitent.IsGrabbingLadder = false;
			this._penitent.DamageArea.EnableEnemyAttack(true);
		}

		private Penitent _penitent;
	}
}
