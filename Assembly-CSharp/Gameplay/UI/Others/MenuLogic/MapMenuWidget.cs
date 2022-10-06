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
	public class MapMenuWidget : SerializedMonoBehaviour
	{
		private MapMenuWidget.STATE currentState
		{
			get
			{
				return this._currentState;
			}
			set
			{
				this._currentState = value;
				if (this.animator)
				{
					this.animator.SetInteger("STATUS", (int)this.currentState);
				}
			}
		}

		private void Awake()
		{
			this.currentState = MapMenuWidget.STATE.STATE_OFF;
			this.animator = base.GetComponent<Animator>();
			this.mapPosX = 0f;
			this.mapPosY = 0f;
			this.currentMenu = MapMenuWidget.MENU.OPTIONS;
			this.nativeHeightRes = Display.main.systemHeight;
			this.nativeWidthRes = Display.main.systemWidth;
			this.CreateMap();
			this.ResetMenus();
		}

		private IEnumerator Start()
		{
			yield return new WaitUntil(() => Core.ready);
			this.ReadOptionsFromFile();
			yield break;
		}

		private void OnDestroy()
		{
			Core.Logic.ResumeGame();
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
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS)
			{
				switch (this.currentMenu)
				{
				case MapMenuWidget.MENU.GAME:
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
				case MapMenuWidget.MENU.ACCESSIBILITY:
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
				case MapMenuWidget.MENU.VIDEO:
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
				case MapMenuWidget.MENU.AUDIO:
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
				}
			}
			if (this.currentState != MapMenuWidget.STATE.STATE_MAP)
			{
				return;
			}
			if (!this.mapEnabled)
			{
				return;
			}
			float num = this.rewired.GetAxisRaw(48);
			float num2 = this.rewired.GetAxisRaw(49);
			bool flag2 = false;
			if (this.rewired.GetButtonDown(52))
			{
				this.automaticScroll = true;
				flag2 = true;
				this.timeInAutomatic = 0f;
			}
			else if (this.automaticScroll)
			{
				this.automaticScroll = (num == 0f && num2 == 0f);
				this.timeInAutomatic += Time.unscaledDeltaTime;
			}
			if (this.automaticScroll)
			{
				Vector2 vector;
				vector..ctor(this.playerSpriteInMap.localPosition.x, this.playerSpriteInMap.localPosition.y);
				Vector2 vector2;
				vector2..ctor(this.mapPosX, this.mapPosY);
				Vector2 vector3 = vector2 - vector;
				if (vector3.sqrMagnitude <= this.mapEpsilonSqrDistance)
				{
					this.automaticScroll = false;
					this.SetMapFocusOnPosition(vector.x, vector.y);
				}
				else
				{
					vector3.Normalize();
					vector3 *= this.automaticFactor.Evaluate(this.timeInAutomatic);
					num = -vector3.x;
					num2 = -vector3.y;
					if (!flag2 && ((this.prevVerticalAxis > 0f && num2 <= 0f) || (this.prevVerticalAxis < 0f && num2 >= 0f)))
					{
						this.SetMapFocusOnPosition(this.mapPosX, vector.y);
					}
					if (!flag2 && ((this.prevHorizontalAxis > 0f && num <= 0f) || (this.prevHorizontalAxis < 0f && num >= 0f)))
					{
						this.SetMapFocusOnPosition(vector.x, this.mapPosY);
					}
					this.prevVerticalAxis = num2;
					this.prevHorizontalAxis = num;
				}
			}
			if (num != 0f || num2 != 0f)
			{
				this.mapPosX += num * this.scrollSpeedX * Time.unscaledDeltaTime;
				this.mapPosY += num2 * this.scrollSpeedY * Time.unscaledDeltaTime;
				this.mapPosX = Mathf.Clamp(this.mapPosX, this.scrollLimitsMin.x, this.scrollLimitsMax.x);
				this.mapPosY = Mathf.Clamp(this.mapPosY, this.scrollLimitsMin.y, this.scrollLimitsMax.y);
				this.SetMapFocusOnPosition(this.mapPosX, this.mapPosY);
			}
			this.UpdateDomainAndZoneName();
		}

		private void UpdateDomainAndZoneName()
		{
			if (this.lastSelectedMapPiece != null)
			{
				this.lastSelectedMapPiece.color = this.mapNotSelected;
				this.lastSelectedMapPiece = null;
			}
			MapManager.DataMapReveal nearestZone = Core.MapManager.GetNearestZone(new Vector2(this.mapPosX, this.mapPosY));
			string text = string.Empty;
			string text2 = string.Empty;
			if (nearestZone != null && (nearestZone.updatedAnyTime || nearestZone.updated))
			{
				text2 = Core.MapManager.GetDomainName(nearestZone.domain);
				text = Core.MapManager.GetZoneName(nearestZone.domain, nearestZone.zone);
				string key = nearestZone.domain + "_" + nearestZone.zone;
				if (this.mapPieces.ContainsKey(key))
				{
					this.lastSelectedMapPiece = this.mapPieces[key];
					this.lastSelectedMapPiece.color = this.mapSelected;
				}
			}
			this.domainCaption.text = text2;
			this.zoneCaption.text = text;
		}

		public bool currentlyActive
		{
			get
			{
				return this.currentState != MapMenuWidget.STATE.STATE_OFF;
			}
		}

		public int appliedResolutionIndex { get; private set; }

		public void ShowOptionsOutsideMap()
		{
			this.initialOptions = true;
			this.UpdateMenuButtons();
			Core.Input.SetBlocker("INGAME_MENU", true);
			Core.Logic.SetState(LogicStates.Unresponsive);
			this.currentState = MapMenuWidget.STATE.STATE_OPTIONS;
			Core.Logic.PauseGame();
			this.ShowMenu(MapMenuWidget.MENU.OPTIONS);
		}

		public void Show(bool b)
		{
			if (SceneManager.GetActiveScene().name == "MainMenu")
			{
				return;
			}
			if (FadeWidget.instance.Fading)
			{
				return;
			}
			if (b && Core.Input.InputBlocked && !Core.Input.HasBlocker("PLAYER_LOGIC"))
			{
				return;
			}
			this.initialOptions = false;
			this.UpdateMenuButtons();
			Core.Input.SetBlocker("INGAME_MENU", b);
			if (b)
			{
				this.UpdateTutorials();
				Core.Logic.SetState(LogicStates.Unresponsive);
				this.currentState = MapMenuWidget.STATE.STATE_MAP;
				Core.Logic.PauseGame();
				this.CreateMap();
				MapManager.DataMapReveal currentZone = Core.MapManager.GetCurrentZone();
				this.mapEnabled = (currentZone != null);
				this.playerSpriteInMap.gameObject.SetActive(this.mapEnabled);
				this.elementsWhenEnabled.ForEach(delegate(GameObject p)
				{
					p.SetActive(this.mapEnabled);
				});
				this.elementsWhenDisabled.ForEach(delegate(GameObject p)
				{
					p.SetActive(!this.mapEnabled);
				});
				if (this.mapEnabled)
				{
					if (currentZone != null)
					{
						Rect rect = this.playerSprite.rect;
						Vector3 pos = currentZone.WorldToTexture(Core.Logic.Penitent.transform.position + Core.MapManager.playerMapOffset);
						this.playerSpriteInMap.localPosition = this.SnapToMapCell(pos, true);
					}
					this.SetMapFocusOnPosition(this.playerSpriteInMap.localPosition.x, this.playerSpriteInMap.localPosition.y);
					this.UpdateZoneInfo();
				}
				else
				{
					this.domainCaption.text = string.Empty;
					this.zoneCaption.text = string.Empty;
				}
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(null);
				this.ResetMenus();
				Core.Logic.ResumeGame();
				Core.Logic.SetState(LogicStates.Playing);
				this.currentState = MapMenuWidget.STATE.STATE_OFF;
			}
		}

		public void GoBack()
		{
			MapMenuWidget.STATE currentState = this.currentState;
			if (currentState != MapMenuWidget.STATE.STATE_MAP)
			{
				if (currentState != MapMenuWidget.STATE.STATE_LEYEND)
				{
					if (currentState == MapMenuWidget.STATE.STATE_OPTIONS)
					{
						if (this.currentMenu != MapMenuWidget.MENU.OPTIONS)
						{
							EventSystem.current.SetSelectedGameObject(null);
							if (this.currentMenu == MapMenuWidget.MENU.CONTROLS)
							{
								this.ShowMenu(MapMenuWidget.MENU.GAME);
							}
							else
							{
								this.ShowMenu(MapMenuWidget.MENU.OPTIONS);
							}
						}
						else
						{
							this.acceptOrApplyButton.text = ScriptLocalization.UI_Map.LABEL_BUTTON_ACCEPT;
							if (this.initialOptions)
							{
								this.Show(false);
							}
							else
							{
								this.currentState = MapMenuWidget.STATE.STATE_MAP;
							}
						}
					}
				}
				else
				{
					this.currentState = MapMenuWidget.STATE.STATE_MAP;
				}
			}
			else
			{
				this.Show(false);
			}
			if (this.soundBack != string.Empty)
			{
				Core.Audio.PlayOneShot(this.soundBack, default(Vector3));
			}
		}

		public bool IsInControlRemapScreen()
		{
			return this.currentState == MapMenuWidget.STATE.STATE_OPTIONS && this.currentMenu == MapMenuWidget.MENU.CONTROLS;
		}

		public void ShowOrHideLeyend()
		{
			if (!this.mapEnabled)
			{
				return;
			}
			if (this.currentState == MapMenuWidget.STATE.STATE_MAP || this.currentState == MapMenuWidget.STATE.STATE_LEYEND)
			{
				this.currentState = ((this.currentState != MapMenuWidget.STATE.STATE_MAP) ? MapMenuWidget.STATE.STATE_MAP : MapMenuWidget.STATE.STATE_LEYEND);
				if (this.soundBack != string.Empty)
				{
					Core.Audio.PlayOneShot(this.soundBack, default(Vector3));
				}
			}
		}

		public void ShowOptions()
		{
			if (this.currentState != MapMenuWidget.STATE.STATE_OPTIONS)
			{
				this.currentState = MapMenuWidget.STATE.STATE_OPTIONS;
				this.ShowMenu(MapMenuWidget.MENU.OPTIONS);
			}
		}

		public void ShowControlsRemap()
		{
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS)
			{
				this.ShowMenu(MapMenuWidget.MENU.CONTROLS);
			}
		}

		public void SelectPreviousTutorial()
		{
			if (this.currentState != MapMenuWidget.STATE.STATE_OPTIONS || this.currentMenu != MapMenuWidget.MENU.TUTORIAL)
			{
				return;
			}
			this.currentTutorial--;
			if (this.currentTutorial < 0)
			{
				this.currentTutorial = this.tutorialOrder.Count - 1;
			}
			this.ShowCurrentTutorial();
		}

		public void SelectNextTutorial()
		{
			if (this.currentState != MapMenuWidget.STATE.STATE_OPTIONS || this.currentMenu != MapMenuWidget.MENU.TUTORIAL)
			{
				return;
			}
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
			this.optionLastGameSelected = (MapMenuWidget.GAME_OPTIONS)idx;
			this.SetOptionGameSelected((MapMenuWidget.GAME_OPTIONS)idx, true);
		}

		public void Option_SelectAccessibility(int idx)
		{
			this.SetOptionAccessibilitySelected(this.optionLastAccessibilitySelected, false);
			this.optionLastAccessibilitySelected = (MapMenuWidget.ACCESSIBILITY_OPTIONS)idx;
			this.SetOptionAccessibilitySelected((MapMenuWidget.ACCESSIBILITY_OPTIONS)idx, true);
		}

		public void Option_SelectVideo(int idx)
		{
			this.SetOptionVideoSelected(this.optionLastVideoSelected, false);
			this.optionLastVideoSelected = (MapMenuWidget.VIDEO_OPTIONS)idx;
			this.SetOptionVideoSelected((MapMenuWidget.VIDEO_OPTIONS)idx, true);
		}

		public void Option_SelectAudio(int idx)
		{
			this.SetOptionAudioSelected(this.optionLastAudioSelected, false);
			this.optionLastAudioSelected = (MapMenuWidget.AUDIO_OPTIONS)idx;
			this.SetOptionAudioSelected((MapMenuWidget.AUDIO_OPTIONS)idx, true);
		}

		private void UpdateInputGameOptions(bool left)
		{
			int num = 1;
			if (left)
			{
				num = -1;
			}
			MapMenuWidget.GAME_OPTIONS game_OPTIONS = this.optionLastGameSelected;
			if (game_OPTIONS != MapMenuWidget.GAME_OPTIONS.AUDIOLANGUAGE)
			{
				if (game_OPTIONS != MapMenuWidget.GAME_OPTIONS.TEXTLANGUAGE)
				{
					if (game_OPTIONS == MapMenuWidget.GAME_OPTIONS.ENABLEHOWTOPLAY)
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
			this.ShowGameValues();
		}

		private void UpdateInputAccessibilityOptions()
		{
			MapMenuWidget.ACCESSIBILITY_OPTIONS accessibility_OPTIONS = this.optionLastAccessibilitySelected;
			if (accessibility_OPTIONS != MapMenuWidget.ACCESSIBILITY_OPTIONS.RUMBLEENABLED)
			{
				if (accessibility_OPTIONS == MapMenuWidget.ACCESSIBILITY_OPTIONS.SHAKEENABLED)
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
			case MapMenuWidget.VIDEO_OPTIONS.VSYNC:
				this.currentVsync = !this.currentVsync;
				break;
			case MapMenuWidget.VIDEO_OPTIONS.WINDOWMODE:
				this.currentFullScreen = !this.currentFullScreen;
				break;
			case MapMenuWidget.VIDEO_OPTIONS.BRIGHTNES:
				num *= this.stepBrightness;
				this.currentBrightness = Mathf.Clamp(this.currentBrightness + num, this.minBrightness, this.maxBrightness);
				break;
			case MapMenuWidget.VIDEO_OPTIONS.RESOLUTION:
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
					this.videoElements[MapMenuWidget.VIDEO_OPTIONS.RESOLUTIONMODE].SetActive(true);
					this.resolutionModeSelectionElement.SetActive(true);
					this.videoElements[MapMenuWidget.VIDEO_OPTIONS.RESOLUTIONMODE].GetComponentInChildren<Text>(true).text = ((this.currentScalingStrategy != MapMenuWidget.SCALING_STRATEGY.PIXEL_PERFECT) ? ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_SCALE : ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_PIXELPERFECT);
					this.LinkButtonsVertical(this.resolutionButton, this.resolutionModeButton, false);
					this.LinkButtonsVertical(this.resolutionModeButton, this.vsyncButton, false);
				}
				else
				{
					this.videoElements[MapMenuWidget.VIDEO_OPTIONS.RESOLUTIONMODE].SetActive(false);
					this.resolutionModeSelectionElement.SetActive(false);
					this.LinkButtonsVertical(this.resolutionButton, this.vsyncButton, false);
				}
				break;
			}
			case MapMenuWidget.VIDEO_OPTIONS.FILTERING:
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
			case MapMenuWidget.VIDEO_OPTIONS.RESOLUTIONMODE:
				if (this.currentScalingStrategy == MapMenuWidget.SCALING_STRATEGY.PIXEL_PERFECT)
				{
					this.currentScalingStrategy = MapMenuWidget.SCALING_STRATEGY.SCALE;
				}
				else
				{
					this.currentScalingStrategy = MapMenuWidget.SCALING_STRATEGY.PIXEL_PERFECT;
				}
				break;
			}
			this.ShowVideoValues();
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
			case MapMenuWidget.AUDIO_OPTIONS.MASTERVOLUME:
				num *= this.stepVolume;
				this.currentMasterVolume = Mathf.Clamp(this.currentMasterVolume + num, this.minVolume, this.maxVolume);
				break;
			case MapMenuWidget.AUDIO_OPTIONS.EFFECTSVOLUME:
				num *= this.stepVolume;
				this.currentEffectsVolume = Mathf.Clamp(this.currentEffectsVolume + num, this.minVolume, this.maxVolume);
				break;
			case MapMenuWidget.AUDIO_OPTIONS.MUSICVOLUME:
				num *= this.stepVolume;
				this.currentMusicVolume = Mathf.Clamp(this.currentMusicVolume + num, this.minVolume, this.maxVolume);
				break;
			case MapMenuWidget.AUDIO_OPTIONS.VOICEOVERVOLUME:
				num *= this.stepVolume;
				this.currentVoiceoverVolume = Mathf.Clamp(this.currentVoiceoverVolume + num, this.minVolume, this.maxVolume);
				break;
			}
			this.ShowAudioValues();
		}

		public void Option_Resume()
		{
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS)
			{
				base.StartCoroutine(this.Option_ResumeSecure());
			}
		}

		private IEnumerator Option_ResumeSecure()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			this.Show(false);
			yield break;
		}

		public void Option_MenuGame()
		{
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS)
			{
				this.ShowMenu(MapMenuWidget.MENU.GAME);
			}
		}

		public void Option_MenuAccessibility()
		{
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS)
			{
				this.ShowMenu(MapMenuWidget.MENU.ACCESSIBILITY);
			}
		}

		public void Option_MenuVideo()
		{
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS)
			{
				this.ShowMenu(MapMenuWidget.MENU.VIDEO);
			}
		}

		public void Option_MenuAudio()
		{
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS)
			{
				this.ShowMenu(MapMenuWidget.MENU.AUDIO);
			}
		}

		public void Option_Tutorial()
		{
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS && this.tutorialOrder.Count > 0)
			{
				this.ShowMenu(MapMenuWidget.MENU.TUTORIAL);
			}
		}

		public void Option_MainMenu()
		{
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS)
			{
				Core.Logic.ResumeGame();
				this.ResetMenus();
				this.Show(false);
				Analytics.CustomEvent("QUIT_GAME", new Dictionary<string, object>
				{
					{
						"Scene",
						SceneManager.GetActiveScene().name
					}
				});
				Core.Logic.LoadMenuScene(true);
			}
		}

		public void Option_Reset()
		{
			if (this.currentState == MapMenuWidget.STATE.STATE_OPTIONS)
			{
				Core.Logic.ResumeGame();
				this.ResetMenus();
				this.Show(false);
				Core.Logic.Penitent.KillInstanteneously();
			}
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
				this.GetCurrentAccessibilitySettings();
				this.GetCurrentVideoSettings();
				this.GetCurrentAudioSettings();
				this.WriteOptionsToFile();
			}
		}

		private IEnumerator ChangeTextLanguageAndGoBack()
		{
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

		private Vector3 SnapToMapCell(Vector3 pos, bool changeX)
		{
			float num = pos.x;
			if (changeX)
			{
				num = Mathf.Round(pos.x * this.FACTOR_X / (float)this.CELL_WIDTH) * (float)this.CELL_WIDTH;
			}
			float num2 = Mathf.Round(pos.y * this.FACTOR_Y / (float)this.CELL_HEIGHT) * (float)this.CELL_HEIGHT;
			foreach (MapMenuWidget.RangePixels rangePixels in this.cellPixelFix)
			{
				if (num2 >= (float)rangePixels.cellMin && num2 <= (float)rangePixels.cellMax)
				{
					num2 += (float)rangePixels.value;
					break;
				}
			}
			return new Vector3(num, num2, pos.z);
		}

		private void UpdateZoneInfo()
		{
			this.domainCaption.text = Core.MapManager.GetCurrentDomainName();
			this.zoneCaption.text = Core.MapManager.GetCurrentZoneName();
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

		private void ShowMenu(MapMenuWidget.MENU menu)
		{
			this.currentMenu = menu;
			foreach (KeyValuePair<MapMenuWidget.MENU, Transform> keyValuePair in this.optionsRoot)
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
			case MapMenuWidget.MENU.OPTIONS:
				this.acceptOrApplyButton.text = ScriptLocalization.UI_Map.LABEL_BUTTON_ACCEPT;
				break;
			case MapMenuWidget.MENU.GAME:
				foreach (KeyValuePair<MapMenuWidget.GAME_OPTIONS, SelectableOption> keyValuePair2 in this.gameElements)
				{
					this.SetOptionGameSelected(keyValuePair2.Key, false);
				}
				this.GetCurrentGameSettings();
				this.ShowGameValues();
				break;
			case MapMenuWidget.MENU.ACCESSIBILITY:
				foreach (KeyValuePair<MapMenuWidget.GAME_OPTIONS, SelectableOption> keyValuePair3 in this.gameElements)
				{
					this.SetOptionGameSelected(keyValuePair3.Key, false);
				}
				this.GetCurrentAccessibilitySettings();
				this.ShowAccessibilityValues();
				break;
			case MapMenuWidget.MENU.VIDEO:
				foreach (KeyValuePair<MapMenuWidget.VIDEO_OPTIONS, GameObject> keyValuePair4 in this.videoElements)
				{
					this.SetOptionVideoSelected(keyValuePair4.Key, false);
				}
				this.GetCurrentVideoSettings();
				this.currentResolution = this.appliedResolutionIndex;
				this.ShowVideoValues();
				break;
			case MapMenuWidget.MENU.AUDIO:
				foreach (KeyValuePair<MapMenuWidget.AUDIO_OPTIONS, GameObject> keyValuePair5 in this.audioElements)
				{
					this.SetOptionAudioSelected(keyValuePair5.Key, false);
				}
				this.GetCurrentAudioSettings();
				this.ShowAudioValues();
				break;
			case MapMenuWidget.MENU.TUTORIAL:
				this.currentTutorial = 0;
				this.ShowCurrentTutorial();
				break;
			case MapMenuWidget.MENU.CONTROLS:
				foreach (KeyValuePair<MapMenuWidget.MENU, Transform> keyValuePair6 in this.optionsRoot)
				{
					if (keyValuePair6.Value != null)
					{
						CanvasGroup component2 = keyValuePair6.Value.gameObject.GetComponent<CanvasGroup>();
						component2.alpha = 0f;
						component2.interactable = false;
					}
				}
				this.controlsMenuScreen.Open();
				break;
			}
			Transform transform = this.optionsRoot[this.currentMenu];
			if (transform == null)
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
			this.currentTextLanguageIndex = Core.Localization.GetCurrentLanguageIndex();
			this.currentEnableHowToPlay = Core.TutorialManager.TutorialsEnabled;
		}

		private void GetCurrentAccessibilitySettings()
		{
			this.currentEnableRumble = SingletonSerialized<RumbleSystem>.Instance.RumblesEnabled;
			this.currentEnableShake = Core.Logic.CameraManager.ProCamera2DShake.enabled;
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

		private void SetOptionGameSelected(MapMenuWidget.GAME_OPTIONS option, bool selected)
		{
			if (option != MapMenuWidget.GAME_OPTIONS.AUDIOLANGUAGE)
			{
				this.gameElements[option].selectionTransform.SetActive(selected);
			}
			this.gameElements[option].highlightableText.color = ((!selected) ? this.optionNormalColor : this.optionHighligterColor);
		}

		private void SetOptionAccessibilitySelected(MapMenuWidget.ACCESSIBILITY_OPTIONS option, bool selected)
		{
			this.accessibilityElements[option].transform.Find("Selection").gameObject.SetActive(selected);
			this.accessibilityElements[option].GetComponentInChildren<Text>(true).color = ((!selected) ? this.optionNormalColor : this.optionHighligterColor);
		}

		private void SetOptionVideoSelected(MapMenuWidget.VIDEO_OPTIONS option, bool selected)
		{
			this.videoElements[option].transform.Find("Selection").gameObject.SetActive(selected);
			if (option != MapMenuWidget.VIDEO_OPTIONS.BRIGHTNES)
			{
				this.videoElements[option].GetComponentInChildren<Text>(true).color = ((!selected) ? this.optionNormalColor : this.optionHighligterColor);
			}
		}

		private void SetOptionAudioSelected(MapMenuWidget.AUDIO_OPTIONS option, bool selected)
		{
			this.audioElements[option].transform.Find("Selection").gameObject.SetActive(selected);
		}

		private void ShowGameValues()
		{
			string key = "UI_Map/LABEL_MENU_" + Core.Localization.GetLanguageCodeByIndex(1).ToUpper() + "_LANGUAGENAME";
			this.gameElements[MapMenuWidget.GAME_OPTIONS.AUDIOLANGUAGE].highlightableText.text = Core.Localization.Get(key);
			string languageNameByIndex = Core.Localization.GetLanguageNameByIndex(this.currentTextLanguageIndex);
			string term = "UI_Map/LABEL_MENU_LANGUAGENAME";
			string overrideLanguage = languageNameByIndex;
			string text = ScriptLocalization.Get(term, true, 0, true, false, null, overrideLanguage);
			this.gameElements[MapMenuWidget.GAME_OPTIONS.TEXTLANGUAGE].highlightableText.text = text;
			this.gameElements[MapMenuWidget.GAME_OPTIONS.TEXTLANGUAGE].highlightableText.font = Core.Localization.GetFontByLanguageName(languageNameByIndex);
			this.gameElements[MapMenuWidget.GAME_OPTIONS.ENABLEHOWTOPLAY].highlightableText.text = ((!this.currentEnableHowToPlay) ? ScriptLocalization.UI.DISABLED_TEXT : ScriptLocalization.UI.ENABLED_TEXT);
		}

		private void ShowAccessibilityValues()
		{
			this.accessibilityElements[MapMenuWidget.ACCESSIBILITY_OPTIONS.RUMBLEENABLED].GetComponentInChildren<Text>(true).text = ((!this.currentEnableRumble) ? ScriptLocalization.UI.DISABLED_TEXT : ScriptLocalization.UI.ENABLED_TEXT);
			this.accessibilityElements[MapMenuWidget.ACCESSIBILITY_OPTIONS.SHAKEENABLED].GetComponentInChildren<Text>(true).text = ((!this.currentEnableShake) ? ScriptLocalization.UI.DISABLED_TEXT : ScriptLocalization.UI.ENABLED_TEXT);
		}

		private void ShowVideoValues()
		{
			if (this.resolutions.Count == 0)
			{
				this.InitializeSupportedResolutions();
			}
			Resolution resolution = this.resolutions[this.currentResolution];
			Debug.Log(string.Concat(new object[]
			{
				" show videos 2 ",
				resolution.width,
				"x",
				resolution.height
			}));
			this.videoElements[MapMenuWidget.VIDEO_OPTIONS.RESOLUTION].GetComponentInChildren<Text>(true).text = resolution.width + "x" + resolution.height;
			this.videoElements[MapMenuWidget.VIDEO_OPTIONS.WINDOWMODE].GetComponentInChildren<Text>(true).text = ((!this.currentFullScreen) ? ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_WINDOWED : ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_FULLSCREEN);
			this.videoElements[MapMenuWidget.VIDEO_OPTIONS.VSYNC].GetComponentInChildren<Text>(true).text = ((!this.currentVsync) ? ScriptLocalization.UI.DISABLED_TEXT : ScriptLocalization.UI.ENABLED_TEXT);
			this.videoElements[MapMenuWidget.VIDEO_OPTIONS.FILTERING].GetComponentInChildren<Text>(true).text = this.currentFilter.ToString();
			float currentValue = ((float)this.currentBrightness - (float)this.minBrightness) / ((float)this.maxBrightness - (float)this.minBrightness);
			this.videoElements[MapMenuWidget.VIDEO_OPTIONS.BRIGHTNES].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue;
			if (Core.Screen.ResolutionRequireStrategyScale(resolution.width, resolution.height))
			{
				this.videoElements[MapMenuWidget.VIDEO_OPTIONS.RESOLUTIONMODE].SetActive(true);
				this.resolutionModeSelectionElement.SetActive(true);
				this.videoElements[MapMenuWidget.VIDEO_OPTIONS.RESOLUTIONMODE].GetComponentInChildren<Text>(true).text = ((this.currentScalingStrategy != MapMenuWidget.SCALING_STRATEGY.PIXEL_PERFECT) ? ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_SCALE : ScriptLocalization.UI_Map.LABEL_MENU_VIDEO_PIXELPERFECT);
				this.LinkButtonsVertical(this.resolutionButton, this.resolutionModeButton, false);
				this.LinkButtonsVertical(this.resolutionModeButton, this.vsyncButton, false);
			}
			else
			{
				this.videoElements[MapMenuWidget.VIDEO_OPTIONS.RESOLUTIONMODE].SetActive(false);
				this.resolutionModeSelectionElement.SetActive(false);
				this.LinkButtonsVertical(this.resolutionButton, this.vsyncButton, false);
			}
		}

		public MapMenuWidget.SCALING_STRATEGY GetScalingStrategy()
		{
			return this.currentScalingStrategy;
		}

		private void ShowAudioValues()
		{
			float currentValue = ((float)this.currentMasterVolume - (float)this.minVolume) / ((float)this.maxVolume - (float)this.minVolume);
			this.audioElements[MapMenuWidget.AUDIO_OPTIONS.MASTERVOLUME].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue;
			float currentValue2 = ((float)this.currentEffectsVolume - (float)this.minVolume) / ((float)this.maxVolume - (float)this.minVolume);
			this.audioElements[MapMenuWidget.AUDIO_OPTIONS.EFFECTSVOLUME].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue2;
			float currentValue3 = ((float)this.currentMusicVolume - (float)this.minVolume) / ((float)this.maxVolume - (float)this.minVolume);
			this.audioElements[MapMenuWidget.AUDIO_OPTIONS.MUSICVOLUME].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue3;
			float currentValue4 = ((float)this.currentVoiceoverVolume - (float)this.minVolume) / ((float)this.maxVolume - (float)this.minVolume);
			this.audioElements[MapMenuWidget.AUDIO_OPTIONS.VOICEOVERVOLUME].GetComponentInChildren<CustomScrollBar>(true).CurrentValue = currentValue4;
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

		private void CreateMap()
		{
			this.mapPieces.Clear();
			this.lastSelectedMapPiece = null;
			this.DeleteAllChildren(this.mapBigLayout);
			MapManager.DataMapReveal currentZone = Core.MapManager.GetCurrentZone();
			if (currentZone != null)
			{
				currentZone.UpdateElementsStatus();
			}
			this.mapRoot = this.CreateUIMapImage(this.mapBigLayout, "ZONES", Vector3.zero, null, false);
			this.allmapRoot = this.CreateUIMapImage(this.mapBigLayout, "REVEAL", Vector3.zero, null, false);
			this.playerSpriteInMap = this.CreateUIMapImage(this.mapBigLayout, "PLAYER", Vector3.zero, this.playerSprite, false);
			this.playerSpriteInMap.GetComponent<Image>().color = this.mapPlayerColor;
			this.elementsRoot = this.CreateUIMapImage(this.mapBigLayout, "ELEMENTS", Vector3.zero, null, false);
			foreach (string text in Core.MapManager.GetZonesList())
			{
				MapManager.DataMapReveal zone = Core.MapManager.GetZone(text);
				RectTransform rectTransform = this.CreateUIMapImage(this.mapRoot, "Map_" + text, zone.GetWorldPosition(), zone.mask, false);
				this.mapPieces[text] = rectTransform.gameObject.GetComponent<Image>();
				this.mapPieces[text].color = this.mapNotSelected;
				this.UpdateZoneElements(zone);
			}
			this.allmapRoot.gameObject.SetActive(false);
		}

		private void UpdateZoneElements(MapManager.DataMapReveal zone)
		{
			foreach (KeyValuePair<string, MapManager.ElementsRevealed> keyValuePair in zone.elements)
			{
				Sprite sprite = null;
				bool changeX = true;
				switch (keyValuePair.Value.element)
				{
				case MapManager.MapElementType.Reclinatory:
					sprite = ((!keyValuePair.Value.activatedOrOpen) ? this.mapPrieDieuOff : this.mapPrieDieuLight);
					break;
				case MapManager.MapElementType.Gate:
					sprite = ((!keyValuePair.Value.activatedOrOpen) ? this.mapGateVertical : null);
					changeX = false;
					break;
				case MapManager.MapElementType.Door:
					sprite = ((!keyValuePair.Value.activatedOrOpen) ? this.mapDoorClose : this.mapDoorOpen);
					break;
				case MapManager.MapElementType.Teleport:
					sprite = this.mapTravel;
					break;
				case MapManager.MapElementType.MeaCulpa:
					sprite = this.mapMeaCulpa;
					break;
				}
				if (sprite != null)
				{
					Vector2 vector = this.SnapToMapCell(zone.WorldToTexture(keyValuePair.Value.pos), changeX);
					RectTransform rectTransform = this.CreateUIMapImage(this.elementsRoot, keyValuePair.Key, vector, sprite, false);
					rectTransform.GetComponent<Image>().color = this.mapElementColor;
				}
			}
			if (this.mapGuiltDrop != null)
			{
				foreach (GuiltManager.GuiltDrop guiltDrop in Core.GuiltManager.GetAllDrops())
				{
					RectTransform rectTransform2 = this.CreateUIMapImage(this.elementsRoot, guiltDrop.id, zone.WorldToTexture(guiltDrop.position), this.mapGuiltDrop, false);
					rectTransform2.GetComponent<Image>().color = this.mapElementColor;
				}
			}
		}

		private RectTransform CreateUIMapImage(Transform parent, string name, Vector3 position, Sprite sprite, bool addMask = false)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(RectTransform)
			});
			gameObject.transform.SetParent(parent);
			RectTransform rectTransform = (RectTransform)gameObject.transform;
			rectTransform.localRotation = Quaternion.identity;
			rectTransform.localScale = Vector3.one;
			Vector3 localPosition;
			localPosition..ctor(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
			rectTransform.localPosition = localPosition;
			if (sprite)
			{
				rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
				Image image = gameObject.AddComponent<Image>();
				image.sprite = sprite;
				if (addMask)
				{
					gameObject.AddComponent<Mask>();
				}
			}
			else
			{
				rectTransform.sizeDelta = new Vector2(1f, 1f);
			}
			return rectTransform;
		}

		private void SetMapFocusOnPosition(float posX, float posY)
		{
			this.mapPosX = Mathf.Round(posX);
			this.mapPosY = Mathf.Round(posY);
			if (this.mapPosX % 2f != 0f)
			{
				this.mapPosX += 1f;
			}
			if (this.mapPosY % 2f != 0f)
			{
				this.mapPosY += 1f;
			}
			this.mapBigLayout.localPosition = new Vector3(-this.mapPosX, -this.mapPosY, 0f);
		}

		private void ResetMenus()
		{
			foreach (KeyValuePair<MapMenuWidget.MENU, Transform> keyValuePair in this.optionsRoot)
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
					GameObject gameObject = Object.Instantiate<GameObject>(tutorial.prefab, Vector3.zero, Quaternion.identity, this.optionsRoot[MapMenuWidget.MENU.TUTORIAL]);
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
				fsData.AsDictionary["textLanguageIndex"] = new fsData((long)this.currentTextLanguageIndex);
				fsData.AsDictionary["enableTips"] = new fsData(this.currentEnableHowToPlay);
				fsData.AsDictionary["enableControllerRumble"] = new fsData(this.currentEnableRumble);
				fsData.AsDictionary["enableCameraRumble"] = new fsData(this.currentEnableShake);
				fsData.AsDictionary["enableVsync"] = new fsData(this.currentVsync);
				fsData.AsDictionary["enableFullScreen"] = new fsData(this.currentFullScreen);
				fsData.AsDictionary["screenBrightness"] = new fsData((long)this.currentBrightness);
				fsData.AsDictionary["screenResolution"] = new fsData((long)this.currentResolution);
				fsData.AsDictionary["anisotropicFiltering"] = new fsData(Enum.GetName(typeof(AnisotropicFiltering), this.currentFilter));
				fsData.AsDictionary["resolutionMode"] = new fsData(Enum.GetName(typeof(MapMenuWidget.SCALING_STRATEGY), this.currentScalingStrategy));
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

		private void ReadOptionsFromFile()
		{
			string pathOptionsSettings = this.GetPathOptionsSettings();
			if (!File.Exists(pathOptionsSettings))
			{
				File.CreateText(pathOptionsSettings);
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
					Core.Localization.SetLanguageByIdx((int)asDictionary["textLanguageIndex"].AsInt64);
					Core.TutorialManager.TutorialsEnabled = asDictionary["enableTips"].AsBool;
					SingletonSerialized<RumbleSystem>.Instance.RumblesEnabled = asDictionary["enableControllerRumble"].AsBool;
					Core.Logic.CameraManager.ProCamera2DShake.enabled = asDictionary["enableCameraRumble"].AsBool;
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
					this.currentFullScreen = asDictionary["enableFullScreen"].AsBool;
					Screen.SetResolution(fitResolution.width, fitResolution.height, this.currentFullScreen);
					QualitySettings.anisotropicFiltering = (AnisotropicFiltering)Enum.Parse(typeof(AnisotropicFiltering), asDictionary["anisotropicFiltering"].AsString);
					this.currentScalingStrategy = (MapMenuWidget.SCALING_STRATEGY)Enum.Parse(typeof(MapMenuWidget.SCALING_STRATEGY), asDictionary["resolutionMode"].AsString);
					this.ApplyVideoOptionsFromFile();
					Core.Audio.MasterVolume = (float)((int)asDictionary["masterVolume"].AsInt64) / 100f;
					Core.Audio.SetSfxVolume((float)((int)asDictionary["effectsVolume"].AsInt64) / 100f);
					Core.Audio.SetMusicVolume((float)((int)asDictionary["musicVolume"].AsInt64) / 100f);
					Core.Audio.SetVoiceoverVolume((float)((int)asDictionary["voiceVolume"].AsInt64) / 100f);
				}
			}
		}

		private string GetPathOptionsSettings()
		{
			return PersistentManager.GetPathAppSettings("/options_settings.json");
		}

		[SerializeField]
		[BoxGroup("Options", true, false, 0)]
		private Dictionary<MapMenuWidget.MENU, Transform> optionsRoot = new Dictionary<MapMenuWidget.MENU, Transform>();

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
		private Dictionary<MapMenuWidget.GAME_OPTIONS, SelectableOption> gameElements;

		[SerializeField]
		[BoxGroup("Options Accessibility", true, false, 0)]
		private Dictionary<MapMenuWidget.ACCESSIBILITY_OPTIONS, GameObject> accessibilityElements;

		[SerializeField]
		[BoxGroup("Options Video", true, false, 0)]
		private Dictionary<MapMenuWidget.VIDEO_OPTIONS, GameObject> videoElements;

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
		[BoxGroup("Options Audio", true, false, 0)]
		private Dictionary<MapMenuWidget.AUDIO_OPTIONS, GameObject> audioElements;

		[SerializeField]
		[BoxGroup("Options Audio", true, false, 0)]
		private int minVolume;

		[SerializeField]
		[BoxGroup("Options Audio", true, false, 0)]
		private int maxVolume = 100;

		[SerializeField]
		[BoxGroup("Options Audio", true, false, 0)]
		private int stepVolume = 10;

		[BoxGroup("Header", true, false, 0)]
		[SerializeField]
		private Text domainCaption;

		[BoxGroup("Header", true, false, 0)]
		[SerializeField]
		private Text zoneCaption;

		[BoxGroup("Navigation Buttons", true, false, 0)]
		[SerializeField]
		private GameObject navigationButtonsRoot;

		[BoxGroup("Map", true, false, 0)]
		[SerializeField]
		private Transform mapBigLayout;

		[BoxGroup("Map", true, false, 0)]
		[SerializeField]
		private float scrollSpeedX = 1000f;

		[BoxGroup("Map", true, false, 0)]
		[SerializeField]
		private float scrollSpeedY = 400f;

		[BoxGroup("Map", true, false, 0)]
		[SerializeField]
		private Vector2 scrollLimitsMin;

		[BoxGroup("Map", true, false, 0)]
		[SerializeField]
		private Vector2 scrollLimitsMax;

		[BoxGroup("Map", true, false, 0)]
		[SerializeField]
		private Color mapElementColor = new Color(1f, 1f, 1f, 0.4f);

		[BoxGroup("Map", true, false, 0)]
		[SerializeField]
		private Color mapPlayerColor = new Color(1f, 1f, 1f, 0.6f);

		[BoxGroup("Map", true, false, 0)]
		[SerializeField]
		private Color mapNotSelected = new Color(0.7f, 0.7f, 0.7f, 1f);

		[BoxGroup("Map", true, false, 0)]
		[SerializeField]
		private Color mapSelected = new Color(1f, 1f, 1f, 1f);

		[BoxGroup("Map Sprites", true, false, 0)]
		[SerializeField]
		private Sprite playerSprite;

		[BoxGroup("Map Sprites", true, false, 0)]
		[SerializeField]
		private Sprite mapGateVertical;

		[BoxGroup("Map Sprites", true, false, 0)]
		[SerializeField]
		private Sprite mapDoorOpen;

		[BoxGroup("Map Sprites", true, false, 0)]
		[SerializeField]
		private Sprite mapDoorClose;

		[BoxGroup("Map Sprites", true, false, 0)]
		[SerializeField]
		private Sprite mapPrieDieuLight;

		[BoxGroup("Map Sprites", true, false, 0)]
		[SerializeField]
		private Sprite mapPrieDieuOff;

		[BoxGroup("Map Sprites", true, false, 0)]
		[SerializeField]
		private Sprite mapMeaCulpa;

		[BoxGroup("Map Sprites", true, false, 0)]
		[SerializeField]
		private Sprite mapTravel;

		[BoxGroup("Map Sprites", true, false, 0)]
		[SerializeField]
		private Sprite mapGuiltDrop;

		[BoxGroup("MapAutomatic", true, false, 0)]
		[SerializeField]
		private float mapEpsilonSqrDistance = 30f;

		[BoxGroup("MapAutomatic", true, false, 0)]
		[SerializeField]
		private AnimationCurve automaticFactor;

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

		[SerializeField]
		[BoxGroup("DisableMap", true, false, 0)]
		private List<GameObject> elementsWhenEnabled;

		[SerializeField]
		[BoxGroup("DisableMap", true, false, 0)]
		private List<GameObject> elementsWhenDisabled;

		public ControlsMenuScreen controlsMenuScreen;

		private Animator animator;

		private GameObject optionLastSelected;

		private MapMenuWidget.STATE _currentState;

		private MapMenuWidget.MENU currentMenu;

		private Player rewired;

		private float mapPosX;

		private float mapPosY;

		private RectTransform playerSpriteInMap;

		private RectTransform mapRoot;

		private RectTransform allmapRoot;

		private RectTransform elementsRoot;

		private bool automaticScroll;

		private float prevVerticalAxis;

		private float prevHorizontalAxis;

		private float timeInAutomatic;

		private Dictionary<string, Image> mapPieces = new Dictionary<string, Image>();

		private Image lastSelectedMapPiece;

		private float lastHorizontalInOptions;

		private bool initialOptions;

		public const float VirtualFrameWidth = 640f;

		public const float VirtualFrameHeight = 360f;

		private List<Resolution> resolutions = new List<Resolution>();

		private Resolution savedResolution;

		private int currentTextLanguageIndex;

		private bool currentEnableHowToPlay;

		private bool currentEnableRumble;

		private bool currentEnableShake;

		private bool currentVsync;

		private bool currentFullScreen;

		private int currentBrightness;

		private int currentResolution = -1;

		private AnisotropicFiltering currentFilter = 1;

		private MapMenuWidget.SCALING_STRATEGY currentScalingStrategy;

		private int currentMasterVolume;

		private int currentEffectsVolume;

		private int currentMusicVolume;

		private int currentVoiceoverVolume;

		private int nativeHeightRes;

		private int nativeWidthRes;

		private MapMenuWidget.GAME_OPTIONS optionLastGameSelected;

		private MapMenuWidget.ACCESSIBILITY_OPTIONS optionLastAccessibilitySelected;

		private MapMenuWidget.VIDEO_OPTIONS optionLastVideoSelected;

		private MapMenuWidget.AUDIO_OPTIONS optionLastAudioSelected;

		private bool mapEnabled = true;

		[BoxGroup("Cell fix", true, false, 0)]
		[SerializeField]
		private int CELL_WIDTH = 18;

		[BoxGroup("Cell fix", true, false, 0)]
		[SerializeField]
		private int CELL_HEIGHT = 10;

		[BoxGroup("Cell fix", true, false, 0)]
		[SerializeField]
		private float FACTOR_X = 1f;

		[BoxGroup("Cell fix", true, false, 0)]
		[SerializeField]
		private float FACTOR_Y = 1f;

		[BoxGroup("Cell fix", true, false, 0)]
		[SerializeField]
		private List<MapMenuWidget.RangePixels> cellPixelFix = new List<MapMenuWidget.RangePixels>();

		private int currentTutorial;

		private Dictionary<string, GameObject> tutorialInstances = new Dictionary<string, GameObject>();

		private List<string> tutorialOrder = new List<string>();

		private const string OPTIONS_SETTINGS_FILE = "/options_settings.json";

		private enum STATE
		{
			STATE_OFF,
			STATE_MAP,
			STATE_LEYEND,
			STATE_OPTIONS
		}

		private enum MENU
		{
			OPTIONS,
			GAME,
			ACCESSIBILITY,
			VIDEO,
			AUDIO,
			TUTORIAL,
			CONTROLS
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
			SHAKEENABLED
		}

		public enum VIDEO_OPTIONS
		{
			VSYNC,
			WINDOWMODE,
			BRIGHTNES,
			RESOLUTION,
			FILTERING,
			RESOLUTIONMODE
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

		[Serializable]
		private class RangePixels
		{
			public int cellMin;

			public int cellMax;

			public int value;
		}
	}
}
