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
				TriggerTrapManager x = UnityEngine.Object.FindObjectOfType<TriggerTrapManager>();
				if (x == null)
				{
					GameObject gameObject = new GameObject("----------TRIGGER TRAP MANAGER----------(auto-generated)");
					TriggerTrapManager triggerTrapManager = gameObject.AddComponent<TriggerTrapManager>();
					triggerTrapManager.LinkToSceneTraps();
				}
				this.executedFlag = true;
			}
		}

		[SerializeField]
		[HideInInspector]
		private bool executedFlag;
	}
}
