using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Stoners;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Stoner
{
	public class StonerIdleBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._stoner == null)
			{
				this._stoner = animator.GetComponentInParent<Stoners>();
			}
		}

		private Stoners _stoner;

		public EntityOrientation CurrentAnimationOrientation;
	}
}
