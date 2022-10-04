using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.Managers;
using FullSerializer;
using Rewired;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class ModeUnlockedWidget : BaseMenuScreen
	{
		public void Open(ModeUnlockedWidget.ModesToUnlock modeUnlocked)
		{
			string pathAppSettings = ModeUnlockedWidget.GetPathAppSettings();
			if (!File.Exists(pathAppSettings))
			{
				File.CreateText(pathAppSettings).Close();
			}
			else
			{
				this.ReadModesUnlocked(pathAppSettings);
			}
			if (!this.unlockedModes.Contains(modeUnlocked))
			{
				this.Open();
				this.unlockedModes.Add(modeUnlocked);
				this.WriteFileAppSettings(pathAppSettings);
			}
		}

		public override void Open()
		{
			base.Open();
			UIController.instance.StartCoroutine(this.DelayedOpen());
		}

		private IEnumerator DelayedOpen()
		{
			yield return new WaitForSeconds(0.1f);
			yield return new WaitUntil(() => !UIController.instance.IsPatchNotesShowing());
			base.gameObject.SetActive(true);
			this.canvasGroup = base.GetComponent<CanvasGroup>();
			this.canvasGroup.alpha = 0f;
			DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 1f, 1f).OnComplete(new TweenCallback(this.OnOpen));
			yield break;
		}

		protected override void OnOpen()
		{
			this.isOpen = true;
			this.rewiredPlayer = ReInput.players.GetPlayer(0);
		}

		public override void Close()
		{
			base.Close();
			DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 0f, 0.2f).OnComplete(new TweenCallback(this.OnClose));
		}

		protected override void OnClose()
		{
			this.isOpen = false;
			base.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (this.rewiredPlayer == null || !this.isOpen)
			{
				return;
			}
			if (this.rewiredPlayer.GetAnyButtonDown() || this.rewiredPlayer.GetAxis(48) != 0f || this.rewiredPlayer.GetAxis(49) != 0f)
			{
				this.Close();
			}
		}

		private static string GetPathAppSettings()
		{
			return PersistentManager.GetPathAppSettings("/app_settings");
		}

		private void ReadModesUnlocked(string filePath)
		{
			fsData fsData = new fsData();
			string s;
			bool flag = PersistentManager.TryToReadFile(filePath, out s);
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
					foreach (KeyValuePair<ModeUnlockedWidget.ModesToUnlock, string> keyValuePair in this.APP_SETTINGS_MODES_UNLOCKED_KEYS)
					{
						if (asDictionary.ContainsKey(keyValuePair.Value) && asDictionary[keyValuePair.Value].AsBool)
						{
							this.unlockedModes.Add(keyValuePair.Key);
						}
					}
				}
			}
		}

		private void WriteFileAppSettings(string filePath)
		{
			fsData fsData = PersistentManager.ReadAppSettings(filePath);
			if (fsData == null || !fsData.IsDictionary)
			{
				return;
			}
			foreach (ModeUnlockedWidget.ModesToUnlock key in this.unlockedModes)
			{
				fsData.AsDictionary[this.APP_SETTINGS_MODES_UNLOCKED_KEYS[key]] = new fsData(true);
			}
			string s = fsJsonPrinter.CompressedJson(fsData);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			string contents = Convert.ToBase64String(bytes);
			File.WriteAllText(filePath, contents);
		}

		private readonly Dictionary<ModeUnlockedWidget.ModesToUnlock, string> APP_SETTINGS_MODES_UNLOCKED_KEYS = new Dictionary<ModeUnlockedWidget.ModesToUnlock, string>
		{
			{
				ModeUnlockedWidget.ModesToUnlock.BossRush,
				"bossrush_unlocked"
			}
		};

		public bool isOpen;

		private CanvasGroup canvasGroup;

		private Player rewiredPlayer;

		private List<ModeUnlockedWidget.ModesToUnlock> unlockedModes = new List<ModeUnlockedWidget.ModesToUnlock>();

		public enum ModesToUnlock
		{
			BossRush
		}
	}
}
