using System;
using System.Diagnostics;
using UnityEngine;

namespace Framework.Util
{
	public class AnimatorEvent : MonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event AnimatorEvent.AnimEvent OnEventLaunched;

		public void LaunchEvent(string eventParam)
		{
			if (this.OnEventLaunched != null)
			{
				this.OnEventLaunched(eventParam);
			}
		}

		public delegate void AnimEvent(string id);
	}
}
