using System;
using System.Collections;
using Framework.Managers;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.FrameworkCore
{
	public class LevelScene
	{
		public LevelScene(string name, string section)
		{
			this.SceneName = name;
			this.Section = section;
			this.InOperation = false;
			this.CurrentStatus = LevelManager.LevelStatus.Unloaded;
		}

		public string SceneName { get; private set; }

		public string Section { get; private set; }

		public Scene Scene { get; private set; }

		public LevelManager.LevelStatus CurrentStatus { get; private set; }

		public bool InOperation { get; private set; }

		public IEnumerator Load()
		{
			if (this.CurrentStatus != LevelManager.LevelStatus.Unloaded)
			{
				Debug.LogError("Scene LOAD, name: " + this.SceneName + " -> Try to load with status " + this.CurrentStatus.ToString());
				yield break;
			}
			if (this.CheckSceneLoadedInEditor())
			{
				this.Scene = SceneManager.GetSceneByName(this.SceneName);
				this.fistTimeLoaded = false;
				this.CurrentStatus = LevelManager.LevelStatus.Loaded;
				yield break;
			}
			this.InOperation = true;
			this.fistTimeLoaded = false;
			this.operation = SceneManager.LoadSceneAsync(this.SceneName, LoadSceneMode.Additive);
			while (!this.operation.isDone)
			{
				yield return null;
			}
			this.CurrentStatus = LevelManager.LevelStatus.Loaded;
			this.Scene = SceneManager.GetSceneByName(this.SceneName);
			this.InOperation = false;
			yield break;
		}

		public IEnumerator Unload()
		{
			if (this.CurrentStatus == LevelManager.LevelStatus.Unloaded)
			{
				Debug.LogError("Scene UNLOAD, name: " + this.SceneName + " -> Try to unload with status " + this.CurrentStatus.ToString());
				yield break;
			}
			this.InOperation = true;
			this.operation = SceneManager.UnloadSceneAsync(this.Scene.name);
			yield return this.operation;
			this.CurrentStatus = LevelManager.LevelStatus.Unloaded;
			this.InOperation = false;
			yield break;
		}

		public IEnumerator Activate()
		{
			if (this.CurrentStatus != LevelManager.LevelStatus.Loaded && this.CurrentStatus != LevelManager.LevelStatus.Deactivated)
			{
				Debug.LogError("Scene ACTIVATE, name: " + this.SceneName + " -> Try to activate with status " + this.CurrentStatus.ToString());
				yield break;
			}
			this.InOperation = true;
			if (this.fistTimeLoaded)
			{
				this.fistTimeLoaded = false;
				this.operation.allowSceneActivation = true;
				yield return new WaitForEndOfFrame();
				yield return null;
				yield return this.operation;
				this.Scene = SceneManager.GetSceneByName(this.SceneName);
			}
			else
			{
				try
				{
					this.Scene.GetRootGameObjects().ForEach(delegate(GameObject obj)
					{
						obj.SetActive(true);
					});
				}
				catch (Exception ex)
				{
					Debug.LogError("Error Activating level");
					Debug.LogError(ex.Message);
				}
			}
			this.CurrentStatus = LevelManager.LevelStatus.Activated;
			this.InOperation = false;
			yield break;
		}

		public void DeActivate()
		{
			this.InOperation = true;
			if (this.CurrentStatus != LevelManager.LevelStatus.Activated && this.CurrentStatus != LevelManager.LevelStatus.Loaded)
			{
				Debug.LogError("Scene DEACTIVATE, name: " + this.SceneName + " -> Try to deactivate with status " + this.CurrentStatus.ToString());
				return;
			}
			try
			{
				this.Scene.GetRootGameObjects().ForEach(delegate(GameObject obj)
				{
					obj.SetActive(false);
				});
			}
			catch (Exception ex)
			{
				Debug.LogError("Error Deactivating level");
				Debug.LogError(ex.Message);
			}
			this.CurrentStatus = LevelManager.LevelStatus.Deactivated;
			this.InOperation = false;
		}

		private bool CheckSceneLoadedInEditor()
		{
			bool result = false;
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.name == this.SceneName)
				{
					this.Scene = sceneAt;
					result = true;
					break;
				}
			}
			return result;
		}

		private AsyncOperation operation;

		private bool fistTimeLoaded = true;
	}
}
