using System;
using Gameplay.GameControllers.Enemies.Stoners;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Stoner
{
	public class StonersFlipBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._stoners == null)
			{
				this._stoners = animator.GetComponentInParent<Stoners>();
			}
			this._stoners.AnimatorInyector.AllowOrientation(false);
		}

		private Stoners _stoners;
	}
}
