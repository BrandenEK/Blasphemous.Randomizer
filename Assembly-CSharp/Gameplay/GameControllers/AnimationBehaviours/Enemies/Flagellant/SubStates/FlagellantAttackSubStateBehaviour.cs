using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Flagellant.SubStates
{
	public class FlagellantAttackSubStateBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant == null)
			{
				this._flagellant = animator.GetComponentInParent<Entity>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._flagellant.EntityAttack.IsWeaponBlowingUp = false;
		}

		private Entity _flagellant;
	}
}
