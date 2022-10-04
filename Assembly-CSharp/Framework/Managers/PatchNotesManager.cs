using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework.PatchNotes;
using FullSerializer;
using Gameplay.UI;
using UnityEngine;

namespace Framework.Managers
{
	public class PatchNotesManager : GameSystem
	{
		public override void Initialize()
		{
			string pathSeenPatchNotes = this.GetPathSeenPatchNotes();
			if (!File.Exists(pathSeenPatchNotes))
			{
				File.CreateText(pathSeenPatchNotes).Close();
			}
			else
			{
				this.ReadFileSeenPatchNotes(pathSeenPatchNotes);
			}
			PatchNotesList patchNotesList = Resources.Load<PatchNotesList>("Patch Notes/PATCH_NOTES_LIST");
			this.FindUnseenPatchNotes(patchNotesList);
		}

		public void MarkPatchNotesAsSeen()
		{
			this.seenPatchNotesVersions.AddRange(this.unseenPatchNotesVersions);
			string pathSeenPatchNotes = this.GetPathSeenPatchNotes();
			this.WriteFileSeenPatchNotes(pathSeenPatchNotes);
		}

		public override void Update()
		{
			if (!this.newPatchesAlreadyShownOnStart && this.unseenPatchNotesVersions.Count > 0 && UIController.instance != null)
			{
				UIController.instance.ShowPatchNotes();
				this.newPatchesAlreadyShownOnStart = true;
			}
		}

		public List<string> GetPatchNotesToBeMarkedAsNew()
		{
			return this.unseenPatchNotesVersions;
		}

		private string GetPathSeenPatchNotes()
		{
			return PersistentManager.GetPathAppSettings("/app_settings");
		}

		private void ReadFileSeenPatchNotes(string filePath)
		{
			fsData fsData = new fsData();
			string s;
			bool flag = this.TryToReadFile(filePath, out s);
			if (flag)
			{
				byte[] bytes = Convert.FromBase64String(s);
				string @string = Encoding.UTF8.GetString(bytes);
				fsResult fsResult = fsJsonParser.Parse(@string, out fsData);
				if (fsResult.Failed && !fsResult.FormattedMessages.Equals("No input"))
				{
					Debug.LogError("Parsing error: " + fsResult.FormattedMessages);
				}
				else if (fsData != null)
				{
					Dictionary<string, fsData> asDictionary = fsData.AsDictionary;
					if (asDictionary.ContainsKey("seen_patch_notes"))
					{
						string[] collection = asDictionary["seen_patch_notes"].AsString.Trim().Split(new char[]
						{
							','
						});
						this.seenPatchNotesVersions = new List<string>(collection);
					}
				}
			}
		}

		private void WriteFileSeenPatchNotes(string filePath)
		{
			if (this.seenPatchNotesVersions.Count == 0)
			{
				return;
			}
			string text = string.Empty;
			foreach (string str in this.seenPatchNotesVersions)
			{
				text = text + str + ",";
			}
			text = text.Remove(text.Length - 1);
			fsData fsData = this.ReadAppSettings(filePath);
			fsData.AsDictionary["seen_patch_notes"] = new fsData(text);
			string s = fsJsonPrinter.CompressedJson(fsData);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			string contents = Convert.ToBase64String(bytes);
			File.WriteAllText(filePath, contents);
		}

		private fsData ReadAppSettings(string filePath)
		{
			fsData result = new fsData();
			string s;
			bool flag = this.TryToReadFile(filePath, out s);
			if (flag)
			{
				byte[] bytes = Convert.FromBase64String(s);
				string @string = Encoding.UTF8.GetString(bytes);
				fsResult fsResult = fsJsonParser.Parse(@string, out result);
				if (fsResult.Failed && !fsResult.FormattedMessages.Equals("No input"))
				{
					Debug.LogError("Parsing error: " + fsResult.FormattedMessages);
				}
			}
			return result;
		}

		private bool TryToReadFile(string filePath, out string fileData)
		{
			if (!File.Exists(filePath))
			{
				Debug.LogError("File at path '" + filePath + "' does not exists!");
				fileData = string.Empty;
				return false;
			}
			fileData = File.ReadAllText(filePath);
			if (fileData.Length == 0)
			{
				Debug.Log("File at path '" + filePath + "' is empty.");
				return false;
			}
			return true;
		}

		private void FindUnseenPatchNotes(PatchNotesList patchNotesList)
		{
			string version = this.FindLastVersionInList(this.seenPatchNotesVersions);
			List<int> versionDigits = this.GetVersionDigits(version);
			foreach (PatchNotes patchNotes in patchNotesList.patchNotesList)
			{
				List<int> versionDigits2 = this.GetVersionDigits(patchNotes.version);
				for (int i = 0; i < versionDigits2.Count; i++)
				{
					if (versionDigits2[i] < versionDigits[i])
					{
						break;
					}
					if (versionDigits2[i] > versionDigits[i])
					{
						this.unseenPatchNotesVersions.Add(patchNotes.version);
						break;
					}
				}
			}
		}

		private string FindLastVersionInList(List<string> versions)
		{
			string text = "1.0";
			foreach (string text2 in versions)
			{
				List<int> versionDigits = this.GetVersionDigits(text);
				List<int> versionDigits2 = this.GetVersionDigits(text2);
				if (versionDigits2 != null)
				{
					for (int i = 0; i < Mathf.Min(versionDigits2.Count, versionDigits.Count); i++)
					{
						if (versionDigits2[i] < versionDigits[i])
						{
							break;
						}
						if (versionDigits2[i] > versionDigits[i])
						{
							text = text2;
							break;
						}
					}
				}
			}
			return text;
		}

		private List<int> GetVersionDigits(string version)
		{
			List<int> list = new List<int>();
			string[] array = version.Split(new char[]
			{
				'.'
			});
			foreach (string s in array)
			{
				int item = 0;
				bool flag = int.TryParse(s, out item);
				if (!flag)
				{
					Debug.LogError("GetVersionDigits error! version '" + version + "' does not seem to be in a correct format!");
					list = null;
					break;
				}
				list.Add(item);
			}
			return list;
		}

		private const string SEEN_PATCH_NOTES_PATH = "/app_settings";

		private const string APP_SETTINGS_KEY = "seen_patch_notes";

		private const string PATCH_NOTES_PATH = "Patch Notes/PATCH_NOTES_LIST";

		private List<string> seenPatchNotesVersions = new List<string>();

		private List<string> unseenPatchNotesVersions = new List<string>();

		private bool newPatchesAlreadyShownOnStart;
	}
}
