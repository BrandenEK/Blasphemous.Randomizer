using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Interactables
{
	public class ActivateIfDLCInstalled : MonoBehaviour
	{
		public void Awake()
		{
			if (this.activateOnlyIfDLC3 && this.activateOnlyIfDLC2)
			{
				Debug.LogError("ActivateIfDLCInstalled: both activateOnlyIfDLC3 and activateOnlyIfDLC2 shouldn't be true!");
			}
			else if (!this.activateOnlyIfDLC3 && !this.activateOnlyIfDLC2)
			{
				Debug.LogError("ActivateIfDLCInstalled: both activateOnlyIfDLC3 and activateOnlyIfDLC2 shouldn't be false!");
			}
			if (this.activateOnlyIfDLC3)
			{
				base.gameObject.SetActive(true);
			}
			else if (this.activateOnlyIfDLC2)
			{
				base.gameObject.SetActive(false);
			}
		}

		[HideIf("activateOnlyIfDLC2", true)]
		public bool activateOnlyIfDLC3 = true;

		[HideIf("activateOnlyIfDLC3", true)]
		public bool activateOnlyIfDLC2;
	}
}
