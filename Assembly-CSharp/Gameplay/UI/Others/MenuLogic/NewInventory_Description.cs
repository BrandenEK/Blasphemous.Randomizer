using System;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class NewInventory_Description : MonoBehaviour
	{
		public void SetObject(BaseInventoryObject invObj, NewInventoryWidget.EquipAction action)
		{
			if (invObj)
			{
				this.objectImage.sprite = invObj.picture;
				this.objectImage.enabled = true;
				this.caption.text = invObj.caption;
				this.description.text = invObj.description;
				this.buttonLore.SetActive(invObj.HasLore());
			}
			else
			{
				this.objectImage.enabled = false;
				this.caption.text = string.Empty;
				this.description.text = string.Empty;
				this.buttonLore.SetActive(false);
			}
			this.buttonEquip.SetActive(invObj && invObj.IsEquipable() && action != NewInventoryWidget.EquipAction.UnEquip);
			this.buttonUnEquip.SetActive(invObj && invObj.IsEquipable() && action == NewInventoryWidget.EquipAction.UnEquip);
			this.buttonDechiper.SetActive(false);
			if (invObj && action == NewInventoryWidget.EquipAction.Equip)
			{
				this.SetColorButton(this.buttonEquip, Color.white, this.colorTextNormal);
			}
			else
			{
				this.SetColorButton(this.buttonEquip, this.colorDisable, this.colorDisable);
			}
			this.customScrollView.NewContentSetted();
		}

		public void SetKill(string skillId, bool editmode)
		{
			if (this.objectImage)
			{
				this.objectImage.enabled = false;
			}
			if (this.buttonEquip)
			{
				this.buttonEquip.SetActive(false);
			}
			if (this.buttonUnEquip)
			{
				this.buttonUnEquip.SetActive(false);
			}
			if (this.buttonDechiper)
			{
				this.buttonDechiper.SetActive(false);
			}
			if (this.buttonLore)
			{
				this.buttonLore.SetActive(false);
			}
			UnlockableSkill skill = Core.SkillManager.GetSkill(skillId);
			this.caption.text = skill.caption;
			this.description.text = skill.description + "\n";
			if (this.instructionsPro)
			{
				this.instructionsPro.text = LocalizationManager.ParseMeshPro(skill.instructions, "SKILL_" + skillId, this.instructionsPro);
			}
			else
			{
				this.instructions.text = skill.instructions;
			}
			this.helpText.SetActive(!editmode);
			if (editmode && Core.SkillManager.CanUnlockSkill(skillId, false))
			{
				this.buttonUnlock.SetActive(true);
			}
			else
			{
				this.buttonUnlock.SetActive(false);
			}
			if (this.bigImageSkill)
			{
				this.bigImageSkill.gameObject.SetActive(skill.bigImage != null);
				this.bigImageSkill.sprite = skill.bigImage;
			}
			this.buttonExit.SetActive(editmode);
			this.customScrollView.NewContentSetted();
		}

		public void SetColorButton(GameObject button, Color buttonColor, Color textColor)
		{
			if (!button)
			{
				return;
			}
			button.transform.GetComponentInChildren<Image>().color = buttonColor;
			button.transform.GetComponentInChildren<Text>().color = textColor;
		}

		public void EnabledInput(bool value)
		{
			this.customScrollView.InputEnabled = value;
		}

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private Image objectImage;

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private Text caption;

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private Text description;

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private Color colorDisable = new Color(0.243f, 0.243f, 0.243f);

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private Color colorTextNormal = new Color(0.545f, 0.522f, 0.376f);

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private CustomScrollView customScrollView;

		[SerializeField]
		[BoxGroup("Skill", true, false, 0)]
		private Text instructions;

		[SerializeField]
		[BoxGroup("Skill", true, false, 0)]
		private TextMeshProUGUI instructionsPro;

		[SerializeField]
		[BoxGroup("Inventory", true, false, 0)]
		private GameObject buttonEquip;

		[SerializeField]
		[BoxGroup("Inventory", true, false, 0)]
		private GameObject buttonUnEquip;

		[SerializeField]
		[BoxGroup("Inventory", true, false, 0)]
		private GameObject buttonDechiper;

		[SerializeField]
		[BoxGroup("Inventory", true, false, 0)]
		private GameObject buttonLore;

		[SerializeField]
		[BoxGroup("Skill", true, false, 0)]
		private GameObject buttonUnlock;

		[SerializeField]
		[BoxGroup("Skill", true, false, 0)]
		private GameObject buttonExit;

		[SerializeField]
		[BoxGroup("Skill", true, false, 0)]
		private GameObject helpText;

		[SerializeField]
		[BoxGroup("Skill", true, false, 0)]
		private Image bigImageSkill;
	}
}
