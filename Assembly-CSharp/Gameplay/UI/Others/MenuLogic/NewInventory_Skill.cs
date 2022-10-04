using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.UI.Others.Buttons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class NewInventory_Skill : MonoBehaviour
	{
		public string GetSkillId()
		{
			return this.skill;
		}

		public EventsButton GetEventButton()
		{
			return base.GetComponent<EventsButton>();
		}

		private IList<string> SkillsValues()
		{
			return null;
		}

		public void Awake()
		{
			if (this.focus)
			{
				this.focus.SetActive(false);
			}
		}

		public void UpdateStatus()
		{
			UnlockableSkill unlockableSkill = Core.SkillManager.GetSkill(this.skill);
			this.backgorundLocked.SetActive(false);
			this.backgorundUnLocked.SetActive(false);
			this.backgorundEnabled.SetActive(false);
			this.tierText.text = string.Empty;
			bool flag = false;
			UnlockableSkill unlockableSkill2 = Core.SkillManager.GetSkill(this.skill);
			this.skillImage.sprite = unlockableSkill2.smallImage;
			if (Core.SkillManager.IsSkillUnlocked(this.skill))
			{
				this.backgorundUnLocked.SetActive(true);
				this.skillImage.sprite = unlockableSkill2.smallImageBuyed;
			}
			else if (Core.SkillManager.CanUnlockSkillNoCheckPoints(this.skill))
			{
				this.backgorundEnabled.SetActive(true);
			}
			else
			{
				this.tierText.text = unlockableSkill.tier.ToString();
				this.tierText.color = this.disabledColor;
				flag = true;
			}
			this.backgorundLocked.SetActive(flag);
			this.skillImage.gameObject.SetActive(!flag);
		}

		public void SetFocus(bool bFocus, bool editMode)
		{
			UnlockableSkill unlockableSkill = Core.SkillManager.GetSkill(this.skill);
			bool flag = bFocus && !Core.SkillManager.IsSkillUnlocked(this.skill) && (editMode || Core.SkillManager.CanUnlockSkillNoCheckPoints(this.skill));
			this.focus.SetActive(bFocus);
			this.cost.SetActive(flag);
			this.costText.gameObject.SetActive(flag);
			bool flag2 = true;
			if (flag)
			{
				if (!Core.SkillManager.CanUnlockSkillNoCheckPoints(this.skill))
				{
					this.costText.text = "???";
				}
				else
				{
					this.costText.text = unlockableSkill.cost.ToString();
					flag2 = (!Core.SkillManager.CanUnlockSkill(this.skill, false) || !editMode);
				}
			}
			this.costText.color = ((!flag2) ? this.nomalColor : this.disabledColor);
		}

		[SerializeField]
		[BoxGroup("Main", true, false, 0)]
		[ValueDropdown("SkillsValues")]
		private string skill;

		[SerializeField]
		[BoxGroup("Main", true, false, 0)]
		private Color nomalColor;

		[SerializeField]
		[BoxGroup("Main", true, false, 0)]
		private Color disabledColor;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private GameObject backgorundLocked;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private GameObject backgorundUnLocked;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private GameObject backgorundEnabled;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private Image skillImage;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private GameObject focus;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private GameObject cost;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private Text costText;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Text tierText;
	}
}
