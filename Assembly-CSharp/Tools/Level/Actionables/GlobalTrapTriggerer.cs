using System;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class GlobalTrapTriggerer : MonoBehaviour
	{
		public void Awake()
		{
			if (this.trapManager == null)
			{
				this.trapManager = Object.FindObjectOfType<TriggerTrapManager>();
			}
		}

		public void TriggerAllTrapsInTheScene()
		{
			this.trapManager.Trigger(this.TriggerTrapID);
		}

		public TriggerTrapManager trapManager;

		public string TriggerTrapID = "SHOCK";
	}
}
