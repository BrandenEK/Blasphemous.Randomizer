using System;
using System.IO;
using UnityEngine;

namespace Framework.Util
{
	public class HandleTextFile
	{
		public static void WriteString(string text)
		{
			string path = Application.dataPath + "/emails.txt";
			StreamWriter streamWriter = new StreamWriter(path, true);
			streamWriter.WriteLine(text);
			streamWriter.Close();
		}

		public static void ReadString()
		{
			string path = Application.dataPath + "/emails.txt";
			StreamReader streamReader = new StreamReader(path);
			Debug.Log(streamReader.ReadToEnd());
			streamReader.Close();
		}
	}
}
