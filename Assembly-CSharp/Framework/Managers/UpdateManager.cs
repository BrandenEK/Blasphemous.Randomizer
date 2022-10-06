using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.FrameworkCore;
using UnityEngine;

namespace Framework.Managers
{
	public class UpdateManager : GameSystem
	{
		public override void Initialize()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			this.LoadBundles();
		}

		private void LoadBundles()
		{
			try
			{
				string path = (!Application.isEditor) ? "/Updates/" : (Application.dataPath + "/../obj/Updates/StandaloneWindows/");
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				FileInfo[] files = directoryInfo.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					if (fileInfo.Extension == string.Empty)
					{
						AssetBundle item = AssetBundle.LoadFromFile(fileInfo.DirectoryName + "/" + fileInfo.Name);
						this.bundles.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}

		private void LoadLevelContent(string scene)
		{
			foreach (AssetBundle assetBundle in this.bundles)
			{
				foreach (string text in assetBundle.GetAllAssetNames())
				{
					string text2 = text.Split(new char[]
					{
						'/'
					}).Last<string>().Split(new char[]
					{
						'.'
					}).First<string>();
					if (text2.Contains(scene.ToLower()))
					{
						GameObject gameObject = assetBundle.LoadAsset(text2) as GameObject;
						Object.Instantiate<GameObject>(gameObject);
					}
				}
			}
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.LoadLevelContent(newLevel.LevelName);
		}

		public override void Dispose()
		{
			base.Dispose();
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		public const string EDITOR_BUNDLE_PATH = "/../obj/Updates/StandaloneWindows/";

		public const string GAME_BUNDLE_PATH = "/Updates/";

		private List<AssetBundle> bundles = new List<AssetBundle>();
	}
}
