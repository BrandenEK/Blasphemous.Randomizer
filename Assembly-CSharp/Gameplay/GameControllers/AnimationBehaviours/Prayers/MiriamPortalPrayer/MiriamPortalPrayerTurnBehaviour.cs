using System;
using Gameplay.GameControllers.Entities.MiriamPortal;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Prayers.MiriamPortalPrayer
{
	public class MiriamPortalPrayerTurnBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.miriamPortal == null)
			{
				this.miriamPortal = animator.GetComponentInParent<MiriamPortalPrayer>();
			}
			this.miriamPortal.Behaviour.IsTurning = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.miriamPortal.SetOrientation(this.miriamPortal.Behaviour.GuessedOrientation, true, false);
			this.miriamPortal.Behaviour.IsTurning = false;
		}

		private MiriamPortalPrayer miriamPortal;
	}
}
