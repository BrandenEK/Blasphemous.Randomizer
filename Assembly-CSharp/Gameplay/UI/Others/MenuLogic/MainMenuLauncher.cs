using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class MainMenuLauncher : MonoBehaviour
	{
		private void Start()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			penitent.gameObject.SetActive(false);
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			string newInitialScene = (!this.OverwriteInitialScene) ? string.Empty : this.initialSceneName;
			UIController.instance.ShowMainMenu(newInitialScene);
		}

		[BoxGroup("InitialScene", true, false, 0)]
		[SerializeField]
		private bool OverwriteInitialScene;

		[BoxGroup("InitialScene", true, false, 0)]
		[ShowIf("OverwriteInitialScene", true)]
		private string initialSceneName = string.Empty;
	}
}
