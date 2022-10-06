using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Runner;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Runner
{
	public class RunnerTurnAroundBehaviour : StateMachineBehaviour
	{
		public Runner Runner { get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Runner == null)
			{
				this.Runner = animator.GetComponentInParent<Runner>();
			}
			this.Runner.Behaviour.TurningAround = true;
			this._turned = false;
			this._throwScream = false;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime > 0.5f)
			{
				this.SetOrientation();
			}
			if (stateInfo.normalizedTime > 0.9f)
			{
				this.ThrowScream();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Runner.Behaviour.TurningAround = false;
			animator.SetBool("TURNING", false);
			this.Runner.SpriteRenderer.flipX = (this.Runner.Status.Orientation == EntityOrientation.Left);
		}

		private void SetOrientation()
		{
			if (this._turned)
			{
				return;
			}
			this._turned = true;
			EntityOrientation orientation = this.Runner.Status.Orientation;
			this.Runner.SetOrientation((orientation != EntityOrientation.Left) ? EntityOrientation.Left : EntityOrientation.Right, false, false);
		}

		private void ThrowScream()
		{
			if (this._throwScream)
			{
				return;
			}
			this._throwScream = true;
			if (Random.value > 0.3f)
			{
				return;
			}
			this._throwScream = true;
			this.Runner.Behaviour.Scream();
		}

		private bool _turned;

		private bool _throwScream;
	}
}
