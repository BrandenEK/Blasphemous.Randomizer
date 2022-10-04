using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Jumper;
using Gameplay.GameControllers.Enemies.Jumper.AI;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Jumper
{
	public class JumperReadyAnimationBehaviour : StateMachineBehaviour
	{
		public Jumper Jumper { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Jumper == null)
			{
				this.Jumper = animator.GetComponentInParent<Jumper>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			JumperBehaviour jumperBehaviour = (JumperBehaviour)this.Jumper.EnemyBehaviour;
			float horizontalInput = (this.Jumper.Target.transform.position.x <= this.Jumper.transform.position.x) ? -1f : 1f;
			this.Jumper.Inputs.HorizontalInput = horizontalInput;
			this.Jumper.SetOrientation(this.GetJumpOrientation(), true, false);
			jumperBehaviour.Jump();
		}

		private EntityOrientation GetJumpOrientation()
		{
			float horizontalInput = this.Jumper.Inputs.HorizontalInput;
			return (horizontalInput > 0f) ? EntityOrientation.Right : EntityOrientation.Left;
		}
	}
}
