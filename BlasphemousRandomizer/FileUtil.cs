using System.Collections.Generic;
using System.IO;
using BepInEx;
using BlasphemousRandomizer.Config;
using UnityEngine;
using Newtonsoft.Json;

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

		// Read all text from file in the root directory or the data folder
		public static bool read(string fileName, bool data, out string text)
		{
			string path = (data ? Paths.PluginPath + "/BlasphemousRandomizer/data" : Paths.GameRootPath) + "/" + fileName;
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
			string path = Paths.PluginPath + "/BlasphemousRandomizer/data/" + fileName;
			if (File.Exists(path))
			{
				data = File.ReadAllBytes(path);
				return true;
			}
			data = new byte[0];
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

		// Deletes file
		public static void delete(string fileName)
        {
			string path = Paths.PluginPath + "/BlasphemousRandomizer/data/" + fileName;
			if (File.Exists(path))
			{
				try
                {
					File.Delete(path);
					Main.Randomizer.Log("Deleted file: " + fileName);
                }
				catch (System.Exception e)
                {
					Main.Randomizer.LogError(e.Message);
                }
			}
		}

		// Read file and split each line into a key value pair
		public static bool parseFileToDictionary(string fileName, out Dictionary<string, string> output)
		{
			output = new Dictionary<string, string>();
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
				output = new string[0];
				return false;
			}
			text = text.Replace("\r", string.Empty);
			output = text.Split('\n');
			return true;
		}

		// Loads an array of square images from a file
		public static bool loadImages(string fileName, int size, int pixels, int border, bool pointFilter, out Sprite[] images)
        {
			// Read bytes from file
			if (!readBytes(fileName, out byte[] data))
            {
				images = new Sprite[0];
				return false;
            }

			// Convert to texture
			Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
			tex.LoadImage(data);
			if (pointFilter)
				tex.filterMode = FilterMode.Point;
			int w = tex.width, h = tex.height;
			images = new Sprite[w * h / (size * size)];

			// Insert each image into the array (T-B, L-R)
			int count = 0;
			for (int i = h - size; i >= 0; i -= size)
			{
				for (int j = 0; j < w; j += size)
				{
					Sprite sprite = Sprite.Create(tex, new Rect(j, i, size, size), new Vector2(0.5f, 0.5f), pixels, 0, SpriteMeshType.Tight, new Vector4(border, border, border, border));
					images[count] = sprite;
					count++;
				}
			}
			return true;
		}

		// Loads the config file from the root directory
		public static MainConfig loadConfig()
		{
			if (read("randomizer.cfg", false, out string json))
			{
				return JsonConvert.DeserializeObject<MainConfig>(json); // Unused
			}
			MainConfig config = MainConfig.Default();
			saveConfig(config);
			return config;
		}

		// Saves the config file to the root directory
		public static void saveConfig(MainConfig config)
		{
			writeFull("randomizer.cfg", JsonConvert.SerializeObject(config, Formatting.Indented)); // Unused
		}

		// Reads object from json file in data folder
		public static bool loadJson<T>(string fileName, out T obj)
		{
			if (!read(fileName, true, out string json))
			{
				obj = default(T);
				return false;
			}
			obj = jsonObject<T>(json);
			return true;
		}

		// Writes object to json file in data folder
		public static void saveJson<T>(string fileName, T obj)
		{
			writeFull(fileName, jsonString(obj));
		}

		// Convert object to/from json
		public static T jsonObject<T>(string json) { return JsonConvert.DeserializeObject<T>(json); }
		public static string jsonString<T>(T obj) { return JsonConvert.SerializeObject(obj, Formatting.Indented); }

		// Recursive method that returns the entire hierarchy of an object
		public static string displayHierarchy(Transform transform, string output, int level, bool components)
		{
			// Indent
			for (int i = 0; i < level; i++)
				output += "\t";

			// Add this object
			output += transform.name;

			// Add components
			if (components)
			{
				output += " (";
				foreach (Component c in transform.GetComponents<Component>())
					output += c.ToString() + ", ";
				output = output.Substring(0, output.Length - 2) + ")";
			}
			output += "\n";

			// Add children
			for (int i = 0; i < transform.childCount; i++)
				output = displayHierarchy(transform.GetChild(i), output, level + 1, components);

			// Return output
			return output;
		}
	}
}
