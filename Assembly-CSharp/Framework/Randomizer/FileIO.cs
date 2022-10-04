using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework.Randomizer
{
	public class FileIO
	{
		public void write(string text, string fileName)
		{
			File.WriteAllText(Environment.CurrentDirectory + "/" + fileName, text);
		}

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
			Config config;
			if (text == "")
			{
				config = new Config(false, false, false, true, true, 0, new List<string>
				{
					"item",
					"cherub",
					"lady",
					"oil",
					"sword",
					"blessing",
					"guiltArena",
					"chalice",
					"tirso",
					"miriam",
					"redento1",
					"redento2",
					"cleofas",
					"jocinero",
					"altasgracias2",
					"tentudia",
					"gemino2",
					"amanecida1",
					"guiltBead",
					"ossuary",
					"boss"
				});
				this.write(JsonUtility.ToJson(config, true), "config.json");
			}
			else
			{
				config = JsonUtility.FromJson<Config>(text);
			}
			return config;
		}
	}
}
