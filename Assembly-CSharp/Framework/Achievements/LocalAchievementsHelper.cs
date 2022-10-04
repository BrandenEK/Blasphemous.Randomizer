using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework.Managers;
using FullSerializer;
using Tools;
using UnityEngine;

namespace Framework.Achievements
{
	public class LocalAchievementsHelper : IAchievementsHelper
	{
		public void SetAchievementProgress(string id, float value)
		{
			PlayerPrefs.SetFloat(id, value);
		}

		public void GetAchievementProgress(string id, GetAchievementOperationEvent evt)
		{
			float @float = PlayerPrefs.GetFloat(id, 0f);
			evt(id, @float);
		}

		public static List<string> GetLocalAchievementIds()
		{
			if (!LocalAchievementsHelper._initialized)
			{
				LocalAchievementsHelper.Initialize();
			}
			return LocalAchievementsHelper.LocalAchievementsCache;
		}

		public static void SetAchievementUnlocked(string id)
		{
			if (!LocalAchievementsHelper._initialized)
			{
				LocalAchievementsHelper.Initialize();
			}
			if (LocalAchievementsHelper.LocalAchievementsCache.Contains(id))
			{
				return;
			}
			LocalAchievementsHelper.LocalAchievementsCache.Add(id);
			LocalAchievementsHelper.SaveLocalAchievements();
		}

		private static void Initialize()
		{
			if (!LocalAchievementsHelper.LoadLocalAchievementsCache())
			{
				LocalAchievementsHelper.GetAchievementsFromSlots();
				LocalAchievementsHelper.SaveLocalAchievements();
			}
			LocalAchievementsHelper._initialized = true;
		}

		private static bool LoadLocalAchievementsCache()
		{
			LocalAchievementsHelper.LocalAchievementsCache.Clear();
			if (!File.Exists(LocalAchievementsHelper.LocalFilePath) || new FileInfo(LocalAchievementsHelper.LocalFilePath).Length == 0L)
			{
				return false;
			}
			fsData fsData = PersistentManager.ReadAppSettings(LocalAchievementsHelper.LocalFilePath);
			if (fsData.IsList)
			{
				List<fsData> asList = fsData.AsList;
				foreach (fsData fsData2 in asList)
				{
					string asString = fsData2.AsString;
					if (!LocalAchievementsHelper.LocalAchievementsCache.Contains(asString))
					{
						LocalAchievementsHelper.LocalAchievementsCache.Add(asString);
					}
				}
			}
			return true;
		}

		private static void GetAchievementsFromSlots()
		{
			for (int i = 0; i < 3; i++)
			{
				PersistentManager.PublicSlotData slotData = Core.Persistence.GetSlotData(i);
				if (slotData != null)
				{
					foreach (Achievement achievement in slotData.achievement.achievements)
					{
						if (achievement.Progress >= 100f)
						{
							LocalAchievementsHelper.LocalAchievementsCache.Add(achievement.Id);
						}
					}
				}
			}
		}

		private static void SaveLocalAchievements()
		{
			fsData data = fsData.CreateList(LocalAchievementsHelper.LocalAchievementsCache.Count);
			LocalAchievementsHelper.LocalAchievementsCache.ForEach(delegate(string id)
			{
				data.AsList.Add(new fsData(id));
			});
			string s = fsJsonPrinter.CompressedJson(data);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			string encryptedData = Convert.ToBase64String(bytes);
			FileTools.SaveSecure(LocalAchievementsHelper.LocalFilePath, encryptedData);
		}

		private const string LocalAchievementsFilename = "/local_achievements.data";

		private static readonly string LocalFilePath = PersistentManager.GetPathAppSettings("/local_achievements.data");

		private static readonly List<string> LocalAchievementsCache = new List<string>();

		private static bool _initialized;
	}
}
