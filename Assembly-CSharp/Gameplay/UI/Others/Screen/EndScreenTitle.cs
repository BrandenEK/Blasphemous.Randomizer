using System;
using Gameplay.UI.Widgets;
using UnityEngine;

namespace Gameplay.UI.Others.Screen
{
	public class EndScreenTitle : MonoBehaviour
	{
		public void EnableSceneBody()
		{
			EndScreenWidget.instance.EnableEndSceneBody();
		}
	}
}
