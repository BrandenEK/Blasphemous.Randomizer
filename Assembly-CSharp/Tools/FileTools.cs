using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework.Managers;
using Framework.Util;
using UnityEngine;

namespace Tools
{
	public static class FileTools
	{
		public static void SaveSecure(string path, string encryptedData)
		{
			Singleton<Core>.Instance.StartCoroutine(FileTools.SaveSecureCorutine(path, encryptedData));
		}

		public static IEnumerator SaveSecureCorutine(string path, string encryptedData)
		{
			bool lockedByThisRutine = false;
			bool keepTrying = true;
			int triesLeft = 180;
			while (keepTrying)
			{
				triesLeft--;
				if (lockedByThisRutine || !FileTools.lockedFiles.Contains(path))
				{
					if (!lockedByThisRutine)
					{
						lockedByThisRutine = true;
						FileTools.lockedFiles.Add(path);
					}
					try
					{
						File.WriteAllText(path, encryptedData);
						keepTrying = false;
					}
					catch (Exception ex)
					{
						if (ex is IOException)
						{
							Debug.Log("-- File is locked is bussy, trying another frame");
						}
						else
						{
							Debug.LogError(string.Concat(new string[]
							{
								"*** CAN'T SAVE FILE ",
								path,
								"\nException:\n",
								ex.Message,
								"\n",
								ex.StackTrace
							}));
							keepTrying = false;
						}
					}
				}
				if (keepTrying)
				{
					if (triesLeft <= 0)
					{
						Debug.LogError("-- Operation Write cancelled, all tries are wasted, file:" + path);
						keepTrying = false;
					}
					else
					{
						yield return new WaitForEndOfFrame();
					}
				}
			}
			if (lockedByThisRutine)
			{
				FileTools.lockedFiles.Remove(path);
			}
			yield break;
		}

		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
			if (!directoryInfo.Exists)
			{
				throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				string destFileName = Path.Combine(destDirName, fileInfo.Name);
				fileInfo.CopyTo(destFileName, overwrite);
			}
			if (copySubDirs)
			{
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					string destDirName2 = Path.Combine(destDirName, directoryInfo2.Name);
					FileTools.DirectoryCopy(directoryInfo2.FullName, destDirName2, copySubDirs, overwrite);
				}
			}
		}

		public static List<string> lockedFiles = new List<string>();

		private const int TICKS_TIMEOUT = 180;
	}
}
