using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.UI.Widgets
{
	public class HideUIOnCommand : SerializedMonoBehaviour
	{
		private void Start()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		private void OnLevelLoaded(Level oldlevel, Level newlevel)
		{
			this.EnableUiControls(this.lastValue);
		}

		private void Update()
		{
			bool flag;
			if (this.debugUI)
			{
				flag = Core.UI.ConsoleShowDebugUI;
			}
			else
			{
				flag = Core.UI.MustShowGamePlayUI();
			}
			if (flag != this.lastValue)
			{
				this.lastValue = flag;
				this.EnableUiControls(this.lastValue);
			}
		}

		private void EnableUiControls(bool enable)
		{
			this.controls.ForEach(delegate(GameObject p)
			{
				p.SetActive(enable);
			});
		}

		public bool debugUI;

		[SerializeField]
		private List<GameObject> controls = new List<GameObject>();

		private bool lastValue = true;
	}
}
