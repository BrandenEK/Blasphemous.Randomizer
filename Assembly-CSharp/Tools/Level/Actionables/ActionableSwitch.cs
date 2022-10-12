using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class ActionableSwitch : MonoBehaviour, IActionable
	{
		public bool Locked { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ActionableSwitch> OnSwitchUsed;

		[Button(ButtonSizes.Small)]
		public void Use()
		{
			this.ChangeState(!this.isOn);
		}

		public void ChangeState(bool turnOn)
		{
			this.isOn = turnOn;
			UnityEngine.Debug.Log("SWITCH USED");
			if (this.OnSwitchUsed != null)
			{
				this.OnSwitchUsed(this);
			}
		}

		public bool isOn;
	}
}
