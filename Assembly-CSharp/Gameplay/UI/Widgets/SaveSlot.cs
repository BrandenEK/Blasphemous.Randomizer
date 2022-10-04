using System;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Widgets
{
	public class SaveSlot : MonoBehaviour
	{
		public bool IsEmpty { get; private set; }

		public bool CanLoad { get; private set; }

		public bool IsNewGamePlus { get; private set; }

		public bool CanConvertToNewGamePlus { get; private set; }

		public int NewGamePlusUpgrades { get; private set; }

		public void SetNumber(int slot)
		{
			this.SlotNumber.text = slot.ToString();
		}

		public void SetData(string zoneName, string info, int purge, bool canLoad, bool isPlus, bool canConvert, int newGamePlusUpgrades, SelectSaveSlots.SlotsModes mode)
		{
			this.CurrentMode = mode;
			this.CanLoad = canLoad;
			this.IsEmpty = (zoneName == string.Empty);
			this.IsNewGamePlus = isPlus;
			this.CanConvertToNewGamePlus = canConvert;
			this.ZoneText.text = zoneName;
			this.InfoText.text = info;
			this.PurgeText.text = purge.ToString();
			this.Penitence.gameObject.SetActive(!this.IsEmpty);
			this.PurgeText.gameObject.SetActive(!this.IsEmpty);
			this.NewGamePlusUpgrades = newGamePlusUpgrades;
			this.SetSelected(false);
		}

		public void SetPenitenceData(PenitenceManager.PenitencePersistenceData data)
		{
			this.Penitence.UpdateFromSavegameData(data);
		}

		public void SetPenitenceConfig(List<SelectSaveSlots.PenitenceData> data)
		{
			this.Penitence.SetPenitenceConfig(data);
		}

		public void SetSelected(bool selected)
		{
			Sprite sprite = this.EmptyBackgorund;
			if (!this.IsEmpty)
			{
				if (this.IsNewGamePlus)
				{
					sprite = ((!selected) ? this.PlusUnSelectedBackgorund : this.PlusSelectedBackgorund);
				}
				else
				{
					sprite = ((!selected) ? this.NormalUnSelectedBackgorund : this.NormalSelectedBackgorund);
				}
			}
			this.Background.sprite = sprite;
			if (this.ButtonUpgradeGamePlus)
			{
				this.ButtonUpgradeGamePlus.SetActive(selected && this.CanConvertToNewGamePlus && this.CurrentMode == SelectSaveSlots.SlotsModes.Normal);
			}
			if (this.ButtonDelete)
			{
				this.ButtonDelete.SetActive(selected && !this.IsEmpty && this.CurrentMode == SelectSaveSlots.SlotsModes.Normal);
			}
			Color color = (!selected) ? this.ColorUnselected : this.ColorSelected;
			this.SlotNumber.color = color;
			this.ZoneText.color = color;
			this.PurgeText.color = color;
			if (this.ChangeInfoColor)
			{
				this.InfoText.color = color;
			}
		}

		[SerializeField]
		[BoxGroup("Buttons", true, false, 0)]
		private GameObject ButtonDelete;

		[SerializeField]
		[BoxGroup("Buttons", true, false, 0)]
		private GameObject ButtonUpgradeGamePlus;

		[SerializeField]
		[BoxGroup("Text", true, false, 0)]
		private Text SlotNumber;

		[SerializeField]
		[BoxGroup("Text", true, false, 0)]
		private Text ZoneText;

		[SerializeField]
		[BoxGroup("Text", true, false, 0)]
		private Text PurgeText;

		[SerializeField]
		[BoxGroup("Text", true, false, 0)]
		private Text InfoText;

		[SerializeField]
		[BoxGroup("Text", true, false, 0)]
		private Color ColorSelected;

		[SerializeField]
		[BoxGroup("Text", true, false, 0)]
		private Color ColorUnselected;

		[SerializeField]
		[BoxGroup("Text", true, false, 0)]
		private bool ChangeInfoColor = true;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private PenitenceSlot Penitence;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Image Background;

		[SerializeField]
		[BoxGroup("Backgorunds", true, false, 0)]
		private Sprite EmptyBackgorund;

		[SerializeField]
		[BoxGroup("Backgorunds", true, false, 0)]
		private Sprite NormalSelectedBackgorund;

		[SerializeField]
		[BoxGroup("Backgorunds", true, false, 0)]
		private Sprite NormalUnSelectedBackgorund;

		[SerializeField]
		[BoxGroup("Backgorunds", true, false, 0)]
		private Sprite PlusSelectedBackgorund;

		[SerializeField]
		[BoxGroup("Backgorunds", true, false, 0)]
		private Sprite PlusUnSelectedBackgorund;

		private SelectSaveSlots.SlotsModes CurrentMode;
	}
}
