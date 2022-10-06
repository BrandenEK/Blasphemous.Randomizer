using System;
using UnityEngine;

namespace Tools.Level.Actionables
{
	[ExecuteInEditMode]
	public class TriggerTrapManagerAutogenerator : MonoBehaviour
	{
		private void Start()
		{
			if (!this.executedFlag)
			{
				TriggerTrapManager triggerTrapManager = Object.FindObjectOfType<TriggerTrapManager>();
				if (triggerTrapManager == null)
				{
					GameObject gameObject = new GameObject("----------TRIGGER TRAP MANAGER----------(auto-generated)");
					TriggerTrapManager triggerTrapManager2 = gameObject.AddComponent<TriggerTrapManager>();
					triggerTrapManager2.LinkToSceneTraps();
				}
				this.executedFlag = true;
			}
		}

		[SerializeField]
		[HideInInspector]
		private bool executedFlag;
	}
}
