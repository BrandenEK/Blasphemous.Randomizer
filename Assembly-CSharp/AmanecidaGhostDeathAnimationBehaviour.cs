using System;
using Gameplay.GameControllers.Enemies.Projectiles;
using UnityEngine;

public class AmanecidaGhostDeathAnimationBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.parriableProjectile == null)
		{
			this.parriableProjectile = animator.GetComponentInParent<ParriableProjectile>();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.normalizedTime >= 0.9f)
		{
			this.parriableProjectile.OnDeathAnimation();
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	private ParriableProjectile parriableProjectile;
}
