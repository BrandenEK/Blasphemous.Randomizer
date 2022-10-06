using System;
using UnityEngine;

namespace Tools.Util
{
	public class RandomFrame : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.previousState = stateInfo.fullPathHash;
			if (stateInfo.fullPathHash != this.previousState)
			{
				animator.Play(stateInfo.fullPathHash, 0, Random.Range(0f, 1f));
			}
		}

		private int previousState;
	}
}
