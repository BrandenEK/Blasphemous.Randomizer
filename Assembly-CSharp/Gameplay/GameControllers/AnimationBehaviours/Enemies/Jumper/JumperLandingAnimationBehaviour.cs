using System;
using Gameplay.GameControllers.Enemies.Jumper;
using Gameplay.GameControllers.Enemies.Jumper.AI;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Jumper
{
	public class JumperLandingAnimationBehaviour : StateMachineBehaviour
	{
		public Jumper Jumper { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Jumper == null)
			{
				this.Jumper = animator.GetComponentInParent<Jumper>();
			}
			JumperBehaviour jumperBehaviour = (JumperBehaviour)this.Jumper.EnemyBehaviour;
			jumperBehaviour.StopMovement();
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			JumperBehaviour jumperBehaviour = (JumperBehaviour)this.Jumper.EnemyBehaviour;
			jumperBehaviour.LookAtTarget(this.Jumper.Target.transform.position);
			jumperBehaviour.IsJumping = false;
		}
	}
}
