using System;
using Gameplay.GameControllers.Bosses.BurntFace;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BurntFace
{
	public class BurntFaceDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			BurntFace componentInParent = animator.GetComponentInParent<BurntFace>();
			if (componentInParent != null)
			{
				UnityEngine.Object.Destroy(componentInParent.gameObject);
			}
		}
	}
}
