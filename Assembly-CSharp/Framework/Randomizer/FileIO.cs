using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework.Randomizer
{
	public class FileIO
	{
		public string read(string fileName)
		{
			if (File.Exists(fileName))
			{
				return File.ReadAllText(fileName);
			}
			return "";
		}

		public Config loadConfig()
		{
			string text = this.read("config.json");
			Config result;
			if (text == "")
			{
				result = this.createNewConfig();
			}
			else
			{
				result = JsonUtility.FromJson<Config>(text);
			}
			return result;
		}

		public void writeLine(string text, string fileName)
		{
			File.AppendAllText(Environment.CurrentDirectory + "/" + fileName, text);
		}

		public void writeAll(string text, string fileName)
		{
			File.WriteAllText(Environment.CurrentDirectory + "/" + fileName, text);
		}

		public Config createNewConfig()
		{
			Config config = new Config(false, false, false, true, true, 0, Randomizer.getVersion(), new List<string>
			{
				"item",
				"cherub",
				"lady",
				"oil",
				"sword",
				"blessing",
				"guiltArena",
				"tirso2",
				"miriam",
				"redento1",
				"jocinero",
				"altasgracias2",
				"tentudia",
				"gemino2",
				"amanecida1",
				"guiltBead",
				"ossuary",
				"boss"
			});
			this.writeAll(JsonUtility.ToJson(config, true), "config.json");
			return config;
		}
	}
}
