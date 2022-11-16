using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;
using BlasphemousRandomizer.Config;

namespace BlasphemousRandomizer
{
    public static class FileUtil
    {
		// Checks if an array contains a certain object
		public static bool arrayContains<T>(T[] arr, T id)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i].Equals(id))
				{
					return true;
				}
			}
			return false;
		}

		// Loads the config file from the root directory
		public static MainConfig loadConfig()
        {
			if (read("randomizer.cfg", false, out string json))
            {
				return JsonUtility.FromJson<MainConfig>(json);
            }
			MainConfig config = MainConfig.Default();
			saveConfig(config);
			return config;
        }

		// Saves the config file to the root directory
		public static void saveConfig(MainConfig config)
        {
			writeFull("randomizer.cfg", JsonUtility.ToJson(config));
        }

		// Read all text from file in the root directory or the data folder
		public static bool read(string fileName, bool data, out string text)
		{
			string path = (data ? Paths.PluginPath + "/BlasphemousRandomizer" : Paths.GameRootPath) + "/" + fileName;
			if (File.Exists(path))
			{
				text = File.ReadAllText(path);
				return true;
			}
			text = "";
			return false;
		}

		// Read bytes from file in data folder
		public static bool readBytes(string fileName, out byte[] data)
		{
			string path = Paths.PluginPath + "/BlasphemousRandomizer/" + fileName;
			if (File.Exists(path))
			{
				data = File.ReadAllBytes(path);
				return true;
			}
			data = null;
			return false;
		}

		// Write to file
		public static void writeFull(string fileName, string text)
		{
			File.WriteAllText(Paths.GameRootPath + "/" + fileName, text);
		}

		// Add line to file
		public static void writeLine(string fileName, string text)
		{
			File.AppendAllText(Paths.GameRootPath + "/" + fileName, text);
		}

		// Read file and split each line into a key value pair
		public static bool parseFileToDictionary(string fileName, Dictionary<string, string> output)
		{
			if (!parseFiletoArray(fileName, out string[] array))
            {
				return false;
            }
			for (int i = 0; i < array.Length; i++)
			{
				int num = array[i].IndexOf(',');
				output.Add(array[i].Substring(0, num), array[i].Substring(num + 1));
			}
			return true;
		}

		// Read file and add each line to an array
		public static bool parseFiletoArray(string fileName, out string[] output)
		{
			if (!read(fileName, true, out string text))
			{
				output = null;
				return false;
			}
			text = text.Replace("\r", string.Empty);
			output = text.Split('\n');
			return true;
		}

		// Reads list of objects from json file in data folder
		public static bool loadJson<T>(string fileName, out List<T> list)
		{
			if (read(fileName, true, out string json))
			{
				Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
				list = wrapper.Items;
				return true;
			}
			list = null;
			return false;
		}

		// Writes list of objects to json file in data folder
		public static void saveJson<T>(string fileName, List<T> list)
		{
			writeFull(fileName, JsonUtility.ToJson(new Wrapper<T>(list), true));
		}

		[Serializable]
		private class Wrapper<T>
		{
			public List<T> Items;

			public Wrapper(List<T> list)
            {
				Items = list;
            }
		}
	}
}
