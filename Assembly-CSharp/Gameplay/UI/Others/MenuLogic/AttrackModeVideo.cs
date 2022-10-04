using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class AttrackModeVideo : MonoBehaviour
	{
		private void Update()
		{
			if (Input.anyKey)
			{
				Core.Logic.LoadMenuScene(true);
			}
		}
	}
}
