using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Stoners;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Stoner
{
	public class StonersAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._stoner == null)
			{
				this._stoner = animator.GetComponentInParent<Stoners>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this._stoner.AnimatorInyector.AllowOrientation(this.CurrentAnimationOrientation != this._stoner.Status.Orientation);
		}

		private Stoners _stoner;

		public EntityOrientation CurrentAnimationOrientation;
	}
}
