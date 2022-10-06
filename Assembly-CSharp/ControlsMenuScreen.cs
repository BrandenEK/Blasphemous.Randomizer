using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Rewired;
using UnityEngine;

public class ControlsMenuScreen : BaseMenuScreen
{
	public bool currentlyActive
	{
		get
		{
			return base.gameObject.activeInHierarchy && this.canvasGroup != null && this.canvasGroup.alpha == 1f;
		}
	}

	public override void Open()
	{
		base.Open();
		base.gameObject.SetActive(true);
		this.canvasGroup = base.GetComponent<CanvasGroup>();
		this.canvasGroup.alpha = 1f;
		this.elements = new List<ControlsConfigurationElement>();
		IEnumerator enumerator = this.contentTransform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				RectTransform rectTransform = (RectTransform)obj;
				ControlsConfigurationElement component = rectTransform.GetComponent<ControlsConfigurationElement>();
				component.OnElementUnselected();
				this.elements.Add(component);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		this.index = 0;
		this.indexOfFirstElementShown = 0;
		this.indexOfLastElementShown = this.maxNumberOfRowsShown - 1;
		this.currentOffset = 0;
		this.contentTransform.anchoredPosition = new Vector2(0f, 0f);
		this.Init();
		this.UpdateAllElements();
		this.elements[this.index].OnElementSelected();
		this.OnOpen();
	}

	private void Init()
	{
		if (!this.initialized)
		{
			this.rewiredPlayer = ReInput.players.GetPlayer(0);
			this.actionNames = Core.ControlRemapManager.GetAllActionNamesInOrder();
			this.initialized = true;
		}
	}

	private void UpdateAllElements()
	{
		this.UnmarkPreviousConflictingActions();
		bool flag = false;
		Dictionary<string, ActionElementMap> rebindeableAemsByActionName = Core.ControlRemapManager.GetRebindeableAemsByActionName(this.actionNames);
		if (rebindeableAemsByActionName.Keys.Count != this.elements.Count)
		{
			Debug.Log("ControlsMenuScreen: UpdateAllElements: The number of elements isn't equal to the number of actions!");
			flag = true;
		}
		if (!flag)
		{
			int num = 0;
			foreach (string text in rebindeableAemsByActionName.Keys)
			{
				if (rebindeableAemsByActionName[text] == null)
				{
					Debug.Log("ControlsMenuScreen: UpdateAllElements: it seems that action '" + text + "' wasn't present in a previous configuration of the game.");
					flag = true;
					break;
				}
				this.elements[num].Init(text, rebindeableAemsByActionName[text].elementIdentifierName, rebindeableAemsByActionName[text].id);
				num++;
			}
		}
		if (flag)
		{
			Core.ControlRemapManager.RestoreDefaultMaps();
			rebindeableAemsByActionName = Core.ControlRemapManager.GetRebindeableAemsByActionName(this.actionNames);
			int num2 = 0;
			foreach (string text2 in rebindeableAemsByActionName.Keys)
			{
				this.elements[num2].Init(text2, rebindeableAemsByActionName[text2].elementIdentifierName, rebindeableAemsByActionName[text2].id);
				num2++;
			}
		}
	}

	public bool TryClose()
	{
		if (!this.editing)
		{
			if (Core.ControlRemapManager.CountConflictingActions() == 0)
			{
				Core.ControlRemapManager.WriteControlsSettingsToFile();
			}
			else
			{
				Core.ControlRemapManager.LoadKeyboardAndMouseControlsSettingsFromFile();
				if (Core.Input.ActiveController.type == 2)
				{
					Core.ControlRemapManager.LoadJoystickControlsSettingsFromFile((Joystick)Core.Input.ActiveController);
				}
			}
			this.Close();
		}
		return !this.editing;
	}

	public override void Close()
	{
		base.Close();
		this.canvasGroup.alpha = 0f;
		this.OnClose();
	}

	protected override void OnOpen()
	{
		base.OnOpen();
		Core.Input.JoystickPressed += this.UpdateAllElements;
		Core.Input.KeyboardPressed += this.UpdateAllElements;
		this.navigationButtonsRoot.SetActive(false);
		this.remappingLeyendButtonsRoot.SetActive(true);
		this.saveAndExitLeyendButton.SetActive(true);
		this.cancelLeyendButton.SetActive(false);
		this.editLeyendButton.SetActive(true);
		this.acceptLeyendButton.SetActive(false);
		this.restoreDefaultsLeyendButton.SetActive(true);
		this.leftStickDisclaimer.SetActive(false);
	}

	private void Update()
	{
		if (!this.initialized)
		{
			return;
		}
		bool buttonDown = this.rewiredPlayer.GetButtonDown(50);
		if (buttonDown)
		{
			this.ProcessSubmitInput();
			return;
		}
		bool buttonDown2 = this.rewiredPlayer.GetButtonDown(52);
		if (buttonDown2)
		{
			this.ProcessRestoreDefaultInput();
			return;
		}
		float axisRaw = this.rewiredPlayer.GetAxisRaw(49);
		if (Mathf.Abs(axisRaw) > this.axisThreshold)
		{
			this.ProcessScrollInput(axisRaw);
		}
		if (Input.GetKeyDown(27))
		{
			this.ProcessSubmitInput();
		}
	}

	private void ProcessSubmitInput()
	{
		this.editing = !this.editing;
		for (int i = 0; i < this.elements.Count; i++)
		{
			if (i == this.index)
			{
				if (this.editing)
				{
					this.elements[i].OnEditPressed();
					ControlRemapManager.InputMappedEvent += this.ProcessAssignmentInput;
				}
				else
				{
					this.elements[i].OnElementSelected();
				}
			}
			else if (Core.Input.ActiveControllerType != 2 || (Core.Input.ActiveControllerType == 2 && this.editing))
			{
				this.elements[i].OnElementToogleGreyOut();
			}
		}
		this.editLeyendButton.SetActive(!this.editing);
		this.restoreDefaultsLeyendButton.SetActive(!this.editing);
		if (Core.Input.ActiveControllerType == 2)
		{
			this.acceptLeyendButton.SetActive(false);
			this.leftStickDisclaimer.SetActive(this.editing);
		}
		else
		{
			this.acceptLeyendButton.SetActive(this.editing);
			this.leftStickDisclaimer.SetActive(false);
		}
		if (!this.editing)
		{
			bool flag = Core.ControlRemapManager.CountConflictingActions() == 0;
			this.saveAndExitLeyendButton.SetActive(!flag);
			this.cancelLeyendButton.SetActive(flag);
		}
		else
		{
			this.saveAndExitLeyendButton.SetActive(false);
			this.cancelLeyendButton.SetActive(false);
		}
	}

	private void ProcessRestoreDefaultInput()
	{
		if (!this.editing)
		{
			Core.ControlRemapManager.RestoreDefaultMaps();
			this.UpdateAllElements();
			this.UnmarkPreviousConflictingActions();
			this.saveAndExitLeyendButton.SetActive(true);
			this.cancelLeyendButton.SetActive(false);
		}
	}

	private void ProcessScrollInput(float scrollAxis)
	{
		float axisRawPrev = this.rewiredPlayer.GetAxisRawPrev(49);
		if (axisRawPrev == 0f)
		{
			this.framesSkipped = 0;
			if (!this.editing)
			{
				this.CalculateNewIndex(scrollAxis);
			}
		}
		else
		{
			float axisTimeActive = this.rewiredPlayer.GetAxisTimeActive(49);
			int num = (axisTimeActive <= this.delaySecondsForFastScroll) ? this.skippedFramesForSlowScroll : this.skippedFramesForFastScroll;
			this.framesSkipped++;
			if (this.framesSkipped % num == 0)
			{
				this.framesSkipped = 0;
				if (!this.editing)
				{
					this.CalculateNewIndex(scrollAxis);
				}
			}
		}
	}

	private void CalculateNewIndex(float scrollAxis)
	{
		int num = this.index;
		num = ((scrollAxis <= 0f) ? (num + 1) : (num - 1));
		num = Mathf.Clamp(num, 0, this.elements.Count - 1);
		if (num != this.index)
		{
			this.UpdateIndexAndElements(num);
		}
	}

	private void ProcessAssignmentInput(string buttonName, int actionElementMapId)
	{
		for (int i = 0; i < this.elements.Count; i++)
		{
			if (i != this.index)
			{
				this.elements[i].OnElementToogleGreyOut();
			}
		}
		Dictionary<int, string> allCurrentConflictingButtonsByAemId = Core.ControlRemapManager.GetAllCurrentConflictingButtonsByAemId();
		bool flag = allCurrentConflictingButtonsByAemId.Keys.Count > 0;
		this.UnmarkPreviousConflictingActions();
		if (flag)
		{
			this.MarkCurrentConflictingActions(allCurrentConflictingButtonsByAemId);
		}
		this.saveAndExitLeyendButton.SetActive(!flag);
		this.cancelLeyendButton.SetActive(flag);
		this.editLeyendButton.SetActive(true);
		this.acceptLeyendButton.SetActive(false);
		this.restoreDefaultsLeyendButton.SetActive(true);
		this.leftStickDisclaimer.SetActive(false);
		base.StartCoroutine(this.ProcessAssignmentInputAtEndOfFrame(false));
	}

	private IEnumerator ProcessAssignmentInputAtEndOfFrame(bool editing)
	{
		yield return new WaitForEndOfFrame();
		this.editing = editing;
		yield break;
	}

	private void MarkCurrentConflictingActions(Dictionary<int, string> currentConflictingActionsAndKeys)
	{
		foreach (int num in currentConflictingActionsAndKeys.Keys)
		{
			foreach (ControlsConfigurationElement controlsConfigurationElement in this.elements)
			{
				if (controlsConfigurationElement.GetCurrentActionElementMapId().Equals(num) && controlsConfigurationElement.GetCurrentButtonKey().Equals(currentConflictingActionsAndKeys[num]))
				{
					controlsConfigurationElement.OnElementMarkedAsConflicting();
				}
			}
		}
	}

	private void UnmarkPreviousConflictingActions()
	{
		foreach (ControlsConfigurationElement controlsConfigurationElement in this.elements)
		{
			controlsConfigurationElement.OnElementUnmarkedAsConflicting();
		}
	}

	private void UpdateIndexAndElements(int i)
	{
		this.elements[this.index].OnElementUnselected();
		this.index = i;
		this.elements[this.index].OnElementSelected();
		Vector2 anchoredPosition = this.elements[this.index].GetComponent<RectTransform>().anchoredPosition;
		float offsetByIndex = this.GetOffsetByIndex(i);
		this.contentTransform.anchoredPosition = new Vector2(0f, offsetByIndex);
	}

	private float GetOffsetByIndex(int index)
	{
		if (index > this.indexOfLastElementShown)
		{
			this.indexOfFirstElementShown++;
			this.indexOfLastElementShown++;
			this.currentOffset = (this.maxNumberOfRowsShown - index - 1) * -30;
		}
		else if (index < this.indexOfFirstElementShown)
		{
			this.indexOfFirstElementShown--;
			this.indexOfLastElementShown--;
			this.currentOffset = index * 30;
		}
		return (float)this.currentOffset;
	}

	protected override void OnClose()
	{
		Core.Input.JoystickPressed -= this.UpdateAllElements;
		Core.Input.KeyboardPressed -= this.UpdateAllElements;
		this.navigationButtonsRoot.SetActive(true);
		this.remappingLeyendButtonsRoot.SetActive(false);
		base.gameObject.SetActive(false);
		base.OnClose();
	}

	public RectTransform contentTransform;

	public GameObject navigationButtonsRoot;

	public GameObject remappingLeyendButtonsRoot;

	public GameObject editLeyendButton;

	public GameObject acceptLeyendButton;

	public GameObject restoreDefaultsLeyendButton;

	public GameObject saveAndExitLeyendButton;

	public GameObject cancelLeyendButton;

	public GameObject leftStickDisclaimer;

	private Player rewiredPlayer;

	private CanvasGroup canvasGroup;

	private List<ControlsConfigurationElement> elements;

	private int index;

	private bool editing;

	private int framesSkipped;

	private bool initialized;

	private float delaySecondsForFastScroll = 0.5f;

	private int skippedFramesForFastScroll = 5;

	private int skippedFramesForSlowScroll = 10;

	private float axisThreshold = 0.3f;

	private int maxNumberOfRowsShown = 6;

	private int indexOfFirstElementShown = -1;

	private int indexOfLastElementShown = -1;

	private int currentOffset;

	private List<string> actionNames = new List<string>();
}
