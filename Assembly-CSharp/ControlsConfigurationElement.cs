using System;
using Framework.Managers;
using Sirenix.OdinInspector;
using Tools.UI;
using UnityEngine;
using UnityEngine.UI;

public class ControlsConfigurationElement : MonoBehaviour
{
	private void ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES newMode)
	{
		this.ExitMode(this.currentMode);
		this.currentMode = newMode;
		this.EnterMode(this.currentMode);
	}

	private void OnEnterSelected()
	{
		this.actionText.color = this.textColorSelected;
		Core.Audio.PlayOneShot(this.OnMoveAudio, default(Vector3));
		this.selectedModeIcon.enabled = true;
		this.conflictText.enabled = false;
	}

	private void OnEnterUnselected()
	{
		this.actionText.color = this.textColorUnselected;
		this.selectedModeIcon.enabled = false;
		this.editModeIcon.enabled = false;
		this.inputIconImage.enabled = true;
		this.conflictText.enabled = false;
	}

	private void OnEnterEditing()
	{
		this.actionText.color = this.textColorEditing;
		Core.Audio.PlayOneShot(this.OnClickAudio, default(Vector3));
		this.editModeIcon.enabled = true;
		this.inputIconImage.enabled = false;
		this.inputIcon.SetIconByButtonKey(string.Empty);
		ControlRemapManager.InputMappedEvent += this.OnAssignedButtonChanged;
		Core.ControlRemapManager.StartListeningInput(this.currentActionElementMapId);
	}

	private void OnEnterConflictSelected()
	{
		this.actionText.color = this.textColorConflict;
		Core.Audio.PlayOneShot(this.OnMoveAudio, default(Vector3));
		this.selectedModeIcon.enabled = true;
		this.conflictText.enabled = true;
	}

	private void OnEnterConflictUnselected()
	{
		this.actionText.color = this.textColorConflict;
		this.selectedModeIcon.enabled = false;
		this.editModeIcon.enabled = false;
		this.inputIconImage.enabled = true;
		this.conflictText.enabled = true;
	}

	private void OnEnterGreyedOut()
	{
		this.actionText.color = this.textColorGreyedOut;
	}

	private void OnExitSelected()
	{
	}

	private void OnExitUnselected()
	{
	}

	private void OnExitEditing()
	{
		Core.Audio.PlayOneShot(this.OnClickAudio, default(Vector3));
		this.editModeIcon.enabled = false;
		this.inputIconImage.enabled = true;
		this.inputIcon.SetIconByButtonKey(this.currentButtonKey);
		Core.ControlRemapManager.StopListeningInput();
	}

	private void OnExitConflictSelected()
	{
	}

	private void OnExitConflictUnselected()
	{
	}

	private void OnExitGreyedOut()
	{
	}

	private void EnterMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES m)
	{
		switch (m)
		{
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.UNSELECTED:
			this.OnEnterUnselected();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.SELECTED:
			this.OnEnterSelected();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.EDITING:
			this.OnEnterEditing();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_UNSELECTED:
			this.OnEnterConflictUnselected();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_SELECTED:
			this.OnEnterConflictSelected();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.GREYEDOUT:
			this.OnEnterGreyedOut();
			break;
		}
	}

	private void ExitMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES m)
	{
		switch (m)
		{
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.UNSELECTED:
			this.OnExitUnselected();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.SELECTED:
			this.OnExitSelected();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.EDITING:
			this.OnExitEditing();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_UNSELECTED:
			this.OnExitConflictUnselected();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_SELECTED:
			this.OnExitConflictSelected();
			break;
		case ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.GREYEDOUT:
			this.OnExitGreyedOut();
			break;
		}
	}

	public void Init(string actionName, string defaultButton, int actionElementMapId)
	{
		this.currentActionName = actionName;
		this.currentActionElementMapId = actionElementMapId;
		this.actionText.text = Core.ControlRemapManager.LocalizeActionName(this.currentActionName);
		this.SetButton(defaultButton);
	}

	public int GetCurrentActionElementMapId()
	{
		return this.currentActionElementMapId;
	}

	public string GetCurrentButtonKey()
	{
		return this.currentButtonKey;
	}

	public void SetButton(string button)
	{
		this.currentButtonKey = button;
		this.inputIcon.isControlsRemappingInputIcon = true;
		this.inputIcon.SetIconByButtonKey(this.currentButtonKey);
	}

	public void OnAssignedButtonChanged(string newButton, int actionElementMapId)
	{
		this.currentActionElementMapId = actionElementMapId;
		this.SetButton(newButton);
		this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.SELECTED);
	}

	public void OnElementUnselected()
	{
		if (this.currentMode == ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_SELECTED)
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_UNSELECTED);
		}
		else
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.UNSELECTED);
		}
	}

	public void OnElementSelected()
	{
		if (this.currentMode == ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_UNSELECTED)
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_SELECTED);
		}
		else
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.SELECTED);
		}
	}

	public void OnElementToogleGreyOut()
	{
		if (this.currentMode == ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.UNSELECTED)
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.GREYEDOUT);
		}
		else if (this.currentMode == ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.GREYEDOUT)
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.UNSELECTED);
		}
	}

	public void OnEditPressed()
	{
		this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.EDITING);
	}

	public void OnElementMarkedAsConflicting()
	{
		if (this.currentMode == ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.UNSELECTED)
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_UNSELECTED);
		}
		else if (this.currentMode == ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.SELECTED)
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_SELECTED);
		}
	}

	public void OnElementUnmarkedAsConflicting()
	{
		if (this.currentMode == ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_UNSELECTED)
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.UNSELECTED);
		}
		else if (this.currentMode == ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.CONFLICT_SELECTED)
		{
			this.ChangeMode(ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES.SELECTED);
		}
	}

	public Text actionText;

	public Text conflictText;

	public InputIcon inputIcon;

	public Image inputIconImage;

	public Image editModeIcon;

	public Image selectedModeIcon;

	[SerializeField]
	[FoldoutGroup("Colors", false, 0)]
	public Color textColorUnselected;

	[SerializeField]
	[FoldoutGroup("Colors", false, 0)]
	public Color textColorSelected;

	[SerializeField]
	[FoldoutGroup("Colors", false, 0)]
	public Color textColorConflict;

	[SerializeField]
	[FoldoutGroup("Colors", false, 0)]
	public Color textColorEditing;

	[SerializeField]
	[FoldoutGroup("Colors", false, 0)]
	public Color textColorGreyedOut;

	[SerializeField]
	[FoldoutGroup("Audio", false, 0)]
	public string OnMoveAudio;

	[SerializeField]
	[FoldoutGroup("Audio", false, 0)]
	public string OnClickAudio;

	private string currentActionName;

	private string currentButtonKey;

	private int currentActionElementMapId;

	private ControlsConfigurationElement.CONTROLS_CONFIGURATION_ELEMENT_MODES currentMode;

	public enum CONTROLS_CONFIGURATION_ELEMENT_MODES
	{
		UNSELECTED,
		SELECTED,
		EDITING,
		CONFLICT_UNSELECTED,
		CONFLICT_SELECTED,
		GREYEDOUT
	}
}
