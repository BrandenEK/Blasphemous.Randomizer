using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Flagellant;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Flagellant
{
	public class FlagellantAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant == null)
			{
				this._flagellant = animator.GetComponentInParent<Flagellant>();
			}
			this._normalizedTime = 0f;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant == null)
			{
				return;
			}
			this._normalizedTime = stateInfo.normalizedTime - (float)Mathf.FloorToInt(stateInfo.normalizedTime);
			if (this._normalizedTime < 0.1f)
			{
				this.LookAtTarget(this._flagellant.Target.transform.position);
			}
			if (!this._flagellant.IsAttacking)
			{
				this._flagellant.IsAttacking = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant == null)
			{
				return;
			}
			if (this._flagellant.IsAttacking)
			{
				this._flagellant.IsAttacking = !this._flagellant.IsAttacking;
			}
		}

		private void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this._flagellant.transform.position.x)
			{
				if (this._flagellant.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this._flagellant.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this._flagellant.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this._flagellant.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		private Flagellant _flagellant;

		private float _normalizedTime;
	}
}
