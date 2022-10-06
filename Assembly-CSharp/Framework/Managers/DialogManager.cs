using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using FMOD.Studio;
using FMODUnity;
using Framework.Dialog;
using Framework.FrameworkCore;
using Framework.Inventory;
using Gameplay.UI;
using I2.Loc;
using UnityEngine;

namespace Framework.Managers
{
	public class DialogManager : GameSystem
	{
		public bool InDialog { get; private set; }

		public int LastDialogAnswer { get; private set; }

		public LanguageSource Language { get; private set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event DialogManager.DialogEvent OnDialogFinished;

		public override void Start()
		{
			this.Language = DialogManager.GetLanguageSource();
			Core.Localization.AddLanguageSource("Dialog/Languages");
			LocalizationManager.OnLocalizeEvent += new LocalizationManager.OnLocalizeCallback(this.OnLocalizationChange);
			LocalizationManager.OnLocalizeAudioEvent += this.OnAudioLocalizationChange;
			this.InDialog = false;
			DialogObject[] array = Resources.LoadAll<DialogObject>("Dialog/");
			this.allDialogs.Clear();
			foreach (DialogObject dialogObject in array)
			{
				this.allDialogs[dialogObject.id] = dialogObject;
			}
			foreach (KeyValuePair<string, DialogObject> keyValuePair in this.allDialogs)
			{
				if (keyValuePair.Value.externalAnswersId != string.Empty)
				{
					if (!this.allDialogs.ContainsKey(keyValuePair.Value.externalAnswersId))
					{
						Debug.LogError("** Dialog " + keyValuePair.Key + " references missing external id: " + keyValuePair.Value.externalAnswersId);
					}
					else
					{
						keyValuePair.Value.answersLines = new List<string>(this.allDialogs[keyValuePair.Value.externalAnswersId].answersLines);
					}
				}
			}
			Log.Debug("Dialog", this.allDialogs.Count.ToString() + " dialogs loaded succesfully.", null);
			this.allMessges.Clear();
			MsgObject[] array3 = Resources.LoadAll<MsgObject>("Dialog/");
			foreach (MsgObject msgObject in array3)
			{
				this.allMessges[msgObject.id] = msgObject;
			}
			Log.Debug("Dialog", this.allMessges.Count.ToString() + " messages loaded succesfully.", null);
			DialogManager.currentLanguage = string.Empty;
			this.LastDialogAnswer = -1;
			this.OnLocalizationChange();
			this.OnAudioLocalizationChange(Core.Localization.GetCurrentAudioLanguageCode());
			this.hideWidgetAtEnd = true;
		}

		public ReadOnlyCollection<DialogObject> GetAllDialogs()
		{
			List<DialogObject> list = new List<DialogObject>();
			foreach (DialogObject item in this.allDialogs.Values)
			{
				list.Add(item);
			}
			return list.AsReadOnly();
		}

		public bool StartConversation(string conversiationId, bool modal, bool useOnlyLast, bool hideWidget = true, int objectCost = 0, bool useBackground = false)
		{
			if (this.InDialog || !this.allDialogs.ContainsKey(conversiationId))
			{
				return false;
			}
			UIController.instance.GetDialog().SetBackgound(useBackground);
			Core.UI.Fade.ClearFade();
			this.hideWidgetAtEnd = hideWidget;
			this.LastDialogAnswer = -1;
			this.currentLine = -1;
			int num = 0;
			this.currentDialog = this.allDialogs[conversiationId];
			switch (this.currentDialog.dialogType)
			{
			case DialogObject.DialogType.Lines:
				num = this.CalculateLines_Text();
				break;
			case DialogObject.DialogType.GiveObject:
				num = this.CalculateLines_Object();
				break;
			case DialogObject.DialogType.GivePurge:
				this.currentCost = objectCost;
				num = this.CalculateLines_Purge();
				break;
			case DialogObject.DialogType.BuyObject:
				this.currentCost = objectCost;
				num = this.CalculateLines_Buy();
				break;
			case DialogObject.DialogType.PurgeGeneric:
				this.currentCost = objectCost;
				num = this.CalculateLines_PurgeGeneric();
				break;
			}
			if (useOnlyLast)
			{
				this.currentLine = num - 1;
			}
			this.currentModal = ((!modal) ? DialogManager.ModalMode.NoModal : DialogManager.ModalMode.Modal);
			if (this.currentDialog.modalBoss)
			{
				this.currentModal = DialogManager.ModalMode.Boss;
			}
			if (this.currentModal != DialogManager.ModalMode.NoModal)
			{
				Core.Input.SetBlocker("DIALOG", true);
			}
			this.InDialog = true;
			this.ShowNextLine();
			return true;
		}

		public bool ShowMessage(string messageId, int line, string eventSound = "", float timeToWait = 0f, bool blockPlayer = true)
		{
			if (this.InDialog || !this.allMessges.ContainsKey(messageId) || this.allMessges[messageId].msgLines.Count < line)
			{
				return false;
			}
			string message = this.allMessges[messageId].msgLines[line];
			UIController.instance.ShowPopUp(message, eventSound, timeToWait, blockPlayer);
			return true;
		}

		public void UIEvent_LineEnded(int response = -1)
		{
			this.LastDialogAnswer = response;
			this.ShowNextLine();
		}

		public EventInstance PlayProgrammerSound(string key, FMODAudioManager.ProgrammerSoundSeted eventSound = null)
		{
			return Core.Audio.PlayProgrammerSound("event:/VoiceOver/All", key, eventSound);
		}

		private void ShowNextLine()
		{
			if (this.currentLine >= this.currentLines.Count - 1)
			{
				this.EndDialog();
				return;
			}
			this.currentLine++;
			DialogManager.DialogLines dialogLines = this.currentLines[this.currentLine];
			List<string> responses = new List<string>();
			if (this.currentLine == this.currentLines.Count - 1)
			{
				responses = this.currentDialog.answersLines;
			}
			if (dialogLines.StopAudio && this.currentSound.isValid() && this.currentSound.isValid())
			{
				this.currentSound.stop(0);
			}
			if (!string.IsNullOrEmpty(dialogLines.AudioKey) && dialogLines.Text.Length > 0)
			{
				this.currentSound = this.PlayProgrammerSound(dialogLines.AudioKey, new FMODAudioManager.ProgrammerSoundSeted(UIController.instance.GetDialog().OnProgrammerSoundSeted));
			}
			switch (this.currentDialog.dialogType)
			{
			case DialogObject.DialogType.Lines:
				if (dialogLines.LongText)
				{
					UIController.instance.GetDialog().ShowLongText(dialogLines.Text, responses, this.currentModal);
				}
				else
				{
					UIController.instance.GetDialog().ShowText(dialogLines.Text, responses, this.currentModal);
				}
				break;
			case DialogObject.DialogType.GiveObject:
				UIController.instance.GetDialog().ShowItem(dialogLines.Text, dialogLines.Image, responses, this.currentModal);
				break;
			case DialogObject.DialogType.GivePurge:
				UIController.instance.GetDialog().ShowPurge(dialogLines.Text, responses, this.currentModal);
				break;
			case DialogObject.DialogType.BuyObject:
				UIController.instance.GetDialog().ShowBuy(dialogLines.Additional["PURGE"], dialogLines.Additional["CAPTION"], dialogLines.Additional["DESCRIPTION"], dialogLines.Image, responses, this.currentModal);
				break;
			case DialogObject.DialogType.PurgeGeneric:
			{
				string text = Regex.Replace(dialogLines.Text, "{PURGEPOINTS}", dialogLines.Additional["PURGE"], RegexOptions.IgnoreCase);
				UIController.instance.GetDialog().ShowPurgeGeneric(text, responses, this.currentModal);
				break;
			}
			}
		}

		private void EndDialog()
		{
			if (this.currentSound.isValid())
			{
				this.currentSound.stop(0);
				this.currentSound = default(EventInstance);
			}
			if (!this.endDialogSafeRunning)
			{
				UIController.instance.StartCoroutine(this.EndDialogSafe());
			}
		}

		private int CalculateLines_Text()
		{
			int result = 0;
			this.currentLines = new List<DialogManager.DialogLines>();
			int num = 0;
			foreach (string text in this.currentDialog.dialogLines)
			{
				string text2 = this.currentDialog.id + "_" + num.ToString();
				if (this.currentDialog.useOverrideAudioKey)
				{
					text2 = this.currentDialog.overrideKey;
				}
				if (num == this.currentDialog.dialogLines.Count - 1)
				{
					result = this.currentLines.Count;
				}
				bool flag = UIController.instance.GetDialog().GetNumberOfLines(text) > 2;
				List<DialogManager.DialogLines> list = this.currentLines;
				string text3 = text;
				string audioKey = text2;
				bool stopAudio = true;
				bool longText = flag;
				list.Add(new DialogManager.DialogLines(text3, audioKey, stopAudio, null, null, longText));
				num++;
			}
			if (this.currentLines.Count == 0 && this.currentDialog.answersLines.Count > 0)
			{
				this.currentLines.Add(new DialogManager.DialogLines(string.Empty, string.Empty, true, null, null, false));
			}
			return result;
		}

		private int CalculateLines_Object()
		{
			this.currentLines = new List<DialogManager.DialogLines>();
			string text = string.Empty;
			string value = string.Empty;
			Sprite sprite = null;
			switch (this.currentDialog.itemType)
			{
			case InventoryManager.ItemType.Relic:
			{
				Relic relic = Core.InventoryManager.GetRelic(this.currentDialog.item);
				if (relic)
				{
					text = relic.caption;
					sprite = relic.picture;
					value = relic.description;
				}
				break;
			}
			case InventoryManager.ItemType.Prayer:
			{
				Prayer prayer = Core.InventoryManager.GetPrayer(this.currentDialog.item);
				if (prayer)
				{
					text = prayer.caption;
					sprite = prayer.picture;
					value = prayer.description;
				}
				break;
			}
			case InventoryManager.ItemType.Bead:
			{
				RosaryBead rosaryBead = Core.InventoryManager.GetRosaryBead(this.currentDialog.item);
				if (rosaryBead)
				{
					text = rosaryBead.caption;
					sprite = rosaryBead.picture;
					value = rosaryBead.description;
				}
				break;
			}
			case InventoryManager.ItemType.Quest:
			{
				QuestItem questItem = Core.InventoryManager.GetQuestItem(this.currentDialog.item);
				if (questItem)
				{
					text = questItem.caption;
					sprite = questItem.picture;
					value = questItem.description;
				}
				break;
			}
			case InventoryManager.ItemType.Collectible:
			{
				Framework.Inventory.CollectibleItem collectibleItem = Core.InventoryManager.GetCollectibleItem(this.currentDialog.item);
				if (collectibleItem)
				{
					text = collectibleItem.caption;
					sprite = collectibleItem.picture;
					value = collectibleItem.description;
				}
				break;
			}
			case InventoryManager.ItemType.Sword:
			{
				Sword sword = Core.InventoryManager.GetSword(this.currentDialog.item);
				if (sword)
				{
					text = sword.caption;
					sprite = sword.picture;
					value = sword.description;
				}
				break;
			}
			}
			Dictionary<string, string> additional = new Dictionary<string, string>
			{
				{
					"CAPTION",
					text
				},
				{
					"DESCRIPTION",
					value
				}
			};
			DialogManager.DialogLines item = new DialogManager.DialogLines(text, string.Empty, true, sprite, additional, false);
			this.currentLines.Add(item);
			return 1;
		}

		private int CalculateLines_Buy()
		{
			this.CalculateLines_Object();
			DialogManager.DialogLines dialogLines = this.currentLines[this.currentLines.Count - 1];
			dialogLines.Additional["PURGE"] = this.currentCost.ToString("F0");
			return 1;
		}

		private int CalculateLines_Purge()
		{
			this.currentLines = new List<DialogManager.DialogLines>();
			DialogManager.DialogLines item = new DialogManager.DialogLines(this.currentCost.ToString("F0"), string.Empty, true, null, null, false);
			this.currentLines.Add(item);
			return 1;
		}

		private int CalculateLines_PurgeGeneric()
		{
			this.currentLines = new List<DialogManager.DialogLines>();
			foreach (string text in this.currentDialog.dialogLines)
			{
				DialogManager.DialogLines dialogLines = new DialogManager.DialogLines(text, string.Empty, true, null, null, false);
				dialogLines.Additional["PURGE"] = this.currentCost.ToString("F0");
				this.currentLines.Add(dialogLines);
			}
			return this.currentLines.Count;
		}

		private int GetCharacterAtMinDistance(string line, string separator)
		{
			int num = line.Length / 2;
			int num2 = line.IndexOf(separator, num);
			int num3 = line.Substring(0, num).LastIndexOf(separator);
			int num4 = num2;
			if (num4 == -1 || num - num3 <= num2 - num)
			{
				num4 = num2;
			}
			return num4;
		}

		private IEnumerator EndDialogSafe()
		{
			this.endDialogSafeRunning = true;
			string id = this.currentDialog.id;
			if (this.hideWidgetAtEnd)
			{
				yield return new WaitForSeconds(0.2f);
			}
			UIController.instance.GetDialog().Hide(this.hideWidgetAtEnd);
			this.InDialog = false;
			this.currentDialog = null;
			Core.Input.SetBlocker("DIALOG", false);
			if (this.OnDialogFinished != null)
			{
				this.OnDialogFinished(id, this.LastDialogAnswer);
			}
			yield return null;
			this.endDialogSafeRunning = false;
			yield break;
		}

		public static LanguageSource GetLanguageSource()
		{
			GameObject asset = ResourceManager.pInstance.GetAsset<GameObject>("Dialog/Languages");
			return (!asset) ? null : asset.GetComponent<LanguageSource>();
		}

		private void OnAudioLocalizationChange(string idlang)
		{
			string text = this.currentBankName;
			this.currentBankName = "VoiceOver_" + idlang;
			if (text != this.currentBankName || text == string.Empty)
			{
				if (text != string.Empty)
				{
					RuntimeManager.UnloadBank(text);
				}
				RuntimeManager.LoadBank(this.currentBankName, true);
				Debug.Log("Audio localization event, new bank " + this.currentBankName);
			}
		}

		private void OnLocalizationChange()
		{
			if (DialogManager.currentLanguage != LocalizationManager.CurrentLanguage)
			{
				if (DialogManager.currentLanguage != string.Empty)
				{
					Log.Debug("Dialog", "Language change, localize items to: " + LocalizationManager.CurrentLanguage, null);
				}
				DialogManager.currentLanguage = LocalizationManager.CurrentLanguage;
				int languageIndexFromCode = this.Language.GetLanguageIndexFromCode(LocalizationManager.CurrentLanguageCode, true);
				foreach (DialogObject dialogObject in this.allDialogs.Values)
				{
					this.LocalizeList(dialogObject.GetBaseTranslationID(), languageIndexFromCode, dialogObject.dialogLines);
					string str = dialogObject.GetBaseTranslationID();
					if (dialogObject.externalAnswersId != string.Empty)
					{
						str = "GENERIC/" + dialogObject.externalAnswersId;
					}
					this.LocalizeList(str + "_ANSWER", languageIndexFromCode, dialogObject.answersLines);
				}
				foreach (MsgObject msgObject in this.allMessges.Values)
				{
					this.LocalizeList(msgObject.GetBaseTranslationID(), languageIndexFromCode, msgObject.msgLines);
				}
			}
		}

		private void LocalizeList(string id, int idxLanguage, List<string> list)
		{
			string str = id + "_";
			for (int i = 0; i < list.Count; i++)
			{
				string text = str + i.ToString();
				TermData termData = this.Language.GetTermData(text, false);
				if (termData == null)
				{
					Debug.LogWarning("Term " + text + " not found in Dialog Localization");
				}
				else if (termData.Languages.ElementAtOrDefault(idxLanguage) != null)
				{
					string text2 = termData.Languages[idxLanguage];
					if (text2.Length == 0)
					{
						text2 = "!ERROR LOC, NO TEXT";
					}
					list[i] = text2;
				}
			}
		}

		private Dictionary<string, DialogObject> allDialogs = new Dictionary<string, DialogObject>();

		private Dictionary<string, MsgObject> allMessges = new Dictionary<string, MsgObject>();

		private int currentLine = -1;

		private DialogObject currentDialog;

		private List<DialogManager.DialogLines> currentLines;

		private DialogManager.ModalMode currentModal;

		private bool hideWidgetAtEnd;

		private bool endDialogSafeRunning;

		private int currentCost;

		private const string LANGUAGE_PREFAB_NAME = "Dialog/Languages";

		private static string currentLanguage = string.Empty;

		private EventInstance currentSound = default(EventInstance);

		private string currentBankName = string.Empty;

		private const string FMOD_EVENT_NAME = "event:/VoiceOver/All";

		private const string FMOD_BANK_NAME = "VoiceOver_";

		public const string GENERIC_QUEST = "GENERIC";

		private const int MAX_LINES = 2;

		private const int MAX_LINE_SEPARATOR = 100;

		private const string KEY_PURGE = "PURGE";

		private const string KEY_CAPTION = "CAPTION";

		private const string KEY_DESCRIPTION = "DESCRIPTION";

		private class DialogLines
		{
			public DialogLines(string text, string audioKey, bool stopAudio, Sprite sprite = null, Dictionary<string, string> additional = null, bool longText = false)
			{
				this.Text = text;
				this.AudioKey = audioKey;
				this.StopAudio = stopAudio;
				this.Image = sprite;
				this.LongText = longText;
				if (additional != null)
				{
					this.Additional = new Dictionary<string, string>(additional);
				}
				else
				{
					this.Additional = new Dictionary<string, string>();
				}
			}

			public string Text { get; private set; }

			public bool LongText { get; private set; }

			public string AudioKey { get; private set; }

			public bool StopAudio { get; private set; }

			public Sprite Image { get; private set; }

			public Dictionary<string, string> Additional { get; private set; }
		}

		public enum ModalMode
		{
			NoModal,
			Modal,
			Boss
		}

		public delegate void DialogEvent(string id, int response);
	}
}
