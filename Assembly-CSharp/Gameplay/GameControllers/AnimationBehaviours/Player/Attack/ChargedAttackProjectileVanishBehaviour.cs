using System;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class ChargedAttackProjectileVanishBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			animator.GetComponent<ChargedAttackProjectile>().Dispose();
		}
	}
}
