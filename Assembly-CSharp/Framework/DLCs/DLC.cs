using System;
using System.IO;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.DLCs
{
	public class DLC : ScriptableObject
	{
		public bool IsDownloaded { get; private set; }

		public string FullPath { get; private set; }

		public void OnIdChanged(string value)
		{
			this.id = value.Replace(' ', '_').ToUpper();
		}

		public string GetFileName()
		{
			return this.assetBundle;
		}

		public void CheckDownloaded()
		{
			string fileName = this.GetFileName();
			foreach (string path in DLCManager.GetDLCBaseDirectories())
			{
				this.FullPath = Path.Combine(path, fileName);
				this.IsDownloaded = File.Exists(this.FullPath);
				if (this.IsDownloaded)
				{
					break;
				}
			}
		}

		[OnValueChanged("OnIdChanged", false)]
		[InfoBox("Common ID for all platforms", 1, null)]
		public string id = string.Empty;

		public string sortDescription = string.Empty;

		[InfoBox("ID of the DLC on various platforms", 1, null)]
		public uint steam_appid;

		public string gog_appid = string.Empty;

		public string epic_appid = string.Empty;

		[InfoBox("Assetbundle to download, load & check", 1, null)]
		public string assetBundle;

		[NonSerialized]
		public AssetBundle loadedBundle;
	}
}
