using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using FullSerializer;
using Gameplay.UI.Others.Buttons;
using Gameplay.UI.Widgets;
using I2.Loc;
using Rewired;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class OptionsWidget : SerializedMonoBehaviour
	{
		public void Initialize()
		{
			this.currentMenu = OptionsWidget.MENU.OPTIONS;
			this.nativeHeightRes = Display.main.systemHeight;
			this.nativeWidthRes = Display.main.systemWidth;
			this.ResetMenus();
			this.ReadRenderModeSettings();
		}

		private void ReadRenderModeSettings()
		{
			this.GetCurrentRenderModeSettings();
			this.ShowRenderModesValues();
		}

		public void OnShow(bool optionsIsInitial)
		{
			this.initialOptions = optionsIsInitial;
			this.UpdateMenuButtons();
			this.UpdateTutorials();
			this.ShowMenu(OptionsWidget.MENU.OPTIONS);
		}

		private void Update()
		{
			if (this.rewired == null)
			{
				Player player = ReInput.players.GetPlayer(0);
				if (player == null)
				{
					return;
				}
				this.rewired = player;
			}
			float axisRaw = this.rewired.GetAxisRaw(48);
			bool flag = axisRaw < 0.3f && (double)axisRaw >= -0.3;
			switch (this.currentMenu)
			{
			case OptionsWidget.MENU.GAME:
				if (flag && this.lastHorizontalInOptions != 0f)
				{
					this.UpdateInputGameOptions(this.lastHorizontalInOptions < 0f);
					this.lastHorizontalInOptions = 0f;
				}
				else if (!flag && axisRaw < 0f)
				{
					this.lastHorizontalInOptions = -1f;
				}
				else if (!flag && axisRaw > 0f)
				{
					this.lastHorizontalInOptions = 1f;
				}
				break;
			case OptionsWidget.MENU.ACCESSIBILITY:
				if (flag && this.lastHorizontalInOptions != 0f)
				{
					this.UpdateInputAccessibilityOptions();
					this.lastHorizontalInOptions = 0f;
				}
				else if (!flag && axisRaw < 0f)
				{
					this.lastHorizontalInOptions = -1f;
				}
				else if (!flag && axisRaw > 0f)
				{
					this.lastHorizontalInOptions = 1f;
				}
				break;
			case OptionsWidget.MENU.VIDEO:
				if (flag && this.lastHorizontalInOptions != 0f)
				{
					this.UpdateInputVideoOptions(this.lastHorizontalInOptions < 0f);
					this.lastHorizontalInOptions = 0f;
				}
				else if (!flag && axisRaw < 0f)
				{
					this.lastHorizontalInOptions = -1f;
				}
				else if (!flag && axisRaw > 0f)
				{
					this.lastHorizontalInOptions = 1f;
				}
				break;
			case OptionsWidget.MENU.AUDIO:
				if (flag && this.lastHorizontalInOptions != 0f)
				{
					this.UpdateInputAudioOptions(this.lastHorizontalInOptions < 0f);
					this.lastHorizontalInOptions = 0f;
				}
				else if (!flag && axisRaw < 0f)
				{
					this.lastHorizontalInOptions = -1f;
				}
				else if (!flag && axisRaw > 0f)
				{
					this.lastHorizontalInOptions = 1f;
				}
				break;
			case OptionsWidget.MENU.RENDER_MODES:
				if (flag && this.lastHorizontalInOptions != 0f)
				{
					this.UpdateInputRenderModeOptions(this.lastHorizontalInOptions < 0f);
					this.lastHorizontalInOptions = 0f;
				}
				else if (!flag && axisRaw < 0f)
				{
					this.lastHorizontalInOptions = -1f;
				}
				else if (!flag && axisRaw > 0f)
				{
					this.lastHorizontalInOptions = 1f;
				}
				break;
			}
		}

		public int appliedResolutionIndex { get; private set; }

		public void ShowControlsRemap()
		{
			this.ShowMenu(OptionsWidget.MENU.CONTROLS);
		}

		public bool GoBack()
		{
			bool result = true;
			if (this.currentMenu != OptionsWidget.MENU.OPTIONS)
			{
				EventSystem.current.SetSelectedGameObject(null);
				if (this.currentMenu == OptionsWidget.MENU.CONTROLS)
				{
					if (this.controlsMenuScreen.currentlyActive)
					{
						if (this.controlsMenuScreen.TryClose())
						{
							this.ShowMenu(OptionsWidget.MENU.GAME);
						}
					}
					else
					{
						this.ShowMenu(OptionsWidget.MENU.GAME);
					}
				}
				else if (this.currentMenu == OptionsWidget.MENU.RENDER_MODES)
				{
					this.acceptButton.SetActive(true);
					this.Option_SaveRenderModesOptions();
					this.ShowMenu(OptionsWidget.MENU.VIDEO);
				}
				else
				{
					this.ShowMenu(OptionsWidget.MENU.OPTIONS);
				}
			}
			else
			{
				result = false;
				this.acceptOrApplyButton.text = ScriptLocalization.UI_Map.LABEL_BUTTON_ACCEPT;
			}
			if (this.soundBack != string.Empty)
			{
				Core.Audio.PlayOneShot(this.soundBack, default(Vector3));
			}
			return result;
		}

		public void SelectPreviousTutorial()
		{
			this.currentTutorial--;
			if (this.currentTutorial < 0)
			{
				this.currentTutorial = this.tutorialOrder.Count - 1;
			}
			this.ShowCurrentTutorial();
		}

		public void SelectNextTutorial()
		{
			this.currentTutorial++;
			if (this.currentTutorial >= this.tutorialOrder.Count)
			{
				this.currentTutorial = 0;
			}
			this.ShowCurrentTutorial();
		}

		public void Option_OnSelect(GameObject item)
		{
			if (this.optionLastSelected)
			{
				this.SetOptionSelected(this.optionLastSelected, false);
			}
			this.optionLastSelected = item;
			this.SetOptionSelected(item, true);
		}

		public void Option_SelectGame(int idx)
		{
			this.SetOptionGameSelected(this.optionLastGameSelected, false);
			this.optionLastGameSelected = (OptionsWidget.GAME_OPTIONS)idx;
			this.SetOptionGameSelected((OptionsWidget.GAME_OPTIONS)idx, true);
		}

		public void Option_SelectAccessibility(int idx)
		{
			this.SetOptionAccessibilitySelected(this.optionLastAccessibilitySelected, false);
			this.optionLastAccessibilitySelected = (OptionsWidget.ACCESSIBILITY_OPTIONS)idx;
			this.SetOptionAccessibilitySelected((OptionsWidget.ACCESSIBILITY_OPTIONS)idx, true);
		}

		public void Option_SelectVideo(int idx)
		{
			this.SetOptionVideoSelected(this.optionLastVideoSelected, false);
			this.optionLastVideoSelected = (OptionsWidget.VIDEO_OPTIONS)idx;
			this.SetOptionVideoSelected((OptionsWidget.VIDEO_OPTIONS)idx, true);
		}

		public void Option_SelectRenderModePreset(int idx)
		{
			this.SetOptionRenderModeSelected(this.optionLastRenderModeSelected, false);
			this.optionLastRenderModeSelected = (OptionsWidget.RENDER_MODE_OPTIONS)idx;
			this.SetOptionRenderModeSelected(this.optionLastRenderModeSelected, true);
		}

		public void Option_SelectAudio(int idx)
		{
			this.SetOptionAudioSelected(this.optionLastAudioSelected, false);
			this.optionLastAudioSelected = (OptionsWidget.AUDIO_OPTIONS)idx;
			this.SetOptionAudioSelected((OptionsWidget.AUDIO_OPTIONS)idx, true);
		}

		private void UpdateInputGameOptions(bool left)
		{
			int num = 1;
			if (left)
			{
				num = -1;
			}
			OptionsWidget.GAME_OPTIONS game_OPTIONS = this.optionLastGameSelected;
			if (game_OPTIONS != OptionsWidget.GAME_OPTIONS.AUDIOLANGUAGE)
			{
				if (game_OPTIONS != OptionsWidget.GAME_OPTIONS.TEXTLANGUAGE)
				{
					if (game_OPTIONS == OptionsWidget.GAME_OPTIONS.ENABLEHOWTOPLAY)
					{
						this.currentEnableHowToPlay = !this.currentEnableHowToPlay;
					}
				}
				else
				{
					this.currentTextLanguageIndex += num;
					if (this.currentTextLanguageIndex >= Core.Localization.GetAllEnabledLanguagesNames().Count)
					{
						this.currentTextLanguageIndex = 0;
					}
					else if (this.currentTextLanguageIndex < 0)
					{
						this.currentTextLanguageIndex = Core.Localization.GetAllEnabledLanguagesNames().Count - 1;
					}
				}
			}
			else
			{
				this.currentAudioLanguageIndex += num;
				if (this.currentAudioLanguageIndex >= Core.Localization.GetAllAudioLanguagesNames().Count)
				{
					this.currentAudioLanguageIndex = 0;
				}
				else if (this.currentAudioLanguageIndex < 0)
				{
					this.currentAudioLanguageIndex = Core.Localization.GetAllAudioLanguagesNames().Count - 1;
				}
			}
			this.ShowGameValues();
		}

		private void UpdateInputAccessibilityOptions()
		{
			OptionsWidget.ACCESSIBILITY_OPTIONS accessibility_OPTIONS = this.optionLastAccessibilitySelected;
			if (accessibility_OPTIONS != OptionsWidget.ACCESSIBILITY_OPTIONS.RUMBLEENABLED)
			{
				if (accessibility_OPTIONS != OptionsWidget.ACCESSIBILITY_OPTIONS.SHAKEENABLED)
				{
					if (accessibility_OPTIONS == OptionsWidget.ACCESSIBILITY_OPTIONS.ACHIEVEMENTSPOPUPNABLED)
					{
						this.currentEnableAchievementsPopup = !this.currentEnableAchievementsPopup;
					}
				}
				else
				{
					this.currentEnableShake = !this.currentEnableShake;
				}
			}
			else
			{
				this.currentEnableRumble = !this.currentEnableRumble;
			}
			this.ShowAccessibilityValues();
		}

		private void UpdateInputVideoOptions(bool left)
		{
			int num = 1;
			if (left)
			{
				num = -1;
			}
			switch (this.optionLastVideoSelected)
			{
			case OptionsWidget.VIDEO_OPTIONS.VSYNC:
				this.currentVsync = !this.currentVsync;
				break;
			case OptionsWidget.VIDEO_OPTIONS.WINDOWMODE:
				this.currentFullScreen = !this.currentFullScreen;
				break;
			case OptionsWidget.VIDEO_OPTIONS.BRIGHTNES:
				num *= this.stepBrightness;
				this.currentBrightness = Mathf.Clamp(this.currentBrightness + num, this.minBrightness, this.maxBrightness);
				break;
			case OptionsWidget.VIDEO_OPTIONS.RESOLUTION:
			{
				this.currentResolution += num;
				if (this.currentResolution < 0)
				{
					this.currentResolution = this.resolutions.Count - 1;
				}
				else if (this.currentResolution >= this.resolutions.Count)
				{
					this.currentResolution = 0;
				}
				Resolution resolution = this.resolutions[this.currentResolution];
				if (Core.Screen.ResolutionRequireStrategyScale(resolution.width, resolution.height))
				{
					this.videoElements[OptionsWidget.VIDEO_OPTIONS.RESOLUTIONMODE].SetActive(true);
					this.resolutionModeSelectionElement.SetActive(true);
					this.videoElements[OptionsWidget.VIDEO_OPTIONS.RESOLUTIONMODE].GetComponentInChildren<Text>(true).text = ((this.currentScalingStrategy != OptionsWidget.SCALING_STRATEGY.PIXEL_PERFECT) ? ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_SCALE : ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_PIXELPERFECT);
					this.LinkButtonsVertical(this.resolutionButton, this.resolutionModeButton, false);
					this.LinkButtonsVertical(this.resolutionModeButton, this.vsyncButton, false);
				}
				else
				{
					this.videoElements[OptionsWidget.VIDEO_OPTIONS.RESOLUTIONMODE].SetActive(false);
					this.resolutionModeSelectionElement.SetActive(false);
					this.LinkButtonsVertical(this.resolutionButton, this.vsyncButton, false);
				}
				break;
			}
			case OptionsWidget.VIDEO_OPTIONS.FILTERING:
			{
				int length = Enum.GetValues(typeof(AnisotropicFiltering)).Length;
				int num2 = this.currentFilter + num;
				if (num2 < 0)
				{
					num2 = length - 1;
				}
				else if (num2 >= length)
				{
					num2 = 0;
				}
				this.currentFilter = num2;
				break;
			}
			case OptionsWidget.VIDEO_OPTIONS.RESOLUTIONMODE:
				if (this.currentScalingStrategy == OptionsWidget.SCALING_STRATEGY.PIXEL_PERFECT)
				{
					this.currentScalingStrategy = OptionsWidget.SCALING_STRATEGY.SCALE;
				}
				else
				{
					this.currentScalingStrategy = OptionsWidget.SCALING_STRATEGY.PIXEL_PERFECT;
				}
				break;
			case OptionsWidget.VIDEO_OPTIONS.RENDERMODES:
				Debug.Log("Esto no hace nada :D");
				break;
			}
			this.ShowVideoValues();
		}

		private void UpdateInputRenderModeOptions(bool left)
		{
			int num = (!left) ? 1 : -1;
			OptionsWidget.RENDER_MODE_OPTIONS render_MODE_OPTIONS = this.optionLastRenderModeSelected;
			if (render_MODE_OPTIONS == OptionsWidget.RENDER_MODE_OPTIONS.PRESET)
			{
				this.currentRenderMode = Mathf.Clamp(this.currentRenderMode + num, -1, this.crtEffect.CRTEffectPresets.Count - 1);
			}
			this.renderModesLeftArrow.SetActive(this.currentRenderMode != -1);
			this.renderModesRightArrow.SetActive(this.currentRenderMode != this.crtEffect.CRTEffectPresets.Count - 1);
			this.ShowRenderModesValues();
		}

		private void UpdateInputAudioOptions(bool left)
		{
			int num = 1;
			if (left)
			{
				num = -1;
			}
			switch (this.optionLastAudioSelected)
			{
			case OptionsWidget.AUDIO_OPTIONS.MASTERVOLUME:
				num *= this.stepVolume;
				this.currentMasterVolume = Mathf.Clamp(this.currentMasterVolume + num, this.minVolume, this.maxVolume);
				break;
			case OptionsWidget.AUDIO_OPTIONS.EFFECTSVOLUME:
				num *= this.stepVolume;
				this.currentEffectsVolume = Mathf.Clamp(this.currentEffectsVolume + num, this.minVolume, this.maxVolume);
				break;
			case OptionsWidget.AUDIO_OPTIONS.MUSICVOLUME:
				num *= this.stepVolume;
				this.currentMusicVolume = Mathf.Clamp(this.currentMusicVolume + num, this.minVolume, this.maxVolume);
				break;
			case OptionsWidget.AUDIO_OPTIONS.VOICEOVERVOLUME:
				num *= this.stepVolume;
				this.currentVoiceoverVolume = Mathf.Clamp(this.currentVoiceoverVolume + num, this.minVolume, this.maxVolume);
				break;
			}
			this.ShowAudioValues();
		}

		public void Option_Resume()
		{
			base.StartCoroutine(this.Option_ResumeSecure());
		}

		private IEnumerator Option_ResumeSecure()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			UIController.instance.HidePauseMenu();
			yield break;
		}

		public void Option_MenuGame()
		{
			this.ShowMenu(OptionsWidget.MENU.GAME);
		}

		public void Option_MenuAccessibility()
		{
			this.ShowMenu(OptionsWidget.MENU.ACCESSIBILITY);
		}

		public void Option_MenuVideo()
		{
			this.ShowMenu(OptionsWidget.MENU.VIDEO);
		}

		public void Option_RenderModes()
		{
			this.ShowMenu(OptionsWidget.MENU.RENDER_MODES);
		}

		public void Option_MenuAudio()
		{
			this.ShowMenu(OptionsWidget.MENU.AUDIO);
		}

		public void Option_Tutorial()
		{
			if (this.tutorialOrder.Count > 0)
			{
				this.ShowMenu(OptionsWidget.MENU.TUTORIAL);
			}
		}

		public void Option_MainMenu()
		{
			Core.Logic.ResumeGame();
			this.ResetMenus();
			UIController.instance.HidePauseMenu();
			UIController.instance.HideMiriamTimer();
			Analytics.CustomEvent("QUIT_GAME", new Dictionary<string, object>
			{
				{
					"Scene",
					SceneManager.GetActiveScene().name
				}
			});
			Core.Persistence.SaveGame(true);
			Core.Logic.LoadMenuScene(true);
		}

		public void Option_Reset()
		{
			Core.Logic.ResumeGame();
			this.ResetMenus();
			UIController.instance.HidePauseMenu();
			Core.Logic.Penitent.KillInstanteneously();
		}

		public void Option_Exit()
		{
			this.Option_MainMenu();
		}

		public void Option_AcceptGameOptions()
		{
			int currentLanguageIndex = Core.Localization.GetCurrentLanguageIndex();
			if (currentLanguageIndex != this.currentTextLanguageIndex)
			{
				base.StartCoroutine(this.ChangeTextLanguageAndGoBack());
			}
			else
			{
				Core.TutorialManager.TutorialsEnabled = this.currentEnableHowToPlay;
				Core.Localization.CurrentAudioLanguageIndex = this.currentAudioLanguageIndex;
				this.GetCurrentAccessibilitySettings();
				this.GetCurrentVideoSettings();
				this.GetCurrentAudioSettings();
				this.WriteOptionsToFile();
			}
		}

		private IEnumerator ChangeTextLanguageAndGoBack()
		{
			Core.Localization.CurrentAudioLanguageIndex = this.currentAudioLanguageIndex;
			Core.Localization.SetLanguageByIdx(this.currentTextLanguageIndex);
			Core.TutorialManager.TutorialsEnabled = this.currentEnableHowToPlay;
			yield return new WaitForEndOfFrame();
			this.GetCurrentAccessibilitySettings();
			this.GetCurrentVideoSettings();
			this.GetCurrentAudioSettings();
			this.WriteOptionsToFile();
			this.ShowGameValues();
			yield break;
		}

		public void Option_AcceptAccesibilityOptions()
		{
			SingletonSerialized<RumbleSystem>.Instance.RumblesEnabled = this.currentEnableRumble;
			Core.Logic.CameraManager.ProCamera2DShake.enabled = this.currentEnableShake;
			Core.AchievementsManager.ShowPopUp = this.currentEnableAchievementsPopup;
			this.GetCurrentGameSettings();
			this.GetCurrentVideoSettings();
			this.GetCurrentAudioSettings();
			this.WriteOptionsToFile();
		}

		public void ApplyVideoOptionsFromFile()
		{
			if (this.appliedResolutionIndex < 0 || this.appliedResolutionIndex >= this.resolutions.Count)
			{
				return;
			}
			Resolution resolution = this.resolutions[this.appliedResolutionIndex];
			Screen.SetResolution(resolution.width, resolution.height, this.currentFullScreen);
			Core.Screen.FitScreenCamera();
			Screen.fullScreen = this.currentFullScreen;
			QualitySettings.vSyncCount = ((!this.currentVsync) ? 0 : 1);
			float num = Mathf.Clamp01((float)this.currentBrightness / 255f);
			RenderSettings.ambientLight = new Color(num, num, num);
			QualitySettings.anisotropicFiltering = this.currentFilter;
		}

		public void Option_AcceptVideoOptions()
		{
			Resolution resolution = this.resolutions[this.currentResolution];
			Screen.SetResolution(resolution.width, resolution.height, this.currentFullScreen);
			Core.Screen.FitScreenCamera();
			this.appliedResolutionIndex = this.currentResolution;
			QualitySettings.vSyncCount = ((!this.currentVsync) ? 0 : 1);
			float num = Mathf.Clamp01((float)this.currentBrightness / 255f);
			RenderSettings.ambientLight = new Color(num, num, num);
			QualitySettings.anisotropicFiltering = this.currentFilter;
			this.GetCurrentGameSettings();
			this.GetCurrentAccessibilitySettings();
			this.GetCurrentAudioSettings();
			this.WriteOptionsToFile();
		}

		private void Option_SaveRenderModesOptions()
		{
			Debug.Log("Save Render Mode Options");
			try
			{
				string pathOptionsSettings = OptionsWidget.GetPathOptionsSettings("rendermodes.conf");
				if (!File.Exists(pathOptionsSettings))
				{
					File.CreateText(pathOptionsSettings).Dispose();
				}
				fsData fsData = fsData.CreateDictionary();
				fsData.AsDictionary["rendermode"] = new fsData((long)this.currentRenderMode);
				string encryptedData = fsJsonPrinter.CompressedJson(fsData);
				FileTools.SaveSecure(pathOptionsSettings, encryptedData);
			}
			catch (IOException ex)
			{
				Debug.LogError(ex.Message + ex.StackTrace);
			}
		}

		public void Option_AcceptAudioOptions()
		{
			Core.Audio.MasterVolume = (float)this.currentMasterVolume / 100f;
			Core.Audio.SetSfxVolume((float)this.currentEffectsVolume / 100f);
			Core.Audio.SetMusicVolume((float)this.currentMusicVolume / 100f);
			Core.Audio.SetVoiceoverVolume((float)this.currentVoiceoverVolume / 100f);
			this.GetCurrentGameSettings();
			this.GetCurrentAccessibilitySettings();
			this.GetCurrentVideoSettings();
			this.WriteOptionsToFile();
		}

		public void Option_AcceptControlsOptions()
		{
			Core.ControlRemapManager.WriteControlsSettingsToFile();
			this.GoBack();
		}

		private void UpdateMenuButtons()
		{
			bool flag = !this.initialOptions && Core.TutorialManager.AnyTutorialIsUnlocked();
			this.TutorialMenuOption.transform.parent.gameObject.SetActive(flag);
			this.ExitToMainMenuOption.transform.parent.gameObject.SetActive(!this.initialOptions);
			this.ResumeGameOption.transform.parent.gameObject.SetActive(!this.initialOptions);
			if (this.initialOptions)
			{
				if (flag)
				{
					this.LinkButtonsVertical(this.beforeToTutorialMenuOption, this.TutorialMenuOption, false);
					this.LinkButtonsVertical(this.TutorialMenuOption, this.afterToResumeGameOption, false);
				}
				else
				{
					this.LinkButtonsVertical(this.beforeToTutorialMenuOption, this.afterToResumeGameOption, false);
				}
			}
			else
			{
				if (flag)
				{
					this.LinkButtonsVertical(this.beforeToTutorialMenuOption, this.TutorialMenuOption, false);
					this.LinkButtonsVertical(this.TutorialMenuOption, this.ExitToMainMenuOption, false);
				}
				else
				{
					this.LinkButtonsVertical(this.beforeToTutorialMenuOption, this.ExitToMainMenuOption, false);
				}
				this.LinkButtonsVertical(this.ExitToMainMenuOption, this.ResumeGameOption, false);
				this.LinkButtonsVertical(this.ResumeGameOption, this.afterToResumeGameOption, false);
			}
		}

		private void ShowMenu(OptionsWidget.MENU menu)
		{
			this.currentMenu = menu;
			foreach (KeyValuePair<OptionsWidget.MENU, Transform> keyValuePair in this.optionsRoot)
			{
				if (keyValuePair.Value != null)
				{
					CanvasGroup component = keyValuePair.Value.gameObject.GetComponent<CanvasGroup>();
					component.alpha = ((keyValuePair.Key != this.currentMenu) ? 0f : 1f);
					component.interactable = (keyValuePair.Key == this.currentMenu);
				}
			}
			this.navigationButtonsRoot.SetActive(true);
			this.acceptOrApplyButton.text = ScriptLocalization.UI_Map.LABEL_BUTTON_APPLY;
			switch (this.currentMenu)
			{
			case OptionsWidget.MENU.OPTIONS:
				this.acceptOrApplyButton.text = ScriptLocalization.UI_Map.LABEL_BUTTON_ACCEPT;
				break;
			case OptionsWidget.MENU.GAME:
				foreach (KeyValuePair<OptionsWidget.GAME_OPTIONS, SelectableOption> keyValuePair2 in this.gameElements)
				{
					this.SetOptionGameSelected(keyValuePair2.Key, false);
				}
				this.GetCurrentGameSettings();
				this.ShowGameValues();
				break;
			case OptionsWidget.MENU.ACCESSIBILITY:
				foreach (KeyValuePair<OptionsWidget.GAME_OPTIONS, SelectableOption> keyValuePair3 in this.gameElements)
				{
					this.SetOptionGameSelected(keyValuePair3.Key, false);
				}
				this.GetCurrentAccessibilitySettings();
				this.ShowAccessibilityValues();
				break;
			case OptionsWidget.MENU.VIDEO:
				foreach (KeyValuePair<OptionsWidget.VIDEO_OPTIONS, GameObject> keyValuePair4 in this.videoElements)
				{
					this.SetOptionVideoSelected(keyValuePair4.Key, false);
				}
				this.GetCurrentVideoSettings();
				this.currentResolution = this.appliedResolutionIndex;
				this.ShowVideoValues();
				break;
			case OptionsWidget.MENU.AUDIO:
				foreach (KeyValuePair<OptionsWidget.AUDIO_OPTIONS, GameObject> keyValuePair5 in this.audioElements)
				{
					this.SetOptionAudioSelected(keyValuePair5.Key, false);
				}
				this.GetCurrentAudioSettings();
				this.ShowAudioValues();
				break;
			case OptionsWidget.MENU.TUTORIAL:
				this.currentTutorial = 0;
				this.ShowCurrentTutorial();
				break;
			case OptionsWidget.MENU.CONTROLS:
				foreach (KeyValuePair<OptionsWidget.MENU, Transform> keyValuePair6 in this.optionsRoot)
				{
					if (keyValuePair6.Value != null)
					{
						CanvasGroup component2 = keyValuePair6.Value.gameObject.GetComponent<CanvasGroup>();
						component2.alpha = 0f;
						component2.interactable = false;
					}
				}
				this.navigationButtonsRoot.SetActive(false);
				this.controlsMenuScreen.Open();
				break;
			case OptionsWidget.MENU.RENDER_MODES:
				foreach (KeyValuePair<OptionsWidget.RENDER_MODE_OPTIONS, GameObject> keyValuePair7 in this.renderModesElements)
				{
					this.SetOptionRenderModeSelected(keyValuePair7.Key, false);
				}
				this.navigationButtonsRoot.SetActive(true);
				this.acceptButton.SetActive(false);
				this.GetCurrentRenderModeSettings();
				this.ShowRenderModesValues();
				break;
			}
			Transform transform;
			if (!this.optionsRoot.TryGetValue(this.currentMenu, out transform))
			{
				return;
			}
			transform = transform.Find("Selection");
			if (transform == null)
			{
				return;
			}
			this.lastHorizontalInOptions = 0f;
			for (int i = 0; i < transform.childCount; i++)
			{
				this.SetOptionSelected(transform.GetChild(i).gameObject, i == 0);
			}
			this.optionLastSelected = transform.GetChild(0).gameObject;
			EventSystem.current.SetSelectedGameObject(this.optionLastSelected.GetComponentInChildren<Text>(true).gameObject);
		}

		public void GoBackFromControlsRemapScreen()
		{
			this.GoBack();
		}

		private void GetCurrentGameSettings()
		{
			this.currentAudioLanguageIndex = Core.Localization.CurrentAudioLanguageIndex;
			this.currentTextLanguageIndex = Core.Localization.GetCurrentLanguageIndex();
			this.currentEnableHowToPlay = Core.TutorialManager.TutorialsEnabled;
		}

		private void GetCurrentAccessibilitySettings()
		{
			this.currentEnableRumble = SingletonSerialized<RumbleSystem>.Instance.RumblesEnabled;
			this.currentEnableShake = Core.Logic.CameraManager.ProCamera2DShake.enabled;
			this.currentEnableAchievementsPopup = Core.AchievementsManager.ShowPopUp;
		}

		private void GetCurrentVideoSettings()
		{
			this.InitializeSupportedResolutions();
			this.currentVsync = (QualitySettings.vSyncCount > 0);
			this.currentFullScreen = Screen.fullScreen;
			this.currentBrightness = (int)(RenderSettings.ambientLight.r * 255f);
			this.currentBrightness = Mathf.Clamp(this.currentBrightness, this.minBrightness, this.maxBrightness);
			this.currentFilter = QualitySettings.anisotropicFiltering;
		}

		private void GetCurrentRenderModeSettings()
		{
			Debug.Log("Loading render mode settings (should be already loaded from the beginning of the game...");
			this.currentRenderMode = -1;
			string pathOptionsSettings = OptionsWidget.GetPathOptionsSettings("rendermodes.conf");
			if (!File.Exists(pathOptionsSettings))
			{
				File.CreateText(pathOptionsSettings).Close();
			}
			else
			{
				string text = File.ReadAllText(pathOptionsSettings);
				fsData fsData;
				fsResult fsResult = fsJsonParser.Parse(text, ref fsData);
				if (fsResult.Failed && !fsResult.FormattedMessages.Equals("No input"))
				{
					Debug.LogError("ReadOptionsFromFile: parsing error: " + fsResult.FormattedMessages);
				}
				else if (fsData != null)
				{
					Dictionary<string, fsData> asDictionary = fsData.AsDictionary;
					if (asDictionary.ContainsKey("rendermode"))
					{
						this.currentRenderMode = (int)asDictionary["rendermode"].AsInt64;
					}
				}
			}
		}

		private void InitializeSupportedResolutions()
		{
			this.resolutions.Clear();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			Resolution[] validResolutions = this.GetValidResolutions(Screen.resolutions);
			for (int i = 0; i < validResolutions.Length; i++)
			{
				Resolution item = validResolutions[i];
				if (i <= 0 || item.width != validResolutions[i - 1].width || item.height != validResolutions[i - 1].height)
				{
					this.resolutions.Add(item);
					if (this.currentResolution == -1 && item.width == Screen.currentResolution.width && item.height == Screen.currentResolution.height)
					{
						this.currentResolution = num;
					}
					if (item.width > num2)
					{
						num2 = item.width;
						num3 = num;
					}
					num++;
				}
			}
			if (this.currentResolution == -1)
			{
				this.currentResolution = num3;
			}
		}

		private Resolution[] GetValidResolutions(Resolution[] res)
		{
			return (from currentRes in res
			where (float)currentRes.height >= 360f && (float)currentRes.width >= 640f
			select currentRes).ToArray<Resolution>();
		}

		private Resolution GetFitResolution(Resolution[] unityAvailableResolutions, int savedScreenResIndex)
		{
			Resolution resolution = this.resolutions[0];
			List<Resolution> list = unityAvailableResolutions.ToList<Resolution>();
			if (savedScreenResIndex < this.resolutions.Count)
			{
				resolution = this.resolutions[savedScreenResIndex];
			}
			if (list.Contains(resolution))
			{
				return resolution;
			}
			Resolution resolution2 = Screen.currentResolution;
			if (list.Contains(resolution2))
			{
				return resolution2;
			}
			List<Resolution> list2 = (from x in this.resolutions.ToList<Resolution>()
			orderby x.height descending
			select x).ToList<Resolution>();
			foreach (Resolution resolution3 in list2)
			{
				if (list.Contains(resolution3))
				{
					resolution = resolution3;
					break;
				}
			}
			return resolution;
		}

		private void GetCurrentAudioSettings()
		{
			this.currentMasterVolume = (int)(Core.Audio.MasterVolume * 100f);
			this.currentEffectsVolume = (int)(Core.Audio.GetSfxVolume() * 100f);
			this.currentMusicVolume = (int)(Core.Audio.GetMusicVolume() * 100f);
			this.currentVoiceoverVolume = (int)(Core.Audio.GetVoiceoverVolume() * 100f);
		}

		private void SetOptionSelected(GameObject option, bool selected)
		{
			option.GetComponentInChildren<Text>(true).color = ((!selected) ? this.optionNormalColor : this.optionHighligterColor);
			option.GetComponentInChildren<Image>(true).gameObject.SetActive(selected);
		}

		private void SetOptionGameSelected(OptionsWidget.GAME_OPTIONS option, bool selected)
		{
			this.gameElements[option].selectionTransform.SetActive(selected);
			this.gameElements[option].highlightableText.color = ((!selected) ? this.optionNormalColor : this.optionHighligterColor);
		}

		private void SetOptionAccessibilitySelected(OptionsWidget.ACCESSIBILITY_OPTIONS option, bool selected)
		{
			this.accessibilityElements[option].transform.Find("Selection").gameObject.SetActive(selected);
			this.accessibilityElements[option].GetComponentInChildren<Text>(true).color = ((!selected) ? this.optionNormalColor : this.optionHighligterColor);
		}

		private void SetOptionVideoSelected(OptionsWidget.VIDEO_OPTIONS option, bool selected)
		{
			if (this.videoElements.ContainsKey(option))
			{
				this.videoElements[option].transform.Find("Selection").gameObject.SetActive(selected);
				if (option != OptionsWidget.VIDEO_OPTIONS.BRIGHTNES)
				{
					this.videoElements[option].GetComponentInChildren<Text>(true).color = ((!selected) ? this.optionNormalColor : this.optionHighligterColor);
				}
			}
		}

		private void SetOptionRenderModeSelected(OptionsWidget.RENDER_MODE_OPTIONS key, bool selected)
		{
			this.renderModesElements[key].transform.Find("Selection").gameObject.SetActive(selected);
		}

		private void SetOptionAudioSelected(OptionsWidget.AUDIO_OPTIONS option, bool selected)
		{
			this.audioElements[option].transform.Find("Selection").gameObject.SetActive(selected);
		}

		private void ShowGameValues()
		{
			string currentAudioLanguageByIndex = Core.Localization.GetCurrentAudioLanguageByIndex(this.currentAudioLanguageIndex);
			string text = "UI_Map/LABEL_MENU_LANGUAGENAME";
			string text2 = currentAudioLanguageByIndex;
			string text3 = ScriptLocalization.Get(text, true, 0, true, false, null, text2);
			this.gameElements[OptionsWidget.GAME_OPTIONS.AUDIOLANGUAGE].highlightableText.text = text3;
			this.gameElements[OptionsWidget.GAME_OPTIONS.TEXTLANGUAGE].highlightableText.font = Core.Localization.GetFontByLanguageName(text3);
			string languageNameByIndex = Core.Localization.GetLanguageNameByIndex(this.currentTextLanguageIndex);
			text2 = "UI_Map/LABEL_MENU_LANGUAGENAME";
			text = languageNameByIndex;
			string text4 = ScriptLocalization.Get(text2, true, 0, true, false, null, text);
			this.gameElements[OptionsWidget.GAME_OPTIONS.TEXTLANGUAGE].highlightableText.text = text4;
			this.gameElements[OptionsWidget.GAME_OPTIONS.TEXTLANGUAGE].highlightableText.font = Core.Localization.GetFontByLanguageName(languageNameByIndex);
			this.gameElements[OptionsWidget.GAME_OPTIONS.ENABLEHOWTOPLAY].highlightableText.text = ((!this.currentEnableHowToPlay) ? ScriptLocalization.UI.DISABLED_TEXT : ScriptLocalization.UI.ENABLED_TEXT);
		}

		private void ShowAccessibilityValues()
		{
			this.accessibilityElements[OptionsWidget.ACCESSIBILITY_OPTIONS.RUMBLEENABLED].GetComponentInChildren<Text>(true).text = ((!this.currentEnableRumble) ? ScriptLocalization.UI.DISABLED_TEXT : ScriptLocalization.UI.ENABLED_TEXT);
			this.accessibilityElements[OptionsWidget.ACCESSIBILITY_OPTIONS.SHAKEENABLED].GetComponentInChildren<Text>(true).text = ((!this.currentEnableShake) ? ScriptLocalization.UI.DISABLED_TEXT : ScriptLocalization.UI.ENABLED_TEXT);
			this.accessibilityElements[OptionsWidget.ACCESSIBILITY_OPTIONS.ACHIEVEMENTSPOPUPNABLED].GetComponentInChildren<Text>(true).text = ((!this.currentEnableAchievementsPopup) ? ScriptLocalization.UI.DISABLED_TEXT : ScriptLocalization.UI.ENABLED_TEXT);
		}

		private void ShowVideoValues()
		{
			if (this.resolutions.Count == 0)
			{
				this.InitializeSupportedResolutions();
			}
			if (this.currentResolution >= this.resolutions.Count || this.currentResolution < 0)
			{
				this.currentResolution = this.resolutions.Count - 1;
				Debug.LogWarning("Current Resolution out of range, correcting to {currentResolution}");
			}
			Resolution resolution = this.resolutions[this.currentResolution];
			Debug.Log(string.Concat(new object[]
			{
				" show videos 2 ",
				resolution.width,
				"x",
				resolution.height
			}));
			this.videoElements[OptionsWidget.VIDEO_OPTIONS.RESOLUTION].GetComponentInChildren<Text>(true).text = resolution.width + "x" + resolution.height;
			this.videoElements[OptionsWidget.VIDEO_OPTIONS.WINDOWMODE].GetComponentInChildren<Text>(true).text = ((!this.currentFullScreen) ? ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_WINDOWED : ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_FULLSCREEN);
			this.videoElements[OptionsWidget.VIDEO_OPTIONS.VSYNC].GetComponentInChildren<Text>(true).text = ((!this.currentVsync) ? ScriptLocalization.UI.DISABLED_TEXT : ScriptLocalization.UI.ENABLED_TEXT);
			this.videoElements[OptionsWidget.VIDEO_OPTIONS.FILTERING].GetComponentInChildren<Text>(true).text = this.currentFilter.ToString();
			float currentValue = ((float)this.currentBrightness - (float)this.minBrightness) / ((float)this.maxBrightness - (float)this.minBrightness);
			this.videoElements[OptionsWidget.VIDEO_OPTIONS.BRIGHTNES].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue;
			if (Core.Screen.ResolutionRequireStrategyScale(resolution.width, resolution.height))
			{
				this.videoElements[OptionsWidget.VIDEO_OPTIONS.RESOLUTIONMODE].SetActive(true);
				this.resolutionModeSelectionElement.SetActive(true);
				this.videoElements[OptionsWidget.VIDEO_OPTIONS.RESOLUTIONMODE].GetComponentInChildren<Text>(true).text = ((this.currentScalingStrategy != OptionsWidget.SCALING_STRATEGY.PIXEL_PERFECT) ? ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_SCALE : ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_PIXELPERFECT);
				this.LinkButtonsVertical(this.resolutionButton, this.resolutionModeButton, false);
				this.LinkButtonsVertical(this.resolutionModeButton, this.vsyncButton, false);
			}
			else
			{
				this.videoElements[OptionsWidget.VIDEO_OPTIONS.RESOLUTIONMODE].SetActive(false);
				this.resolutionModeSelectionElement.SetActive(false);
				this.LinkButtonsVertical(this.resolutionButton, this.vsyncButton, false);
			}
		}

		public OptionsWidget.SCALING_STRATEGY GetScalingStrategy()
		{
			return this.currentScalingStrategy;
		}

		private void ShowRenderModesValues()
		{
			if (!this.crtEffect || this.crtEffect.CRTEffectPresets == null)
			{
				return;
			}
			if (this.currentRenderMode < -1 || this.currentRenderMode >= this.crtEffect.CRTEffectPresets.Count)
			{
				Debug.LogWarning("<color=red>Invalid render mode preset: " + this.currentRenderMode + "</color>");
				this.currentRenderMode = -1;
			}
			string text = (this.currentRenderMode != -1) ? this.crtEffect.CRTEffectPresets[this.currentRenderMode].name : LocalizationManager.GetTranslation("UI/DISABLED_TEXT", true, 0, true, false, null, null);
			this.renderModesElements[OptionsWidget.RENDER_MODE_OPTIONS.PRESET].GetComponentInChildren<Text>().text = text;
			this.crtEffect.enabled = (this.currentRenderMode != -1);
			this.scanlineEffect.enabled = (this.currentRenderMode != -1);
			if (this.currentRenderMode > -1)
			{
				this.crtEffect.CRTEffectPresets[this.currentRenderMode].Load(this.crtEffect, this.scanlineEffect);
			}
			this.renderModesLeftArrow.SetActive(this.currentRenderMode != -1);
			this.renderModesRightArrow.SetActive(this.currentRenderMode != this.crtEffect.CRTEffectPresets.Count - 1);
		}

		private void ShowAudioValues()
		{
			float currentValue = ((float)this.currentMasterVolume - (float)this.minVolume) / ((float)this.maxVolume - (float)this.minVolume);
			this.audioElements[OptionsWidget.AUDIO_OPTIONS.MASTERVOLUME].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue;
			float currentValue2 = ((float)this.currentEffectsVolume - (float)this.minVolume) / ((float)this.maxVolume - (float)this.minVolume);
			this.audioElements[OptionsWidget.AUDIO_OPTIONS.EFFECTSVOLUME].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue2;
			float currentValue3 = ((float)this.currentMusicVolume - (float)this.minVolume) / ((float)this.maxVolume - (float)this.minVolume);
			this.audioElements[OptionsWidget.AUDIO_OPTIONS.MUSICVOLUME].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue3;
			float currentValue4 = ((float)this.currentVoiceoverVolume - (float)this.minVolume) / ((float)this.maxVolume - (float)this.minVolume);
			this.audioElements[OptionsWidget.AUDIO_OPTIONS.VOICEOVERVOLUME].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue4;
		}

		private void DeleteAllChildren(Transform parent)
		{
			List<GameObject> list = new List<GameObject>();
			IEnumerator enumerator = parent.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					list.Add(transform.gameObject);
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
			foreach (GameObject gameObject in list)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(gameObject);
				}
				else
				{
					Object.DestroyImmediate(gameObject);
				}
			}
		}

		private void ResetMenus()
		{
			foreach (KeyValuePair<OptionsWidget.MENU, Transform> keyValuePair in this.optionsRoot)
			{
				if (keyValuePair.Value != null)
				{
					keyValuePair.Value.gameObject.SetActive(true);
					CanvasGroup component = keyValuePair.Value.gameObject.GetComponent<CanvasGroup>();
					component.alpha = 0f;
					component.interactable = false;
				}
			}
			int num = 0;
			List<string> allEnabledLanguagesNames = Core.Localization.GetAllEnabledLanguagesNames();
			EventsButton first = null;
			EventsButton second = null;
			EventsButton eventsButton = null;
			IEnumerator enumerator2 = this.languageRoot.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					object obj = enumerator2.Current;
					Transform transform = (Transform)obj;
					EventsButton component2 = transform.GetComponent<EventsButton>();
					if (num == 0)
					{
						first = component2;
					}
					if (num < allEnabledLanguagesNames.Count)
					{
						transform.gameObject.SetActive(true);
						transform.GetComponentInChildren<Text>().text = allEnabledLanguagesNames[num];
						second = component2;
						if (eventsButton != null)
						{
							this.LinkButtonsVertical(eventsButton, component2, false);
						}
						eventsButton = component2;
					}
					else
					{
						transform.gameObject.SetActive(false);
					}
					num++;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator2 as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.LinkButtonsVertical(first, second, true);
		}

		private void LinkButtonsVertical(EventsButton first, EventsButton second, bool firstAndLast)
		{
			Navigation navigation = first.navigation;
			Navigation navigation2 = second.navigation;
			if (firstAndLast)
			{
				navigation.selectOnUp = second;
				navigation2.selectOnDown = first;
			}
			else
			{
				navigation.selectOnDown = second;
				navigation2.selectOnUp = first;
			}
			first.navigation = navigation;
			second.navigation = navigation2;
		}

		private void LinkButtonsHorizontal(EventsButton first, EventsButton second)
		{
			Navigation navigation = first.navigation;
			Navigation navigation2 = second.navigation;
			navigation.selectOnRight = second;
			navigation2.selectOnLeft = first;
			first.navigation = navigation;
			second.navigation = navigation2;
		}

		private void UpdateTutorials()
		{
			this.tutorialOrder.Clear();
			foreach (Tutorial tutorial in Core.TutorialManager.GetUnlockedTutorials())
			{
				this.tutorialOrder.Add(tutorial.id);
				if (!this.tutorialInstances.ContainsKey(tutorial.id))
				{
					GameObject gameObject = Object.Instantiate<GameObject>(tutorial.prefab, Vector3.zero, Quaternion.identity, this.optionsRoot[OptionsWidget.MENU.TUTORIAL]);
					gameObject.transform.localPosition = Vector3.zero;
					this.tutorialInstances[tutorial.id] = gameObject;
				}
			}
		}

		private void ShowCurrentTutorial()
		{
			foreach (KeyValuePair<string, GameObject> keyValuePair in this.tutorialInstances)
			{
				keyValuePair.Value.SetActive(false);
			}
			GameObject gameObject = this.tutorialInstances[this.tutorialOrder[this.currentTutorial]];
			gameObject.SetActive(true);
			gameObject.GetComponent<TutorialWidget>().ShowInMenu(this.currentTutorial + 1, this.tutorialOrder.Count);
		}

		private void WriteOptionsToFile()
		{
			try
			{
				string pathOptionsSettings = this.GetPathOptionsSettings();
				if (!File.Exists(pathOptionsSettings))
				{
					File.CreateText(pathOptionsSettings).Dispose();
				}
				fsData fsData = fsData.CreateDictionary();
				fsData.AsDictionary["audioLanguageIndex"] = new fsData((long)this.currentAudioLanguageIndex);
				fsData.AsDictionary["textLanguageIndex"] = new fsData((long)this.currentTextLanguageIndex);
				fsData.AsDictionary["enableTips"] = new fsData(this.currentEnableHowToPlay);
				fsData.AsDictionary["enableControllerRumble"] = new fsData(this.currentEnableRumble);
				fsData.AsDictionary["enableCameraRumble"] = new fsData(this.currentEnableShake);
				fsData.AsDictionary["enableAchievementsPopup"] = new fsData(this.currentEnableAchievementsPopup);
				fsData.AsDictionary["enableVsync"] = new fsData(this.currentVsync);
				fsData.AsDictionary["enableFullScreen"] = new fsData(this.currentFullScreen);
				fsData.AsDictionary["screenBrightness"] = new fsData((long)this.currentBrightness);
				fsData.AsDictionary["screenResolution"] = new fsData((long)this.currentResolution);
				fsData.AsDictionary["anisotropicFiltering"] = new fsData(Enum.GetName(typeof(AnisotropicFiltering), this.currentFilter));
				fsData.AsDictionary["resolutionMode"] = new fsData(Enum.GetName(typeof(OptionsWidget.SCALING_STRATEGY), this.currentScalingStrategy));
				fsData.AsDictionary["masterVolume"] = new fsData((long)this.currentMasterVolume);
				fsData.AsDictionary["effectsVolume"] = new fsData((long)this.currentEffectsVolume);
				fsData.AsDictionary["musicVolume"] = new fsData((long)this.currentMusicVolume);
				fsData.AsDictionary["voiceVolume"] = new fsData((long)this.currentVoiceoverVolume);
				string encryptedData = fsJsonPrinter.CompressedJson(fsData);
				FileTools.SaveSecure(pathOptionsSettings, encryptedData);
			}
			catch (IOException ex)
			{
				Debug.LogError(ex.Message + ex.StackTrace);
			}
		}

		public bool ReadOptionsFromFile()
		{
			string pathOptionsSettings = this.GetPathOptionsSettings();
			bool flag = File.Exists(pathOptionsSettings);
			if (!flag)
			{
				File.CreateText(pathOptionsSettings).Close();
			}
			else
			{
				string text = File.ReadAllText(pathOptionsSettings);
				fsData fsData;
				fsResult fsResult = fsJsonParser.Parse(text, ref fsData);
				if (fsResult.Failed && !fsResult.FormattedMessages.Equals("No input"))
				{
					Debug.LogError("ReadOptionsFromFile: parsing error: " + fsResult.FormattedMessages);
				}
				else if (fsData != null)
				{
					Dictionary<string, fsData> asDictionary = fsData.AsDictionary;
					if (asDictionary.ContainsKey("audioLanguageIndex"))
					{
						this.currentAudioLanguageIndex = (int)asDictionary["audioLanguageIndex"].AsInt64;
					}
					else
					{
						this.currentAudioLanguageIndex = 0;
					}
					Core.Localization.CurrentAudioLanguageIndex = this.currentAudioLanguageIndex;
					Core.Localization.SetLanguageByIdx((int)asDictionary["textLanguageIndex"].AsInt64);
					Core.TutorialManager.TutorialsEnabled = asDictionary["enableTips"].AsBool;
					SingletonSerialized<RumbleSystem>.Instance.RumblesEnabled = asDictionary["enableControllerRumble"].AsBool;
					Core.Logic.CameraManager.ProCamera2DShake.enabled = asDictionary["enableCameraRumble"].AsBool;
					if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))
					{
						Core.Logic.CameraManager.ProCamera2DShake.enabled = false;
					}
					fsData fsData2;
					if (asDictionary.TryGetValue("enableAchievementsPopup", out fsData2))
					{
						Core.AchievementsManager.ShowPopUp = fsData2.AsBool;
					}
					this.currentVsync = asDictionary["enableVsync"].AsBool;
					QualitySettings.vSyncCount = ((!this.currentVsync) ? 0 : 1);
					float num = Mathf.Clamp01((float)((int)asDictionary["screenBrightness"].AsInt64) / 255f);
					RenderSettings.ambientLight = new Color(num, num, num);
					int num2 = (int)asDictionary["screenResolution"].AsInt64;
					if (this.resolutions.Count == 0)
					{
						this.InitializeSupportedResolutions();
					}
					Resolution fitResolution = this.GetFitResolution(this.GetValidResolutions(Screen.resolutions), num2);
					this.savedResolution = fitResolution;
					this.appliedResolutionIndex = num2;
					this.currentResolution = num2;
					this.currentFullScreen = asDictionary["enableFullScreen"].AsBool;
					Screen.SetResolution(fitResolution.width, fitResolution.height, this.currentFullScreen);
					QualitySettings.anisotropicFiltering = (AnisotropicFiltering)Enum.Parse(typeof(AnisotropicFiltering), asDictionary["anisotropicFiltering"].AsString);
					this.currentScalingStrategy = (OptionsWidget.SCALING_STRATEGY)Enum.Parse(typeof(OptionsWidget.SCALING_STRATEGY), asDictionary["resolutionMode"].AsString);
					this.ApplyVideoOptionsFromFile();
					Core.Audio.MasterVolume = (float)((int)asDictionary["masterVolume"].AsInt64) / 100f;
					Core.Audio.SetSfxVolume((float)((int)asDictionary["effectsVolume"].AsInt64) / 100f);
					Core.Audio.SetMusicVolume((float)((int)asDictionary["musicVolume"].AsInt64) / 100f);
					Core.Audio.SetVoiceoverVolume((float)((int)asDictionary["voiceVolume"].AsInt64) / 100f);
				}
			}
			return flag;
		}

		private string GetPathOptionsSettings()
		{
			return OptionsWidget.GetPathOptionsSettings("/options_settings.json");
		}

		private static string GetPathOptionsSettings(string filename)
		{
			return PersistentManager.GetPathAppSettings(filename);
		}

		[SerializeField]
		[BoxGroup("Options", true, false, 0)]
		private Dictionary<OptionsWidget.MENU, Transform> optionsRoot = new Dictionary<OptionsWidget.MENU, Transform>();

		[SerializeField]
		[BoxGroup("Options", true, false, 0)]
		private Color optionNormalColor = new Color(0.972549f, 0.89411765f, 0.78039217f);

		[SerializeField]
		[BoxGroup("Options", true, false, 0)]
		private Color optionHighligterColor = new Color(0.80784315f, 0.84705883f, 0.49803922f);

		[SerializeField]
		[BoxGroup("Options", true, false, 0)]
		private Transform languageRoot;

		[SerializeField]
		[BoxGroup("Options", true, false, 0)]
		private Text acceptOrApplyButton;

		[SerializeField]
		[BoxGroup("Options Game", true, false, 0)]
		private Dictionary<OptionsWidget.GAME_OPTIONS, SelectableOption> gameElements;

		[SerializeField]
		[BoxGroup("Options Accessibility", true, false, 0)]
		private Dictionary<OptionsWidget.ACCESSIBILITY_OPTIONS, GameObject> accessibilityElements;

		[SerializeField]
		[BoxGroup("Options Video", true, false, 0)]
		private Dictionary<OptionsWidget.VIDEO_OPTIONS, GameObject> videoElements;

		[SerializeField]
		[BoxGroup("Options Video", true, false, 0)]
		private EventsButton vsyncButton;

		[SerializeField]
		[BoxGroup("Options Video", true, false, 0)]
		private EventsButton resolutionButton;

		[SerializeField]
		[BoxGroup("Options Video", true, false, 0)]
		private EventsButton resolutionModeButton;

		[SerializeField]
		[BoxGroup("Options Video", true, false, 0)]
		private GameObject resolutionModeSelectionElement;

		[SerializeField]
		[BoxGroup("Options Video", true, false, 0)]
		private int minBrightness = 20;

		[SerializeField]
		[BoxGroup("Options Video", true, false, 0)]
		private int maxBrightness = 100;

		[SerializeField]
		[BoxGroup("Options Video", true, false, 0)]
		private int stepBrightness = 5;

		[SerializeField]
		[BoxGroup("Options Render Modes", true, false, 0)]
		private Dictionary<OptionsWidget.RENDER_MODE_OPTIONS, GameObject> renderModesElements;

		[BoxGroup("Options Render Modes", true, false, 0)]
		[SerializeField]
		public CRTEffect crtEffect;

		[BoxGroup("Options Render Modes", true, false, 0)]
		[SerializeField]
		public CRTScanlineEffect scanlineEffect;

		[BoxGroup("Options Render Modes", true, false, 0)]
		public GameObject acceptButton;

		[BoxGroup("Options Render Modes", true, false, 0)]
		public GameObject renderModesLeftArrow;

		[BoxGroup("Options Render Modes", true, false, 0)]
		public GameObject renderModesRightArrow;

		[SerializeField]
		[BoxGroup("Options Audio", true, false, 0)]
		private Dictionary<OptionsWidget.AUDIO_OPTIONS, GameObject> audioElements;

		[SerializeField]
		[BoxGroup("Options Audio", true, false, 0)]
		private int minVolume;

		[SerializeField]
		[BoxGroup("Options Audio", true, false, 0)]
		private int maxVolume = 100;

		[SerializeField]
		[BoxGroup("Options Audio", true, false, 0)]
		private int stepVolume = 10;

		[BoxGroup("Navigation Buttons", true, false, 0)]
		[SerializeField]
		private GameObject navigationButtonsRoot;

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundBack = "event:/SFX/UI/ChangeTab";

		[BoxGroup("Options", true, false, 0)]
		[SerializeField]
		private EventsButton beforeToTutorialMenuOption;

		[BoxGroup("Options", true, false, 0)]
		[SerializeField]
		private EventsButton TutorialMenuOption;

		[BoxGroup("Options", true, false, 0)]
		[SerializeField]
		private EventsButton ExitToMainMenuOption;

		[BoxGroup("Options", true, false, 0)]
		[SerializeField]
		private EventsButton ResumeGameOption;

		[BoxGroup("Options", true, false, 0)]
		[SerializeField]
		private EventsButton afterToResumeGameOption;

		public ControlsMenuScreen controlsMenuScreen;

		private OptionsWidget.MENU currentMenu;

		private List<Resolution> resolutions = new List<Resolution>();

		private Resolution savedResolution;

		private Player rewired;

		private int currentTextLanguageIndex;

		private int currentAudioLanguageIndex;

		private bool currentEnableHowToPlay;

		private bool currentEnableRumble;

		private bool currentEnableShake;

		private bool currentEnableAchievementsPopup;

		private bool currentVsync;

		private bool currentFullScreen;

		private int currentBrightness;

		private int currentResolution = -1;

		private AnisotropicFiltering currentFilter = 1;

		private OptionsWidget.SCALING_STRATEGY currentScalingStrategy;

		private int currentMasterVolume;

		private int currentEffectsVolume;

		private int currentMusicVolume;

		private int currentVoiceoverVolume;

		private int nativeHeightRes;

		private int nativeWidthRes;

		private const int RENDERMODE_NONE = -1;

		private int currentRenderMode = -1;

		private const string RENDERMODE_OPTIONS_FILE = "rendermodes.conf";

		private const string RENDERMODE_OPTIONS_KEY = "rendermode";

		private OptionsWidget.GAME_OPTIONS optionLastGameSelected;

		private OptionsWidget.ACCESSIBILITY_OPTIONS optionLastAccessibilitySelected;

		private OptionsWidget.VIDEO_OPTIONS optionLastVideoSelected;

		private OptionsWidget.AUDIO_OPTIONS optionLastAudioSelected;

		private OptionsWidget.RENDER_MODE_OPTIONS optionLastRenderModeSelected;

		private float lastHorizontalInOptions;

		private bool initialOptions;

		private GameObject optionLastSelected;

		private int currentTutorial;

		private Dictionary<string, GameObject> tutorialInstances = new Dictionary<string, GameObject>();

		private List<string> tutorialOrder = new List<string>();

		private const string OPTIONS_SETTINGS_FILE = "/options_settings.json";

		public const float VirtualFrameWidth = 640f;

		public const float VirtualFrameHeight = 360f;

		private enum MENU
		{
			OPTIONS,
			GAME,
			ACCESSIBILITY,
			VIDEO,
			AUDIO,
			TUTORIAL,
			CONTROLS,
			RENDER_MODES
		}

		public enum GAME_OPTIONS
		{
			AUDIOLANGUAGE,
			TEXTLANGUAGE,
			ENABLEHOWTOPLAY,
			CONTROLSREMAP
		}

		public enum ACCESSIBILITY_OPTIONS
		{
			RUMBLEENABLED,
			SHAKEENABLED,
			ACHIEVEMENTSPOPUPNABLED
		}

		public enum VIDEO_OPTIONS
		{
			VSYNC,
			WINDOWMODE,
			BRIGHTNES,
			RESOLUTION,
			FILTERING,
			RESOLUTIONMODE,
			RENDERMODES
		}

		public enum SCALING_STRATEGY
		{
			PIXEL_PERFECT,
			SCALE
		}

		public enum AUDIO_OPTIONS
		{
			MASTERVOLUME,
			EFFECTSVOLUME,
			MUSICVOLUME,
			VOICEOVERVOLUME
		}

		public enum RENDER_MODE_OPTIONS
		{
			PRESET
		}
	}
}
