using System;
using System.Collections.Generic;
using Framework.Managers;
using UnityEngine;

namespace Tools.UI
{
	public class InputEnableObjects : MonoBehaviour
	{
		private void Start()
		{
			Core.Input.JoystickPressed += this.RefreshLists;
			Core.Input.KeyboardPressed += this.RefreshLists;
			this.RefreshLists();
		}

		private void OnDestroy()
		{
			Core.Input.JoystickPressed -= this.RefreshLists;
			Core.Input.KeyboardPressed -= this.RefreshLists;
		}

		private void RefreshLists()
		{
			bool IsPad = Core.Input.ActiveControllerType == 2;
			this.keyboardControls.ForEach(delegate(GameObject p)
			{
				p.SetActive(!IsPad);
			});
			this.padControls.ForEach(delegate(GameObject p)
			{
				p.SetActive(IsPad);
			});
		}

		public List<GameObject> keyboardControls = new List<GameObject>();

		public List<GameObject> padControls = new List<GameObject>();
	}
}
