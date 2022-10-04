using System;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffGiant.Animator
{
	public class PontiffGiantAnimationEvents : MonoBehaviour
	{
		public void AnimationEvent_OnMaskClosed()
		{
			this.behaviour.OnMaskClosed();
		}

		public void AnimationEvent_OnMaskOpened()
		{
			this.behaviour.OnMaskOpened();
		}

		public PontiffGiantBehaviour behaviour;
	}
}
