using System;
using System.Collections.Generic;
using System.IO;
using Framework.DLCs;
using Framework.FrameworkCore;
using UnityEngine;

namespace Framework.Managers
{
	public class DLCManager : GameSystem
	{
		public static List<string> GetDLCBaseDirectories()
		{
			if (DLCManager.cacheList.Count == 0)
			{
				DLCManager.cacheList.Add(Path.Combine(Application.streamingAssetsPath, "DLC"));
				DLCManager.cacheList.Add(Path.Combine(Application.dataPath, "DLC"));
				DLCManager.cacheList.Add(PersistentManager.GetPathAppSettings("DLC"));
			}
			return DLCManager.cacheList;
		}

		public override void Initialize()
		{
			this.helper = new TestDLCHelper();
			DLC[] array = Resources.LoadAll<DLC>("DLC/");
			this.allDLCs.Clear();
			foreach (DLC dlc in array)
			{
				dlc.CheckDownloaded();
				this.allDLCs[dlc.id] = dlc;
			}
			foreach (string text in DLCManager.GetDLCBaseDirectories())
			{
				Log.Debug("DLCManager", "Base DLC path: " + text, null);
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
			}
			Log.Debug("DLCManager", this.allDLCs.Count.ToString() + " DLCs loaded succesfully.", null);
			foreach (DLC dlc2 in array)
			{
				Log.Debug(string.Format("DLC: {0} is {1}.", dlc2.name, (!dlc2.IsDownloaded) ? "not present" : "present"), null);
			}
		}

		public bool IsFirstDLCInstalled()
		{
			return true;
		}

		public bool IsSecondDLCInstalled()
		{
			return false;
		}

		public bool IsThirdDLCInstalled()
		{
			return true;
		}

		public bool IsDLCDownloaded(string Id, bool recheck = false)
		{
			bool result = false;
			if (this.allDLCs.ContainsKey(Id))
			{
				if (recheck)
				{
					this.allDLCs[Id].CheckDownloaded();
				}
				result = this.allDLCs[Id].IsDownloaded;
			}
			return result;
		}

		public static List<string> GetAllDLCsIDFromEditor()
		{
			if (DLCManager.cachedDLCs.Count == 0)
			{
				DLC[] array = Resources.LoadAll<DLC>("DLC/");
				foreach (DLC dlc in array)
				{
					DLCManager.cachedDLCs.Add(dlc.id);
				}
			}
			return DLCManager.cachedDLCs;
		}

		public const string DLC_SUBFOLDER = "DLC";

		private IDLCHelper helper;

		private Dictionary<string, DLC> allDLCs = new Dictionary<string, DLC>();

		private static List<string> cacheList = new List<string>();

		private static List<string> cachedDLCs = new List<string>();
	}
}
