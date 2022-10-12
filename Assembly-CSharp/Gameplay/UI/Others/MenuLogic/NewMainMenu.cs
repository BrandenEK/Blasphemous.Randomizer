using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using FullSerializer;
using I2.Loc;
using Rewired;
using Sirenix.OdinInspector;
using Tools.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class NewMainMenu : MonoBehaviour
	{
		public bool currentlyActive
		{
			get
			{
				return this.currentState != NewMainMenu.MenuState.OFF;
			}
		}

		private void Awake()
		{
			if (Application.runInBackground)
			{
				Debug.LogWarning("Run in background was true! Correcting.");
			}
			Application.runInBackground = false;
			this.animator = base.GetComponent<Animator>();
			this.selectSaveSlotsWidget.gameObject.SetActive(true);
			this.selectSaveSlotsWidget.Clear();
			this.SetState(NewMainMenu.MenuState.OFF);
			this.bossRushWidget.InitializeWidget();
			string pathAppSettings = NewMainMenu.GetPathAppSettings();
			if (!File.Exists(pathAppSettings))
			{
				File.CreateText(pathAppSettings).Close();
			}
			else
			{
				this.ReadFileSelectedBackground(pathAppSettings);
			}
			this.rewiredPlayer = ReInput.players.GetPlayer(0);
			this.SetBackgroundSpriteAndAnimation();
			this.ShowMainMenuOptions();
			this.HideChoosingBackgroundOptions();
			this.UpdateBackgroundLabelText();
		}

		private void Update()
		{
			if (!this.currentlyActive)
			{
				return;
			}
			if (this.menuButtonsNavEnabled && (UIController.instance.IsPatchNotesShowing() || UIController.instance.IsModeUnlockedShowing()))
			{
				this.DisableMenuButtonsNav();
			}
			else if (!this.menuButtonsNavEnabled && !UIController.instance.IsPatchNotesShowing() && !UIController.instance.IsModeUnlockedShowing())
			{
				this.EnableMenuButtonsNav();
			}
			if (this.isChoosingBackground)
			{
				bool buttonDown = this.rewiredPlayer.GetButtonDown(50);
				if (buttonDown)
				{
					this.ProcessSubmitInput();
					return;
				}
				bool buttonDown2 = this.rewiredPlayer.GetButtonDown(51);
				if (buttonDown2)
				{
					this.ProcessBackInput();
					return;
				}
				float axisPrev = this.rewiredPlayer.GetAxisPrev(48);
				if (axisPrev < 0.3f && axisPrev > -0.3f)
				{
					bool flag = this.rewiredPlayer.GetAxisRaw(48) > 0.3f;
					if (flag)
					{
						this.ProcessMoveInput(true);
						return;
					}
					bool flag2 = this.rewiredPlayer.GetAxisRaw(48) < -0.3f;
					if (flag2)
					{
						this.ProcessMoveInput(false);
					}
				}
			}
			else
			{
				this.CheckSequenceButtons();
			}
		}

		private void CheckSequenceButtons()
		{
			NewMainMenu.SequenceButtons sequenceButtons = NewMainMenu.SequenceButtons.DUMMY;
			if (this.CheckRight())
			{
				sequenceButtons = NewMainMenu.SequenceButtons.RIGHT;
			}
			if (sequenceButtons == NewMainMenu.SequenceButtons.DUMMY && this.CheckLeft())
			{
				sequenceButtons = NewMainMenu.SequenceButtons.LEFT;
			}
			if (sequenceButtons == NewMainMenu.SequenceButtons.DUMMY && this.CheckUp())
			{
				sequenceButtons = NewMainMenu.SequenceButtons.UP;
			}
			if (sequenceButtons == NewMainMenu.SequenceButtons.DUMMY && this.CheckDown())
			{
				sequenceButtons = NewMainMenu.SequenceButtons.DOWN;
			}
			if (sequenceButtons != NewMainMenu.SequenceButtons.DUMMY)
			{
				this.UpdateSequence(sequenceButtons);
			}
		}

		private bool CheckRight()
		{
			float axisPrev = this.rewiredPlayer.GetAxisPrev(48);
			return axisPrev < 0.3f && axisPrev > -0.3f && this.rewiredPlayer.GetAxisRaw(48) > 0.3f;
		}

		private bool CheckLeft()
		{
			float axisPrev = this.rewiredPlayer.GetAxisPrev(48);
			return axisPrev < 0.3f && axisPrev > -0.3f && this.rewiredPlayer.GetAxisRaw(48) < -0.3f;
		}

		private bool CheckUp()
		{
			float axisPrev = this.rewiredPlayer.GetAxisPrev(49);
			return axisPrev < 0.3f && axisPrev > -0.3f && this.rewiredPlayer.GetAxisRaw(49) > 0.3f;
		}

		private bool CheckDown()
		{
			float axisPrev = this.rewiredPlayer.GetAxisPrev(49);
			return axisPrev < 0.3f && axisPrev > -0.3f && this.rewiredPlayer.GetAxisRaw(49) < -0.3f;
		}

		private void UpdateSequence(NewMainMenu.SequenceButtons currentSequenceButton)
		{
			if (this.currentSequence.Count < this.skinSequence.Count)
			{
				NewMainMenu.SequenceButtons sequenceButtons = this.skinSequence[this.currentSequence.Count];
				if (sequenceButtons == currentSequenceButton)
				{
					this.currentSequence.Add(currentSequenceButton);
				}
				else
				{
					this.currentSequence.Clear();
				}
			}
			if (this.currentSequence.Count == this.skinSequence.Count)
			{
				this.currentSequence.Clear();
				if (Core.ColorPaletteManager.IsColorPaletteUnlocked("PENITENT_KONAMI"))
				{
					Core.Audio.PlayOneShot(this.SkinAlreadyUnlockedSFX, default(Vector3));
				}
				else
				{
					Core.Audio.PlayOneShot(this.SkinUnlockedSFX, default(Vector3));
					Core.ColorPaletteManager.UnlockBossKonamiColorPalette();
				}
			}
		}

		public void OptionCampain()
		{
			if (this.currentState != NewMainMenu.MenuState.MENU)
			{
				return;
			}
			this.SetState(NewMainMenu.MenuState.SLOT);
			this.selectSaveSlotsWidget.SetAllData(this, SelectSaveSlots.SlotsModes.Normal);
			base.StartCoroutine(this.ShowSlotSaveWidget());
		}

		public void OptionBossRush()
		{
			if (this.currentState != NewMainMenu.MenuState.MENU)
			{
				return;
			}
			base.StartCoroutine(this.ShowWidgetForBossRush());
		}

		public void OptionOptions()
		{
			if (this.currentState != NewMainMenu.MenuState.MENU)
			{
				return;
			}
			base.StartCoroutine(this.ShowOptionsFromMap());
		}

		public void OptionExtras()
		{
			if (this.currentState != NewMainMenu.MenuState.MENU)
			{
				return;
			}
			this.ShowExtras();
		}

		public void OptionExitGame()
		{
			if (this.currentState != NewMainMenu.MenuState.MENU)
			{
				return;
			}
			Application.Quit();
		}

		public void SetConfirmationDeleteFromSlot()
		{
			this.SetState(NewMainMenu.MenuState.CONFIRM);
		}

		public void SetNormalModeFromConfirmation()
		{
			this.SetState(NewMainMenu.MenuState.SLOT);
		}

		public void ShowMenu(string newInitialScene = "")
		{
			PersistentManager.ResetAutomaticSlot();
			this.sceneName = ((!(newInitialScene != string.Empty)) ? this.initialSceneName : newInitialScene);
			if (NewMainMenu.OnMenuOpen != null)
			{
				NewMainMenu.OnMenuOpen();
			}
			Core.Audio.Ambient.SetSceneParams(this.trackIdentifier, string.Empty, new AudioParam[0], string.Empty);
			this.SetState(NewMainMenu.MenuState.MENU);
			this.SetBackgroundSpriteAndAnimation();
			Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.MENU);
			this.ShowMainMenuOptions();
		}

		public IEnumerator ShowOptionsFromMap()
		{
			this.SetState(NewMainMenu.MenuState.OPTIONS);
			yield return UIController.instance.ShowOptions();
			this.SetState(NewMainMenu.MenuState.MENU);
			yield break;
		}

		public void ShowExtras()
		{
			this.SetState(NewMainMenu.MenuState.OFF);
			UIController.instance.ShowExtras();
		}

		public void ShowChooseBackground()
		{
			if (NewMainMenu.OnMenuOpen != null)
			{
				NewMainMenu.OnMenuOpen();
			}
			Core.Audio.Ambient.SetSceneParams(this.trackIdentifier, string.Empty, new AudioParam[0], string.Empty);
			this.ShowChoosingBackgroundOptions();
			this.HideMainMenuOptions();
			this.SetState(NewMainMenu.MenuState.CHOOSE_BACKGROUND);
		}

		public IEnumerator ShowSlotSaveWidget()
		{
			while (this.selectSaveSlotsWidget.IsShowing)
			{
				yield return new WaitForEndOfFrame();
			}
			if (this.selectSaveSlotsWidget.SelectedSlot >= 0)
			{
				PersistentManager.SetAutomaticSlot(this.selectSaveSlotsWidget.SelectedSlot);
				this.isContinue = this.selectSaveSlotsWidget.CanLoadSelectedSlot;
				this.mustConvertToNewgamePlus = this.selectSaveSlotsWidget.MustConvertToNewgamePlus;
				if (this.isContinue)
				{
					if (this.mustConvertToNewgamePlus)
					{
						Log.Trace("Continue pressed and new game plus, starting the game...", null);
					}
					else
					{
						Log.Trace("Continue pressed, starting the game...", null);
					}
				}
				else
				{
					Log.Trace("Play pressed, starting the game...", null);
				}
				EventSystem.current.SetSelectedGameObject(null);
				this.SetState(NewMainMenu.MenuState.OFF);
				Core.Audio.Ambient.StopCurrent();
				this.InternalPlay();
			}
			else
			{
				this.SetState(NewMainMenu.MenuState.MENU);
			}
			yield return null;
			yield break;
		}

		public IEnumerator ShowWidgetForBossRush()
		{
			this.mainMenuKeepFocus.enabled = false;
			EventSystem.current.SetSelectedGameObject(null);
			this.SetState(NewMainMenu.MenuState.MENU);
			this.currentState = NewMainMenu.MenuState.BOSSRUSH;
			this.bossRushWidget.FadeShow(false, false, true);
			while (this.bossRushWidget.IsActive())
			{
				yield return new WaitForEndOfFrame();
			}
			bool returnToMenu = true;
			if (this.bossRushWidget.IsAllSelected)
			{
				this.InternalBossRush();
				returnToMenu = false;
			}
			if (returnToMenu)
			{
				this.mainMenuKeepFocus.enabled = true;
				this.SetState(NewMainMenu.MenuState.MENU);
			}
			yield return null;
			yield break;
		}

		private bool IsAnySlotForBossRush()
		{
			Core.BossRushManager.CheckCoursesUnlockBySlots();
			return Core.BossRushManager.IsAnyCourseUnlocked();
		}

		private void InternalPlay()
		{
			Core.SpawnManager.InitialScene = this.sceneName;
			Core.LevelManager.ActivatePrecachedScene();
			UIController.instance.HideMainMenu();
			if (this.mustConvertToNewgamePlus)
			{
				Core.Persistence.LoadGameWithOutRespawn(PersistentManager.GetAutomaticSlot());
				Core.GameModeManager.ConvertCurrentGameToPlus();
				Core.Persistence.SaveGame(false);
				Core.SpawnManager.FirstSpanw = true;
				Core.SpawnManager.SetInitialSpawn(this.sceneName);
				Core.LevelManager.ChangeLevel(this.sceneName, false, true, false, null);
				return;
			}
			if (this.isContinue)
			{
				Core.Persistence.LoadGame(PersistentManager.GetAutomaticSlot());
				return;
			}
			Core.Logic.ResetAllData();
			Core.Persistence.DeleteSaveGame(PersistentManager.GetAutomaticSlot());
			Core.SpawnManager.FirstSpanw = true;
			Core.Randomizer.newGame();
			Core.GameModeManager.ChangeMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS);
			Core.SpawnManager.SetInitialSpawn(this.sceneName);
			Core.LevelManager.ChangeLevel(this.sceneName, false, true, false, null);
		}

		private void InternalBossRush()
		{
			this.SetState(NewMainMenu.MenuState.OFF);
			this.mainMenuKeepFocus.enabled = true;
			UIController.instance.HideMainMenu();
			Core.BossRushManager.StartCourse(this.bossRushWidget.SelectedCourse, this.bossRushWidget.SelectedMode, this.bossRushWidget.CurrentSlot);
		}

		private void SetState(NewMainMenu.MenuState state)
		{
			this.currentState = state;
			this.animator.SetInteger("STATUS", (int)this.currentState);
		}

		private void ProcessSubmitInput()
		{
			if (this.soundOnAccept != string.Empty)
			{
				Core.Audio.PlayOneShot(this.soundOnAccept, default(Vector3));
			}
			string pathAppSettings = NewMainMenu.GetPathAppSettings();
			this.WriteFileAppSettings(pathAppSettings);
			this.HideChoosingBackgroundOptions();
			this.ShowMainMenuOptions();
			this.SetState(NewMainMenu.MenuState.MENU);
		}

		public void PlaySoundOnBack()
		{
			if (this.soundOnBack != string.Empty)
			{
				Core.Audio.PlayOneShot(this.soundOnBack, default(Vector3));
			}
		}

		private void ProcessBackInput()
		{
			this.PlaySoundOnBack();
			string pathAppSettings = NewMainMenu.GetPathAppSettings();
			this.ReadFileSelectedBackground(pathAppSettings);
			this.SetBackgroundSpriteAndAnimation();
			this.HideChoosingBackgroundOptions();
			this.ShowMainMenuOptions();
			this.SetState(NewMainMenu.MenuState.MENU);
		}

		private void ProcessMoveInput(bool movingRight)
		{
			if (this.soundOnMove != string.Empty)
			{
				Core.Audio.PlayOneShot(this.soundOnMove, default(Vector3));
			}
			if (movingRight)
			{
				this.bgIndex++;
				if (this.bgIndex > 3)
				{
					this.bgIndex = 0;
				}
			}
			else
			{
				this.bgIndex--;
				if (this.bgIndex < 0)
				{
					this.bgIndex = 3;
				}
			}
			this.SetBackgroundSpriteAndAnimation();
			this.UpdateBackgroundLabelText();
		}

		private void ShowChoosingBackgroundOptions()
		{
			this.isChoosingBackground = true;
			this.choosingBackgroundOptions.ForEach(delegate(GameObject x)
			{
				x.SetActive(true);
			});
			this.UpdateBackgroundLabelText();
		}

		private void HideChoosingBackgroundOptions()
		{
			this.isChoosingBackground = false;
			this.choosingBackgroundOptions.ForEach(delegate(GameObject x)
			{
				x.SetActive(false);
			});
		}

		private void ShowMainMenuOptions()
		{
			this.mainMenuOptions.ForEach(delegate(GameObject x)
			{
				x.SetActive(true);
			});
			this.ShowBossRush = this.IsAnySlotForBossRush();
			if (this.ShowBossRush)
			{
				this.BossRushButton.gameObject.SetActive(true);
				Selectable selectOnUp = this.BossRushButton.navigation.selectOnUp;
				Navigation navigation = selectOnUp.navigation;
				navigation.selectOnDown = this.BossRushButton;
				selectOnUp.navigation = navigation;
				Selectable selectOnDown = this.BossRushButton.navigation.selectOnDown;
				Navigation navigation2 = selectOnDown.navigation;
				navigation2.selectOnUp = this.BossRushButton;
				selectOnDown.navigation = navigation2;
				UIController.instance.ShowModeUnlockedWidget(ModeUnlockedWidget.ModesToUnlock.BossRush);
			}
			else
			{
				Selectable selectOnUp2 = this.BossRushButton.navigation.selectOnUp;
				Selectable selectOnDown2 = this.BossRushButton.navigation.selectOnDown;
				Navigation navigation3 = selectOnUp2.navigation;
				navigation3.selectOnDown = selectOnDown2;
				selectOnUp2.navigation = navigation3;
				Navigation navigation4 = selectOnDown2.navigation;
				navigation4.selectOnUp = selectOnUp2;
				selectOnDown2.navigation = navigation4;
				this.BossRushButton.gameObject.SetActive(false);
			}
		}

		private void HideMainMenuOptions()
		{
			this.mainMenuOptions.ForEach(delegate(GameObject x)
			{
				x.SetActive(false);
			});
		}

		private void SetBackgroundSpriteAndAnimation()
		{
			this.background.sprite = this.availableBackgrounds[this.bgIndex];
			this.backgroundAnimator.SetInteger("BG_INDEX", this.bgIndex);
		}

		private void UpdateBackgroundLabelText()
		{
			switch (this.bgIndex)
			{
			case 0:
				this.backgroundLabel.text = ScriptLocalization.UI_Extras.BACKGROUND_0_LABEL;
				return;
			case 1:
				this.backgroundLabel.text = ScriptLocalization.UI_Extras.BACKGROUND_1_LABEL;
				return;
			case 2:
				this.backgroundLabel.text = ScriptLocalization.UI_Extras.BACKGROUND_2_LABEL;
				return;
			case 3:
				this.backgroundLabel.text = ScriptLocalization.UI_Extras.BACKGROUND_3_LABEL;
				return;
			default:
				return;
			}
		}

		private void DisableMenuButtonsNav()
		{
			this.menuButtonsNavEnabled = false;
			this.AllButtons.ForEach(delegate(Button x)
			{
				Navigation navigation = x.navigation;
				navigation.mode = Navigation.Mode.None;
				x.navigation = navigation;
				x.interactable = false;
			});
		}

		private void EnableMenuButtonsNav()
		{
			this.menuButtonsNavEnabled = true;
			this.AllButtons.ForEach(delegate(Button x)
			{
				Navigation navigation = x.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				x.navigation = navigation;
				x.interactable = true;
			});
		}

		private static string GetPathAppSettings()
		{
			return PersistentManager.GetPathAppSettings("/app_settings");
		}

		private void ReadFileSelectedBackground(string filePath)
		{
			this.bgIndex = 3;
			fsData fsData = new fsData();
			string s;
			bool flag = PersistentManager.TryToReadFile(filePath, out s);
			if (flag)
			{
				byte[] bytes = Convert.FromBase64String(s);
				string @string = Encoding.UTF8.GetString(bytes);
				fsResult fsResult = fsJsonParser.Parse(@string, out fsData);
				if (fsResult.Failed && !fsResult.FormattedMessages.Equals("No input"))
				{
					Debug.LogError("Parsing error: " + fsResult.FormattedMessages);
				}
				else if (fsData != null)
				{
					Dictionary<string, fsData> asDictionary = fsData.AsDictionary;
					bool flag2 = false;
					if (asDictionary.ContainsKey("latest_background_index") && asDictionary["latest_background_index"].IsInt64)
					{
						int num = (int)asDictionary["latest_background_index"].AsInt64;
						if (num != 3)
						{
							flag2 = true;
						}
					}
					else
					{
						flag2 = true;
					}
					if (flag2)
					{
						this.bgIndex = 3;
					}
					else if (asDictionary.ContainsKey("main_menu_background") && asDictionary["main_menu_background"].IsInt64)
					{
						int value = (int)asDictionary["main_menu_background"].AsInt64;
						this.bgIndex = Mathf.Clamp(value, 0, 3);
					}
				}
			}
			this.bgIndex = Mathf.Clamp(this.bgIndex, 0, this.availableBackgrounds.Length - 1);
			this.background.sprite = this.availableBackgrounds[this.bgIndex];
		}

		private void WriteFileAppSettings(string filePath)
		{
			fsData fsData = PersistentManager.ReadAppSettings(filePath);
			if (fsData == null || !fsData.IsDictionary)
			{
				return;
			}
			fsData.AsDictionary["main_menu_background"] = new fsData((long)this.bgIndex);
			fsData.AsDictionary["latest_background_index"] = new fsData(3L);
			string s = fsJsonPrinter.CompressedJson(fsData);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			string contents = Convert.ToBase64String(bytes);
			File.WriteAllText(filePath, contents);
		}

		public static Core.SimpleEvent OnMenuOpen;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		protected string trackIdentifier;

		[BoxGroup("Audio Settings", true, false, 0)]
		[SerializeField]
		[EventRef]
		private string soundOnAccept = "event:/SFX/UI/ChangeTab";

		[BoxGroup("Audio Settings", true, false, 0)]
		[SerializeField]
		[EventRef]
		private string soundOnBack = "event:/SFX/UI/ChangeTab";

		[BoxGroup("Audio Settings", true, false, 0)]
		[SerializeField]
		[EventRef]
		private string soundOnMove = "event:/SFX/UI/ChangeTab";

		[BoxGroup("Config", true, false, 0)]
		public string initialSceneName;

		[BoxGroup("Widgets", true, false, 0)]
		[SerializeField]
		private SelectSaveSlots selectSaveSlotsWidget;

		[BoxGroup("Widgets", true, false, 0)]
		[SerializeField]
		private BossRushWidget bossRushWidget;

		[BoxGroup("Widgets", true, false, 0)]
		[SerializeField]
		private KeepFocus mainMenuKeepFocus;

		[BoxGroup("Widgets", true, false, 0)]
		[SerializeField]
		private List<GameObject> choosingBackgroundOptions;

		[BoxGroup("Widgets", true, false, 0)]
		[SerializeField]
		private List<GameObject> mainMenuOptions;

		[BoxGroup("Widgets Background", true, false, 0)]
		[SerializeField]
		private Sprite[] availableBackgrounds;

		[BoxGroup("Widgets Background", true, false, 0)]
		[SerializeField]
		private Animator backgroundAnimator;

		[BoxGroup("Widgets Background", true, false, 0)]
		[SerializeField]
		private Text backgroundLabel;

		[BoxGroup("Widgets Background", true, false, 0)]
		[SerializeField]
		private Image background;

		[BoxGroup("Widgets", true, false, 0)]
		[SerializeField]
		private Button BossRushButton;

		[BoxGroup("Widgets", true, false, 0)]
		[SerializeField]
		private List<Button> AllButtons;

		private const string APP_SETTINGS_MAIN_MENU_BG_KEY = "main_menu_background";

		private const string APP_SETTINGS_LATEST_BACKGROUND_INDEX_KEY = "latest_background_index";

		private const string ANIMATOR_STATE = "STATUS";

		private const int LATEST_BACKGROUND_INDEX = 3;

		private bool ShowBossRush;

		private NewMainMenu.MenuState currentState;

		private Player rewiredPlayer;

		private Animator animator;

		private float timeWaiting;

		private string sceneName;

		private bool mustConvertToNewgamePlus;

		private bool isChoosingBackground;

		private bool isContinue;

		public int bgIndex;

		private bool menuButtonsNavEnabled = true;

		[ShowInInspector]
		private List<NewMainMenu.SequenceButtons> skinSequence = new List<NewMainMenu.SequenceButtons>
		{
			NewMainMenu.SequenceButtons.UP,
			NewMainMenu.SequenceButtons.UP,
			NewMainMenu.SequenceButtons.DOWN,
			NewMainMenu.SequenceButtons.DOWN,
			NewMainMenu.SequenceButtons.LEFT,
			NewMainMenu.SequenceButtons.RIGHT,
			NewMainMenu.SequenceButtons.LEFT,
			NewMainMenu.SequenceButtons.RIGHT
		};

		[ShowInInspector]
		private List<NewMainMenu.SequenceButtons> currentSequence = new List<NewMainMenu.SequenceButtons>();

		[EventRef]
		public string SkinUnlockedSFX = "event:/SFX/DEMAKE/DSkinItem";

		[EventRef]
		public string SkinAlreadyUnlockedSFX = "event:/SFX/DEMAKE/DPlatformCollapse";

		private enum MenuState
		{
			OFF,
			MENU,
			SLOT,
			FADEOUT,
			CONFIRM,
			OPTIONS,
			CHOOSE_BACKGROUND,
			BOSSRUSH
		}

		private enum SequenceButtons
		{
			LEFT,
			RIGHT,
			UP,
			DOWN,
			DUMMY
		}
	}
}
