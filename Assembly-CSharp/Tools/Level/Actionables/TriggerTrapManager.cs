using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class TriggerTrapManager : MonoBehaviour
	{
		[Button(0)]
		public void LinkToSceneTraps()
		{
			this.traps = new List<TriggerBasedTrap>(Object.FindObjectsOfType<TriggerBasedTrap>());
		}

		public float GetSceneTrapLapse()
		{
			return this.loopTrapLapse;
		}

		public float GetFirstTrapLapse()
		{
			return this.firstTrapLapse;
		}

		public void Trigger(string id)
		{
			List<TriggerBasedTrap> list = this.traps.FindAll((TriggerBasedTrap x) => x.triggerID == id);
			foreach (TriggerBasedTrap triggerBasedTrap in list)
			{
				triggerBasedTrap.Use();
			}
		}

		public List<TriggerBasedTrap> traps;

		[SerializeField]
		[FoldoutGroup("Trap configuration", 0)]
		private float firstTrapLapse = 2f;

		[SerializeField]
		[FoldoutGroup("Trap configuration", 0)]
		private float loopTrapLapse = 5f;
	}
}
