using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Framework.Map;
using Gameplay.UI.Others.Buttons;
using Gameplay.UI.Widgets;
using I2.Loc;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class SelectSaveSlots : SerializedMonoBehaviour
	{
		public bool IsShowing { get; private set; }

		public bool IsConfirming { get; private set; }

		public bool IsConfirmingUpgrade { get; private set; }

		public int SelectedSlot { get; private set; }

		public bool MustConvertToNewgamePlus { get; private set; }

		public bool CanLoadSelectedSlot
		{
			get
			{
				return this.SelectedSlot >= 0 && this.slots[this.SelectedSlot].CanLoad;
			}
		}

		private void Update()
		{
			if (ReInput.players.playerCount <= 0 || (this.corruptedSaveMessage && this.corruptedSaveMessage.IsShowing))
			{
				return;
			}
			if (this.CurrentSlotsMode == SelectSaveSlots.SlotsModes.BossRush)
			{
				return;
			}
			Player player = ReInput.players.GetPlayer(0);
			if (player.GetButtonDown(51))
			{
				if (this.IsConfirming)
				{
					this.OnConfirmationCancel();
				}
				else if (this.IsShowing)
				{
					this.parentMainMenu.PlaySoundOnBack();
					this.SelectedSlot = -1;
					this.IsShowing = false;
				}
			}
			if (this.IsShowing && !this.IsConfirming && this.SelectedSlot >= 0 && player.GetButtonDown(52) && !this.slots[this.SelectedSlot].IsEmpty)
			{
				this.SetConfirming(false, 0);
			}
			if (this.IsShowing && !this.IsConfirming && this.SelectedSlot >= 0 && player.GetButtonDown(61) && !this.slots[this.SelectedSlot].IsEmpty && this.slots[this.SelectedSlot].CanLoad && this.slots[this.SelectedSlot].CanConvertToNewGamePlus)
			{
				this.SetConfirming(true, this.slots[this.SelectedSlot].NewGamePlusUpgrades);
			}
		}

		public void Clear()
		{
			this.IsShowing = false;
			this.IsConfirming = false;
			this.MustConvertToNewgamePlus = false;
			this.SelectedSlot = -1;
			this.SetAccept(false);
			for (int i = 0; i < this.slots.Count; i++)
			{
				SaveSlot saveSlot = this.slots[i];
				saveSlot.SetNumber(i + 1);
				saveSlot.SetData(string.Empty, string.Empty, 0, false, false, false, 0, this.CurrentSlotsMode);
				saveSlot.SetPenitenceConfig(this.PenitencesConfig);
			}
		}

		public void SetAllData(NewMainMenu mainMenu, SelectSaveSlots.SlotsModes mode)
		{
			this.CurrentSlotsMode = mode;
			this.IsConfirming = false;
			this.MustConvertToNewgamePlus = false;
			this.parentMainMenu = mainMenu;
			this.IsShowing = true;
			this.SelectedSlot = -1;
			int num = 0;
			bool flag = false;
			if (this.ConfirmationUpgradeRoot)
			{
				this.ConfirmationUpgradeRoot.transform.parent.gameObject.SetActive(true);
			}
			foreach (SaveSlot saveSlot in this.slots)
			{
				string zoneName = string.Empty;
				string info = string.Empty;
				float num2 = 0f;
				PersistentManager.PublicSlotData slotData = Core.Persistence.GetSlotData(num);
				bool flag2 = slotData != null && (!slotData.persistence.Corrupted || slotData.persistence.HasBackup);
				bool isPlus = false;
				bool canConvert = false;
				int newGamePlusUpgrades = 0;
				if (flag2)
				{
					int num3 = (int)(slotData.persistence.Time / 3600f);
					int num4 = (int)(slotData.persistence.Time % 3600f / 60f);
					string text = string.Empty;
					if (num3 > 0)
					{
						text = num3.ToString() + "h ";
					}
					text = text + num4.ToString() + "m";
					ZoneKey sceneKey = new ZoneKey(slotData.persistence.CurrentDomain, slotData.persistence.CurrentZone, string.Empty);
					zoneName = Core.NewMapManager.GetZoneName(sceneKey);
					float num5 = Mathf.Min(slotData.persistence.Percent, 150f);
					info = Core.Localization.GetValueWithParams(ScriptLocalization.UI_Slot.TEXT_SLOT_INFO, new Dictionary<string, string>
					{
						{
							"playtime",
							text
						},
						{
							"completed",
							num5.ToString("0.##")
						}
					});
					num2 = slotData.persistence.Purge;
					isPlus = slotData.persistence.IsNewGamePlus;
					canConvert = slotData.persistence.CanConvertToNewGamePlus;
					newGamePlusUpgrades = slotData.persistence.NewGamePlusUpgrades;
					saveSlot.SetPenitenceData(slotData.penitence);
				}
				if (slotData != null && slotData.persistence.Corrupted)
				{
					flag = true;
				}
				saveSlot.SetData(zoneName, info, (int)num2, flag2, isPlus, canConvert, newGamePlusUpgrades, this.CurrentSlotsMode);
				num++;
			}
			if (flag)
			{
				base.StartCoroutine(this.ShowCorruptedMenssage());
			}
			else
			{
				this.OnSelectedSlots(0);
			}
		}

		public void OnSelectedSlots(int idxSlot)
		{
			if (!this.IsShowing)
			{
				return;
			}
			if (this.corruptedSaveMessage && this.corruptedSaveMessage.IsShowing)
			{
				return;
			}
			if (this.SelectedSlot >= 0 && this.SelectedSlot != idxSlot)
			{
				this.slots[this.SelectedSlot].SetSelected(false);
			}
			this.SelectedSlot = idxSlot;
			if (this.SelectedSlot >= 0 && this.SelectedSlot < this.slots.Count)
			{
				this.slots[this.SelectedSlot].SetSelected(true);
				this.SetAccept(this.slots[this.SelectedSlot].IsEmpty);
			}
		}

		public void OnAcceptSlots(int idxSlot)
		{
			if (!this.IsShowing)
			{
				return;
			}
			if (this.corruptedSaveMessage && this.corruptedSaveMessage.IsShowing)
			{
				return;
			}
			if (this.CurrentSlotsMode == SelectSaveSlots.SlotsModes.BossRush && this.slots[idxSlot].IsEmpty)
			{
				return;
			}
			this.SelectedSlot = idxSlot;
			this.IsShowing = false;
		}

		public void OnConfirmationDelete()
		{
			this.IsConfirming = false;
			if (this.IsConfirmingUpgrade)
			{
				this.MustConvertToNewgamePlus = true;
				this.IsShowing = false;
			}
			else
			{
				Core.Persistence.DeleteSaveGame(this.SelectedSlot);
				this.SetAllData(this.parentMainMenu, SelectSaveSlots.SlotsModes.Normal);
			}
			this.parentMainMenu.SetNormalModeFromConfirmation();
			this.parentMainMenu.PlaySoundOnBack();
		}

		public void OnConfirmationCancel()
		{
			this.IsConfirming = false;
			this.parentMainMenu.SetNormalModeFromConfirmation();
			this.parentMainMenu.PlaySoundOnBack();
		}

		private void SetAccept(bool isEmpty)
		{
			if (this.buttonNewGame)
			{
				this.buttonNewGame.SetActive(this.CurrentSlotsMode == SelectSaveSlots.SlotsModes.Normal && isEmpty);
			}
			if (this.buttonContinue)
			{
				this.buttonContinue.SetActive(this.CurrentSlotsMode == SelectSaveSlots.SlotsModes.Normal && !isEmpty);
			}
		}

		public IEnumerator ShowCorruptedMenssage()
		{
			this.AllSlots.ForEach(delegate(EventsButton x)
			{
				Navigation navigation = x.navigation;
				navigation.mode = 0;
				x.navigation = navigation;
			});
			this.corruptedSaveMessage.Show();
			yield return new WaitForSecondsRealtime(this.timetoShowCorruptedMessage);
			this.corruptedSaveMessage.Hide();
			this.corruptedSaveMessage.OnHidden += this.OnCorruptedSaveMessageHidden;
			yield break;
		}

		private void OnCorruptedSaveMessageHidden(ShowHideUIItem item)
		{
			this.corruptedSaveMessage.OnHidden -= this.OnCorruptedSaveMessageHidden;
			this.AllSlots.ForEach(delegate(EventsButton x)
			{
				Navigation navigation = x.navigation;
				navigation.mode = 4;
				x.navigation = navigation;
			});
			this.OnSelectedSlots(0);
		}

		private void SetConfirming(bool IsUpgrade, int NumberOfNewGamePlusUpgrades)
		{
			this.IsConfirming = true;
			this.IsConfirmingUpgrade = IsUpgrade;
			this.ConfirmationUpgradeRoot.SetActive(IsUpgrade && NumberOfNewGamePlusUpgrades == 0);
			this.ConfirmationUpgradePlusRoot.SetActive(IsUpgrade && NumberOfNewGamePlusUpgrades > 0);
			this.ConfirmationDeleteRoot.SetActive(!IsUpgrade);
			this.parentMainMenu.SetConfirmationDeleteFromSlot();
		}

		[SerializeField]
		[BoxGroup("Elements", true, false, 0)]
		private GameObject buttonNewGame;

		[SerializeField]
		[BoxGroup("Elements", true, false, 0)]
		private GameObject buttonContinue;

		[SerializeField]
		[BoxGroup("Elements", true, false, 0)]
		private GameObject ConfirmationUpgradeRoot;

		[SerializeField]
		[BoxGroup("Elements", true, false, 0)]
		private GameObject ConfirmationUpgradePlusRoot;

		[SerializeField]
		[BoxGroup("Elements", true, false, 0)]
		private GameObject ConfirmationDeleteRoot;

		[SerializeField]
		[BoxGroup("Elements", true, false, 0)]
		private List<EventsButton> AllSlots;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float timetoShowCorruptedMessage = 2f;

		[SerializeField]
		[BoxGroup("Penitence", true, false, 0)]
		private List<SelectSaveSlots.PenitenceData> PenitencesConfig = new List<SelectSaveSlots.PenitenceData>();

		[SerializeField]
		[BoxGroup("Slots", true, false, 0)]
		private List<SaveSlot> slots = new List<SaveSlot>();

		private List<GameObject> options = new List<GameObject>();

		private NewMainMenu parentMainMenu;

		private SelectSaveSlots.SlotsModes CurrentSlotsMode;

		public ShowHideUIItem corruptedSaveMessage;

		[Serializable]
		public class PenitenceData
		{
			public string id;

			public Sprite InProgress;

			public Sprite Completed;

			public Sprite Missing;
		}

		public enum SlotsModes
		{
			Normal,
			BossRush
		}
	}
}
