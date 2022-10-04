using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Menina;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Menina
{
	public class MeninaWalkBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._menina == null)
			{
				this._menina = animator.GetComponentInParent<Menina>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			float normalizedTime = stateInfo.normalizedTime;
			float num = normalizedTime - Mathf.Floor(normalizedTime);
			if (num > 0.9f)
			{
				this._menina.Inputs.HorizontalInput = 0f;
				this._menina.Controller.PlatformCharacterPhysics.HSpeed = 0f;
				this._menina.AnimatorInyector.NotifyOnStepFinished();
			}
			else
			{
				this.Step();
			}
		}

		private void Step()
		{
			if (this._menina.Animator.GetBool("STEP_FWD"))
			{
				this._menina.Inputs.HorizontalInput = ((this._menina.Status.Orientation != EntityOrientation.Right) ? -1f : 1f);
			}
			else if (this._menina.Animator.GetBool("STEP_BCK"))
			{
				this._menina.Inputs.HorizontalInput = ((this._menina.Status.Orientation != EntityOrientation.Right) ? 1f : -1f);
			}
		}

		private Menina _menina;
	}
}
