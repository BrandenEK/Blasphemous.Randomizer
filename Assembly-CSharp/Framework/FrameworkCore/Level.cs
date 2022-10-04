using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using UnityEngine;

namespace Framework.FrameworkCore
{
	public class Level
	{
		public Level(string name, bool bundle = true)
		{
			this.CurrentStatus = LevelManager.LevelStatus.Unloaded;
			this.IsBundle = bundle;
			this.LevelName = name;
			this.scenes = new Dictionary<string, LevelScene>();
			this.Distance = 1;
		}

		public string LevelName { get; private set; }

		public LevelManager.LevelStatus CurrentStatus { get; private set; }

		public bool IsBundle { get; private set; }

		public int Distance { get; set; }

		public LevelScene GetLogicScene()
		{
			return this.scenes["LOGIC"];
		}

		public IEnumerator Load(bool streaming)
		{
			if (this.CurrentStatus != LevelManager.LevelStatus.Unloaded)
			{
				Debug.LogError("Level LOAD, name: " + this.LevelName + " -> Try to load with status " + this.CurrentStatus.ToString());
				yield break;
			}
			this.CurrentStatus = LevelManager.LevelStatus.Loading;
			if (this.IsBundle)
			{
				foreach (string section in Level.sections)
				{
					string sceneName = this.LevelName + "_" + section;
					LevelScene scene = new LevelScene(sceneName, section);
					this.scenes[section] = scene;
					if (section != "LOGIC")
					{
						yield return scene.Load();
						if (streaming)
						{
							scene.DeActivate();
						}
					}
				}
			}
			else
			{
				LevelScene scene2 = new LevelScene(this.LevelName, "LOGIC");
				this.scenes["LOGIC"] = scene2;
				yield return scene2.Load();
				if (streaming)
				{
					scene2.DeActivate();
				}
			}
			this.CurrentStatus = LevelManager.LevelStatus.Loaded;
			yield break;
		}

		public IEnumerator UnLoad()
		{
			if (this.CurrentStatus != LevelManager.LevelStatus.Loaded && this.CurrentStatus != LevelManager.LevelStatus.Deactivated)
			{
				Debug.LogError("Level UNLOAD, name: " + this.LevelName + " -> Try to unload with status " + this.CurrentStatus.ToString());
				yield break;
			}
			this.CurrentStatus = LevelManager.LevelStatus.Unloading;
			foreach (LevelScene scene in this.scenes.Values)
			{
				if (scene.Section == "LOGIC")
				{
					if (scene.CurrentStatus == LevelManager.LevelStatus.Activated)
					{
						yield return scene.Unload();
					}
				}
				else
				{
					yield return scene.Unload();
				}
			}
			this.CurrentStatus = LevelManager.LevelStatus.Unloaded;
			yield break;
		}

		public IEnumerator Activate()
		{
			if (this.CurrentStatus != LevelManager.LevelStatus.Loaded && this.CurrentStatus != LevelManager.LevelStatus.Deactivated)
			{
				Debug.LogError("Level ACTIVATE, name: " + this.LevelName + " -> Try to activate with status " + this.CurrentStatus.ToString());
				yield break;
			}
			this.CurrentStatus = LevelManager.LevelStatus.Activating;
			foreach (LevelScene scene in this.scenes.Values)
			{
				if (scene.Section == "LOGIC" && scene.CurrentStatus == LevelManager.LevelStatus.Unloaded)
				{
					yield return scene.Load();
				}
				yield return scene.Activate();
			}
			Resources.UnloadUnusedAssets();
			this.CurrentStatus = LevelManager.LevelStatus.Activated;
			yield break;
		}

		public IEnumerator DeActivate()
		{
			if (this.CurrentStatus != LevelManager.LevelStatus.Activated)
			{
				Debug.LogError("Level DEACTIVATE, name: " + this.LevelName + " -> Try to deactivate with status " + this.CurrentStatus.ToString());
				yield break;
			}
			this.CurrentStatus = LevelManager.LevelStatus.Deactivating;
			foreach (LevelScene scene in this.scenes.Values)
			{
				if (scene.Section == "LOGIC")
				{
					yield return scene.Unload();
				}
				else
				{
					scene.DeActivate();
					yield return 0;
				}
			}
			this.CurrentStatus = LevelManager.LevelStatus.Deactivated;
			yield break;
		}

		public const string LOGIC_SECTION = "LOGIC";

		public const string MAIN_SECTION = "MAIN";

		public static string[] sections = new string[]
		{
			"LAYOUT",
			"DECO",
			"AUDIO",
			"MAIN",
			"LOGIC"
		};

		private Dictionary<string, LevelScene> scenes;
	}
}
