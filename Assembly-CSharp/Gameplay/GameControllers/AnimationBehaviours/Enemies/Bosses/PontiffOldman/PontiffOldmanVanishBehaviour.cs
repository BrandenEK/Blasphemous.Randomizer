using System;
using Gameplay.GameControllers.Bosses.PontiffOldman;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.PontiffOldman
{
	public class PontiffOldmanVanishBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			PontiffOldman componentInParent = animator.GetComponentInParent<PontiffOldman>();
			componentInParent.Behaviour.OnVanishEnds();
		}
	}
}
