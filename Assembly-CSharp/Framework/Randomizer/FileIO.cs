using System;
using System.IO;
using Framework.Randomizer.Config;
using UnityEngine;

namespace Framework.Randomizer
{
	public class FileIO
	{
		public FileIO()
		{
			this.randoLocations = new string[]
			{
				"item",
				"cherub",
				"lady",
				"oil",
				"sword",
				"blessing",
				"guiltArena",
				"tirso",
				"miriam",
				"redento",
				"jocinero",
				"altasgracias",
				"tentudia",
				"gemino",
				"guiltBead",
				"ossuary",
				"boss",
				"visage",
				"mask",
				"herb",
				"church",
				"shop",
				"thorn",
				"candle"
			};
		}

		public string read(string fileName)
		{
			if (File.Exists(Environment.CurrentDirectory + "/" + fileName))
			{
				return File.ReadAllText(Environment.CurrentDirectory + "/" + fileName);
			}
			return "";
		}

		public void writeLine(string text, string fileName)
		{
			File.AppendAllText(Environment.CurrentDirectory + "/" + fileName, text);
		}

		public void writeAll(string text, string fileName)
		{
			File.WriteAllText(Environment.CurrentDirectory + "/" + fileName, text);
		}

		public MainConfig loadConfig()
		{
			string text = this.read("randomizer.cfg");
			MainConfig result;
			if (text == "")
			{
				result = this.createNewConfig();
			}
			else
			{
				result = JsonUtility.FromJson<MainConfig>(text);
			}
			return result;
		}

		public MainConfig createNewConfig()
		{
			MainConfig mainConfig = new MainConfig(Randomizer.getVersion(), new GeneralConfig(true, true, false, false, 0), new ItemConfig(1, true, true, this.randoLocations), new EnemyConfig(1, true), new PrayerConfig(1, false), new RoomConfig(0), new DebugConfig(0));
			this.writeAll(JsonUtility.ToJson(mainConfig, true), "randomizer.cfg");
			return mainConfig;
		}

		public static bool arrayContains(string[] arr, string id)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] == id)
				{
					return true;
				}
			}
			return false;
		}

		private string[] randoLocations;

		private const string configPath = "randomizer.cfg";
	}
}
