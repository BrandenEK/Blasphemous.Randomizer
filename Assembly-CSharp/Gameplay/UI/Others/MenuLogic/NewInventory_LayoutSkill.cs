using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI.Others.Buttons;
using Gameplay.UI.Others.UIGameLogic;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class NewInventory_LayoutSkill : NewInventory_Layout
	{
		private IList<string> SkillsValues()
		{
			return null;
		}

		private void Awake()
		{
			this.unlockMask.fillAmount = 0f;
			this.timePressingUnlock = 0f;
			this.ignoreSelectSound = false;
			this.inEditMode = false;
			if (this.objectsCached)
			{
				return;
			}
			this.cachedSkills = new List<NewInventory_Skill>();
			this.objectsCached = true;
			int num = 0;
			NewInventory_Skill[] componentsInChildren = this.skillsRoot.GetComponentsInChildren<NewInventory_Skill>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				NewInventory_Skill newInventory_Skill = componentsInChildren[i];
				if (newInventory_Skill.GetSkillId() == this.initialSkill)
				{
					this.initialSlot = num;
				}
				int elementNumber = num;
				EventsButton eventButton = newInventory_Skill.GetEventButton();
				eventButton.onClick = new EventsButton.ButtonClickedEvent();
				eventButton.onClick.AddListener(delegate()
				{
					this.ActivateSkill(elementNumber);
				});
				eventButton.onSelected = new EventsButton.ButtonSelectedEvent();
				eventButton.onSelected.AddListener(delegate()
				{
					this.SelectSkill(elementNumber, true);
				});
				this.cachedSkills.Add(newInventory_Skill);
				num++;
			}
		}

		private void Update()
		{
			NewInventory_Skill newInventory_Skill = null;
			if (this.inEditMode)
			{
				bool flag = false;
				if (this.currentSelected != -1 && this.currentSelected < this.cachedSkills.Count)
				{
					newInventory_Skill = this.cachedSkills[this.currentSelected];
					flag = Core.SkillManager.CanUnlockSkill(newInventory_Skill.GetSkillId(), false);
				}
				if (flag)
				{
					if (this.rewired == null)
					{
						this.rewired = ReInput.players.GetPlayer(0);
					}
					if (this.rewired.GetButton(52))
					{
						this.timePressingUnlock += Time.unscaledDeltaTime;
						this.PlayLoadingPurchaseFx(out this._loadingPurchaseFxEvent);
						if (this.timePressingUnlock >= this.timeToUnlockSKill)
						{
							this.timePressingUnlock = 0f;
							Core.SkillManager.UnlockSkill(newInventory_Skill.GetSkillId(), false);
							this.UpdateAllSKills();
							this.SelectSkill(this.currentSelected, false);
							Core.Audio.PlayOneShot(this.soundUnlockSkill, default(Vector3));
							this.StopLoadingPurchaseFx(ref this._loadingPurchaseFxEvent);
						}
					}
					else
					{
						this.timePressingUnlock = 0f;
						this.StopLoadingPurchaseFx(ref this._loadingPurchaseFxEvent);
					}
				}
				else
				{
					this.timePressingUnlock = 0f;
				}
				this.unlockMask.fillAmount = this.timePressingUnlock / this.timeToUnlockSKill;
			}
		}

		public override void RestoreFromLore()
		{
			base.StartCoroutine(this.FocusSkillSecure((this.currentSelected == -1) ? 0 : this.currentSelected, true));
		}

		public override void RestoreSlotPosition(int slotPosition)
		{
			base.StartCoroutine(this.FocusSkillSecure(slotPosition, true));
		}

		public override int GetLastSlotSelected()
		{
			return this.currentSelected;
		}

		public void CancelEditMode()
		{
			this.inEditMode = false;
		}

		public override void ShowLayout(NewInventoryWidget.TabType tabType, bool editMode)
		{
			if (this.purgeControl)
			{
				this.purgeControl.RefreshPoints(true);
			}
			this.inEditMode = editMode;
			this.UpdateAllSKills();
			this.maxTier.text = Core.SkillManager.GetCurrentMeaCulpa().ToString();
			base.StartCoroutine(this.FocusSkillSecure(this.initialSlot, true));
		}

		public override void GetSelectedLoreData(out string caption, out string lore)
		{
			caption = string.Empty;
			lore = string.Empty;
		}

		public override int GetItemPosition(BaseInventoryObject item)
		{
			for (int i = 0; i < this.cachedSkills.Count; i++)
			{
				if (this.cachedSkills[i] == item)
				{
					return i;
				}
			}
			return this.GetLastSlotSelected();
		}

		public void ActivateSkill(int skill)
		{
		}

		public void SelectSkill(int skill, bool playSound = true)
		{
			this.timePressingUnlock = 0f;
			if (this.currentSelected != -1 && this.currentSelected < this.cachedSkills.Count)
			{
				this.cachedSkills[this.currentSelected].SetFocus(false, this.inEditMode);
			}
			this.currentSelected = skill;
			NewInventory_Skill newInventory_Skill = this.cachedSkills[this.currentSelected];
			if (this.description)
			{
				this.description.SetKill(newInventory_Skill.GetSkillId(), this.inEditMode);
			}
			if (playSound)
			{
				if (this.ignoreSelectSound)
				{
					this.ignoreSelectSound = false;
				}
				else
				{
					Core.Audio.PlayOneShot(this.soundElementChange, default(Vector3));
				}
			}
			newInventory_Skill.SetFocus(true, this.inEditMode);
		}

		private IEnumerator FocusSkillSecure(int skill, bool ignoreSound = true)
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			if (this.cachedSkills.Count > skill)
			{
				GameObject gameObject = this.cachedSkills[skill].gameObject;
				this.ignoreSelectSound = ignoreSound;
				EventSystem.current.SetSelectedGameObject(gameObject);
				this.SelectSkill(skill, false);
			}
			yield break;
		}

		private void UpdateAllSKills()
		{
			foreach (NewInventory_Skill newInventory_Skill in this.cachedSkills)
			{
				newInventory_Skill.UpdateStatus();
				newInventory_Skill.SetFocus(false, this.inEditMode);
			}
		}

		public void PlayLoadingPurchaseFx(out EventInstance eventInstance)
		{
			if (string.IsNullOrEmpty("event:/SFX/UI/BuySkill"))
			{
				eventInstance = default(EventInstance);
				return;
			}
			if (this._loadingPurchaseFxEvent.isValid())
			{
				eventInstance = this._loadingPurchaseFxEvent;
				return;
			}
			eventInstance = Core.Audio.CreateEvent("event:/SFX/UI/BuySkill", default(Vector3));
			eventInstance.start();
		}

		public void StopLoadingPurchaseFx(ref EventInstance eventInstance)
		{
			if (!eventInstance.isValid())
			{
				return;
			}
			eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			eventInstance.release();
		}

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private GameObject skillsRoot;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private NewInventory_Description description;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private Text maxTier;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private PlayerPurgePoints purgeControl;

		[BoxGroup("Sounds", true, false, 0)]
		[SerializeField]
		private const string loadingPurchaseFx = "event:/SFX/UI/BuySkill";

		private EventInstance _loadingPurchaseFxEvent;

		[SerializeField]
		[BoxGroup("Unlock", true, false, 0)]
		private float timeToUnlockSKill = 2f;

		[SerializeField]
		[BoxGroup("Unlock", true, false, 0)]
		private Image unlockMask;

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundElementChange = "event:/SFX/UI/ChangeSelection";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundUnlockSkill = "event:/SFX/UI/EquipPrayer";

		[SerializeField]
		[BoxGroup("Main", true, false, 0)]
		[ValueDropdown("SkillsValues")]
		private string initialSkill;

		private List<NewInventory_Skill> cachedSkills;

		private bool objectsCached;

		private int currentSelected = -1;

		private bool ignoreSelectSound;

		private bool inEditMode;

		private int initialSlot;

		private float timePressingUnlock;

		private Player rewired;
	}
}
