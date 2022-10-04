using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.GameControllers.Environment
{
	public class GateManager : MonoBehaviour
	{
		private void Start()
		{
		}

		private void Update()
		{
		}

		protected Scene[] getAllScenesInBuild()
		{
			int sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;
			Scene[] array = new Scene[sceneCountInBuildSettings];
			for (int i = 0; i < sceneCountInBuildSettings; i++)
			{
				array[i] = SceneManager.GetSceneByBuildIndex(i);
			}
			return array;
		}
	}
}
