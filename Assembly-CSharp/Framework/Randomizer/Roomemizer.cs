using System;
using System.Collections.Generic;

namespace Framework.Randomizer
{
	public static class Roomemizer
	{
		public static void start()
		{
			Roomemizer.doorIds = new List<string>();
			Roomemizer.doorScenes = new List<string>();
			Roomemizer.targetIds = new List<string>();
			Roomemizer.targetScenes = new List<string>();
		}

		public static void addDoor(string scene, string id, string tarScene, string tarId)
		{
			if (Roomemizer.doorIds == null)
			{
				Roomemizer.start();
			}
			if (!Roomemizer.doorIds.Contains(id) || !Roomemizer.doorScenes.Contains(scene))
			{
				Roomemizer.doorIds.Add(id);
				Roomemizer.doorScenes.Add(scene);
				Roomemizer.targetIds.Add(tarId);
				Roomemizer.targetScenes.Add(tarScene);
			}
		}

		public static string getData()
		{
			if (Roomemizer.doorIds == null)
			{
				return "";
			}
			string text = "";
			for (int i = 0; i < Roomemizer.doorIds.Count; i++)
			{
				text = string.Concat(new string[]
				{
					text,
					Roomemizer.doorScenes[i],
					",",
					Roomemizer.doorIds[i],
					",",
					Roomemizer.targetScenes[i],
					",",
					Roomemizer.targetIds[i],
					"\n"
				});
			}
			return text;
		}

		private static List<string> doorIds;

		private static List<string> doorScenes;

		private static List<string> targetIds;

		private static List<string> targetScenes;
	}
}
