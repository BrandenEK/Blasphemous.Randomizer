using System;
using Gameplay.GameControllers.Entities.Guardian;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Prayers.GuardianPrayer
{
	public class GuardianPrayerTurnBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.guardian == null)
			{
				this.guardian = animator.GetComponentInParent<GuardianPrayer>();
			}
			this.guardian.Behaviour.IsTurning = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.guardian.SetOrientation(this.guardian.Behaviour.GuessedOrientation, true, false);
			this.guardian.Behaviour.IsTurning = false;
		}

		private GuardianPrayer guardian;
	}
}
