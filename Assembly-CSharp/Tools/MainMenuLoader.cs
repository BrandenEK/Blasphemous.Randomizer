using System;
using Framework.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools
{
	[DefaultExecutionOrder(2)]
	public class MainMenuLoader : MonoBehaviour
	{
		private void Start()
		{
			bool flag = !Core.ready;
			string sceneName = "MainMenu_LOGIC";
			if (flag)
			{
				SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
			}
		}
	}
}
