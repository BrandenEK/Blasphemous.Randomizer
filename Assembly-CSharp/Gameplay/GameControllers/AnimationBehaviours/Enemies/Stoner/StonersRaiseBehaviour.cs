using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Stoners;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Stoner
{
	public class StonersRaiseBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._stoner == null)
			{
				this._stoner = animator.GetComponentInParent<Stoners>();
			}
			this._stoner.Status.Orientation = this.CurrentAnimationOrientation;
			if (this._stoner.StonerBehaviour.IsRaised)
			{
				return;
			}
			animator.speed = 0f;
		}

		private Stoners _stoner;

		public EntityOrientation CurrentAnimationOrientation;
	}
}
