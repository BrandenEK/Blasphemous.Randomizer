using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.Achievements;
using Framework.BossRush;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Isidora;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI.Others.Buttons;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Others.UIGameLogic;
using Gameplay.UI.Widgets;
using I2.Loc;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI
{
	[RequireComponent(typeof(Canvas))]
	[DefaultExecutionOrder(-1)]
	public class UIController : SerializedMonoBehaviour
	{
		public bool CanEquipSwordHearts
		{
			get
			{
				return this._canEquipSwordHearts;
			}
			set
			{
				this._canEquipSwordHearts = value;
			}
		}

		private void Awake()
		{
			UIController.instance = this;
			int childCount = this.content.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				if (!(base.transform.name == "UNUSED"))
				{
					this.content.transform.GetChild(i).gameObject.SetActive(true);
				}
			}
			this.EnableCanvas(this.newInventoryMenu.gameObject, false);
			this.tutorialWidget.SetActive(false);
			if (this.unlockWidget != null)
			{
				this.unlockWidget.SetActive(false);
			}
			this.fullMessageCanvas = this.fullMessages.GetComponent<CanvasGroup>();
			this.fullMessageCanvas.alpha = 0f;
			this.fullMessages.SetActive(false);
			if (EventSystem.current)
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
			this.patchNotesWidget.gameObject.SetActive(false);
			this.upgradeFlasksWidget.gameObject.SetActive(false);
			this.choosePenitenceWidget.gameObject.SetActive(false);
			this.quoteWidget.gameObject.SetActive(false);
			this.confirmationWidget.gameObject.SetActive(false);
			this.abandonPenitenceWidget.gameObject.SetActive(false);
			this.introDemakeWidget.gameObject.SetActive(false);
			this.modeUnlockedWidget.gameObject.SetActive(false);
			this.gameplayWidget.ShowPurgePoints();
			this.allUIBLockingWidgets.Clear();
			this.allUIBLockingWidgets.Add(this.pauseWidget);
			this.allUIBLockingWidgets.Add(this.almsWidget);
			this.allUIBLockingWidgets.Add(this.teleportWidget);
			this.allUIBLockingWidgets.Add(this.bossRushRankWidget);
			foreach (BasicUIBlockingWidget basicUIBlockingWidget in this.allUIBLockingWidgets)
			{
				basicUIBlockingWidget.InitializeWidget();
			}
			this.firstRun = true;
		}

		private void Start()
		{
			Canvas component = base.GetComponent<Canvas>();
			component.planeDistance = 1f;
			component.sortingLayerName = "Canvas UI";
			SpawnManager.OnPlayerSpawn += this.OnPenitentReady;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			LevelManager.OnBeforeLevelLoad += this.OnBeforeLevelLoad;
			this.rewired = ReInput.players.GetPlayer(0);
		}

		private void OnPenitentReady(Penitent penitent)
		{
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnDead));
		}

		private void OnBeforeLevelLoad(Level oldLevel, Level newLevel)
		{
			this.popUpWidget.HideAreaPopup();
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			if (this.firstRun)
			{
				this.firstRun = false;
				this.pauseWidget.ReadOptionConfigurations();
				Debug.LogWarning("<color=yellow>Removing initial input block</color>");
				Core.Input.SetBlocker("InitialBlocker", false);
			}
			if (this.fullMessages.activeInHierarchy)
			{
				base.StopCoroutine("ShowFullMessageCourrutine");
				this.fullMessages.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPenitentReady;
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			LevelManager.OnBeforeLevelLoad -= this.OnBeforeLevelLoad;
			this.firstRun = false;
		}

		private void Update()
		{
			bool flag = false;
			if (this.rewired == null)
			{
				return;
			}
			if (this.dialogWidget.IsShowingDialog())
			{
				flag = true;
				if (this.rewired.GetButtonDown(39))
				{
					this.dialogWidget.DialogButtonPressed();
				}
			}
			if (this.IsTutorialActive() || CreditsWidget.instance != null || (ConsoleWidget.Instance != null && ConsoleWidget.Instance.IsEnabled()) || this.fullMessages.activeInHierarchy || this.patchNotesWidget.isOpen || this.modeUnlockedWidget.isOpen)
			{
				flag = true;
			}
			bool flag2 = this.rewired.GetButtonDown(51);
			if (!flag && flag2)
			{
				if (this.newInventoryMenu.currentlyActive)
				{
					base.StartCoroutine(this.SafeGoBackInventory());
					flag = true;
				}
				else if (this.extrasMenu.currentlyActive)
				{
					base.StartCoroutine(this.SafeGoBackExtras());
					flag = true;
				}
				else if (this.quoteWidget.IsOpen)
				{
					this.quoteWidget.Close();
				}
				else
				{
					foreach (BasicUIBlockingWidget basicUIBlockingWidget in this.allUIBLockingWidgets)
					{
						if (basicUIBlockingWidget.AutomaticBack() && basicUIBlockingWidget.IsActive() && !basicUIBlockingWidget.IsFading && !basicUIBlockingWidget.GoBack())
						{
							basicUIBlockingWidget.FadeHide();
						}
					}
				}
			}
			if (flag2 && (this.rewired.GetButtonDown(22) || this.rewired.GetButtonDown(10)))
			{
				flag2 = false;
			}
			if (!flag && this.pauseWidget.IsActive())
			{
				if (this.rewired.GetButtonDown(61))
				{
					flag = true;
					if (this.pauseWidget.ChangeToOptions() && !string.IsNullOrEmpty(this.sfxOpenOptions))
					{
						Core.Audio.PlayOneShot(this.sfxOpenOptions, default(Vector3));
					}
				}
				if (this.rewired.GetButtonDown(60))
				{
					this.pauseWidget.CenterView();
					flag = true;
				}
				if (this.rewired.GetButtonDown(50))
				{
					this.pauseWidget.SubmitPressed();
					flag = true;
				}
				if (this.rewired.GetButtonDown(28))
				{
					this.pauseWidget.UITabLeft();
					flag = true;
				}
				if (this.rewired.GetButtonDown(29))
				{
					this.pauseWidget.UITabRight();
					flag = true;
				}
			}
			if (!flag && this.newInventoryMenu.currentlyActive)
			{
				if (this.rewired.GetButtonDown(64))
				{
					this.newInventoryMenu.ShowLore();
					flag = true;
				}
				if (this.rewired.GetButtonDown(28))
				{
					this.newInventoryMenu.SelectPreviousCategory();
				}
				if (this.rewired.GetButtonDown(29))
				{
					this.newInventoryMenu.SelectNextCategory();
				}
			}
			if (!flag && this.almsWidget.IsActive() && this.rewired.GetButtonDown(50))
			{
				this.almsWidget.SubmitPressed();
				flag = true;
			}
			if (!flag && this.rewired.GetButtonDown(22) && !flag2 && !Core.Input.HasBlocker("INTERACTABLE"))
			{
				bool flag3 = Core.Input.HasBlocker("PLAYER_LOGIC") || Core.Logic.IsSlowMode();
				if (!this.newInventoryMenu.currentlyActive && flag3)
				{
					flag = true;
				}
				else if (this.CanOpenBlockWidget() && !this.kneelMenuWidget.IsShowing && this.CanOpenInventory)
				{
					if (this.IsInIsidoraBossfight())
					{
						if (Core.Logic != null && Core.Logic.Penitent && !Core.Logic.Penitent.Dead)
						{
							this.ShowPopUp(ScriptLocalization.UI.ISIDORA_MENU_FORBIDDEN, string.Empty, 2f, false);
						}
						flag = true;
					}
					else
					{
						this.EnableCanvas(this.newInventoryMenu.gameObject, true);
						this.newInventoryMenu.Show(!this.newInventoryMenu.currentlyActive);
						flag = true;
						if (!this.newInventoryMenu.currentlyActive)
						{
							this.CheckPrayerTutorial();
						}
					}
				}
			}
			if (!flag && this.rewired.GetButtonDown(10) && !flag2 && !Core.Input.HasBlocker("PLAYER_LOGIC"))
			{
				flag = true;
				if (this.pauseWidget.IsActive() && !this.pauseWidget.OptionsWidget.controlsMenuScreen.enabled)
				{
					this.pauseWidget.FadeHide();
				}
				else if (this.CanOpenBlockWidget())
				{
					if (this.IsInIsidoraBossfight())
					{
						if (Core.Logic != null && Core.Logic.Penitent && !Core.Logic.Penitent.Dead)
						{
							this.ShowPopUp(ScriptLocalization.UI.ISIDORA_MENU_FORBIDDEN, string.Empty, 2f, false);
						}
						flag = true;
					}
					else
					{
						this.pauseWidget.InitialWidget = PauseWidget.ChildWidgets.MAP;
						this.pauseWidget.InitialMapMode = PauseWidget.MapModes.SHOW;
						this.pauseWidget.FadeShow(true, true, true);
					}
				}
			}
		}

		private bool IsInIsidoraBossfight()
		{
			return (Core.LevelManager.currentLevel.LevelName.Equals("D01BZ08S01") || Core.LevelManager.currentLevel.LevelName.Equals("D22Z01S18")) && Object.FindObjectOfType<IsidoraBehaviour>();
		}

		internal void ShowJoysticksButtons()
		{
			throw new NotImplementedException();
		}

		internal void ShowKeyboardButtons()
		{
			throw new NotImplementedException();
		}

		private void OnDead()
		{
			foreach (BasicUIBlockingWidget basicUIBlockingWidget in this.allUIBLockingWidgets)
			{
				basicUIBlockingWidget.FadeHide();
			}
			this.newInventoryMenu.Show(false);
		}

		public void ShowKneelMenu(KneelPopUpWidget.Modes mode)
		{
			this.kneelMenuWidget.ShowPopUp(mode);
			this.CanEquipSwordHearts = true;
		}

		public void MakeKneelMenuInvisible()
		{
			this.kneelMenuWidget.HidePopUp();
		}

		public void HideKneelMenu()
		{
			this.kneelMenuWidget.HidePopUp();
			this.CanEquipSwordHearts = false;
		}

		public bool IsInventoryMenuPressed()
		{
			return this.rewired.GetButtonDown(22);
		}

		public bool IsStopKneelPressed()
		{
			return this.rewired.GetButtonDown(51);
		}

		public bool IsInventoryClosed()
		{
			return !this.newInventoryMenu.currentlyActive;
		}

		public void CloseInventory()
		{
			base.StartCoroutine(this.SafeGoBackInventory());
		}

		public void ToggleInventoryMenu()
		{
			this.EnableCanvas(this.newInventoryMenu.gameObject, true);
			this.newInventoryMenu.Show(!this.newInventoryMenu.currentlyActive);
		}

		public void SelectTab(NewInventoryWidget.TabType tab)
		{
			this.newInventoryMenu.SetTab(tab);
		}

		public bool IsTeleportMenuPressed()
		{
			return this.rewired.GetButtonDown(6);
		}

		public void ShowPopUp(string message, string eventSound = "", float timeToWait = 0f, bool blockPlayer = true)
		{
			this.popUpWidget.ShowPopUp(message, eventSound, timeToWait, blockPlayer);
		}

		public void ShowCherubPopUp(string message, string eventSound = "", float timeToWait = 0f, bool blockPlayer = true)
		{
			this.popUpWidget.ShowCherubPopUp(message, eventSound, timeToWait, blockPlayer);
		}

		public void ShowPopUpObjectUse(string itemName, string eventSound = "")
		{
			string valueWithParam = Core.Localization.GetValueWithParam(ScriptLocalization.UI_Inventory.TEXT_DOOR_USE_OBJECT, "object_caption", itemName);
			string eventSound2 = (!(eventSound != string.Empty)) ? "event:/Key Event/UseQuestItem" : eventSound;
			this.ShowPopUp(valueWithParam, eventSound2, 0f, true);
		}

		public void ShowObjectPopUp(UIController.PopupItemAction action, string itemName, Sprite image, InventoryManager.ItemType objType, float timeToWait = 3f, bool blockPlayer = false)
		{
			string message = string.Empty;
			if (action != UIController.PopupItemAction.GetObejct)
			{
				if (action == UIController.PopupItemAction.GiveObject)
				{
					message = ScriptLocalization.UI_Inventory.TEXT_ITEM_GIVE;
				}
			}
			else
			{
				message = ScriptLocalization.UI_Inventory.TEXT_ITEM_FOUND;
			}
			this.popUpWidget.ShowItemGet(message, itemName, image, objType, timeToWait, blockPlayer);
		}

		public void ShowAreaPopUp(string area, float timeToWait = 3f, bool blockPlayer = false)
		{
			base.StartCoroutine(this.ShowPopUpDelayed(area, this.waitTimeToShowZone, timeToWait, blockPlayer));
		}

		public IEnumerator ShowPopUpDelayed(string area, float timeToShow = 1f, float timeToWait = 3f, bool blockPlayer = false)
		{
			this.popUpWidget.WaitingToShowArea = true;
			yield return new WaitForSeconds(timeToShow);
			this.popUpWidget.ShowAreaPopUp(area, timeToWait, blockPlayer);
			yield break;
		}

		public bool IsShowingPopUp()
		{
			return this.popUpWidget.IsShowing;
		}

		public void ShowUnlockSKill()
		{
			if (!this.newInventoryMenu.currentlyActive)
			{
				ConsoleWidget.Instance.SetEnabled(false);
				this.EnableCanvas(this.newInventoryMenu.gameObject, true);
				this.newInventoryMenu.ShowSkills(true);
			}
		}

		public void ShowTeleportUI()
		{
			if (!this.newInventoryMenu.currentlyActive)
			{
				ConsoleWidget.Instance.SetEnabled(false);
				this.teleportWidget.FadeShow(false, true, true);
			}
		}

		public void HideInventory()
		{
			if (this.newInventoryMenu.currentlyActive)
			{
				this.newInventoryMenu.Show(false);
			}
		}

		public IEnumerator ShowUnlockSKillCourrutine()
		{
			this.ShowUnlockSKill();
			while (this.newInventoryMenu.currentlyActive)
			{
				yield return 0;
			}
			yield break;
		}

		public IEnumerator ShowAlmsWidgetCourrutine()
		{
			this.almsWidget.FadeShow(false, true, true);
			while (this.almsWidget.IsActive())
			{
				yield return 0;
			}
			yield break;
		}

		public bool IsTutorialActive()
		{
			return this.tutorialWidget.activeSelf;
		}

		public GameObject GetTutorialRoot()
		{
			return this.tutorialWidget;
		}

		public bool IsUnlockActive()
		{
			return this.unlockWidget.activeSelf;
		}

		public GameObject GetUnlockRoot()
		{
			return this.unlockWidget;
		}

		public void ShowUnlockPopup(string unlockId)
		{
			if (this.unlockWidget != null)
			{
				base.StartCoroutine(this.ShowUnlock(unlockId, true));
			}
		}

		public IEnumerator ShowUnlock(string unlockId, bool blockPlayer = true)
		{
			if (blockPlayer)
			{
				Core.Input.SetBlocker("UNLOCK", true);
				Core.Logic.PauseGame();
			}
			GameObject uiroot = this.GetUnlockRoot();
			UnlockWidget widget = this.unlockWidget.GetComponent<UnlockWidget>();
			widget.Configurate(unlockId);
			widget.ShowInGame();
			this.ShowUnlock(unlockId, true);
			CanvasGroup gr = widget.GetComponentInChildren<CanvasGroup>();
			gr.alpha = 0f;
			uiroot.SetActive(true);
			DOTween.defaultTimeScaleIndependent = true;
			DOTween.To(() => gr.alpha, delegate(float x)
			{
				gr.alpha = x;
			}, 1f, 1f);
			while (!widget.WantToExit)
			{
				yield return null;
			}
			TweenerCore<float, float, FloatOptions> teen = DOTween.To(() => gr.alpha, delegate(float x)
			{
				gr.alpha = x;
			}, 0f, 1f);
			yield return new WaitForSecondsRealtime(0.5f);
			if (blockPlayer)
			{
				Core.Input.SetBlocker("UNLOCK", false);
				Core.Logic.ResumeGame();
			}
			uiroot.SetActive(false);
			DOTween.defaultTimeScaleIndependent = false;
			yield return null;
			yield break;
		}

		public void ShowFullMessage(UIController.FullMensages message, float totalTime, float fadeInTime, float fadeOutTime)
		{
			base.StartCoroutine(this.ShowFullMessageCourrutine(message, totalTime, fadeInTime, fadeOutTime));
		}

		public IEnumerator ShowFullMessageCourrutine(UIController.FullMensages message, float totalTime, float fadeInTime, float fadeOutTime)
		{
			this.fullMessageCanvas.alpha = 0f;
			foreach (KeyValuePair<UIController.FullMensages, GameObject> keyValuePair in this.fullMessagesConfig)
			{
				keyValuePair.Value.SetActive(keyValuePair.Key == message);
			}
			this.fullMessages.SetActive(true);
			Tweener myTween = ShortcutExtensions46.DOFade(this.fullMessageCanvas, 1f, fadeInTime);
			yield return TweenExtensions.WaitForCompletion(myTween);
			yield return new WaitForSeconds(totalTime);
			if (fadeOutTime >= 0f)
			{
				myTween = ShortcutExtensions46.DOFade(this.fullMessageCanvas, 0f, fadeOutTime);
				yield return TweenExtensions.WaitForCompletion(myTween);
				this.fullMessages.SetActive(false);
			}
			yield break;
		}

		public void PlayBossRushRankAudio(bool complete)
		{
			base.StartCoroutine(this.bossRushRankWidget.PlayBossRushRankAudio(complete));
		}

		public bool BossRushRetryPressed { get; private set; }

		public IEnumerator ShowBossRushRanksAndWait(BossRushHighScore score, bool pauseGame, bool complete, bool unlockHard)
		{
			if (complete)
			{
				this.PlayBossRushRankAudio(true);
				yield return this.ShowFullMessageCourrutine(UIController.FullMensages.EndBossDefeated, 4f, 1f, -1f);
				Core.UI.Fade.Fade(true, 1f, 0f, null);
				yield return new WaitForSecondsRealtime(1f);
				this.fullMessages.SetActive(false);
			}
			this.bossRushRankWidget.ShowHighScore(score, pauseGame, complete, unlockHard);
			while (this.bossRushRankWidget.IsFading)
			{
				yield return 0;
			}
			if (Core.Input.HasBlocker("FADE"))
			{
				Core.UI.Fade.Fade(false, 0.5f, 0f, null);
			}
			while (this.bossRushRankWidget.IsActive())
			{
				yield return 0;
			}
			this.BossRushRetryPressed = this.bossRushRankWidget.RetryPressed;
			yield break;
		}

		public void ShowPopupAchievement(Achievement achievement)
		{
			if (Core.AchievementsManager.ShowPopUp)
			{
				this.popupAchievementWidget.ShowPopup(achievement);
			}
		}

		public void ShowPatchNotes()
		{
			this.patchNotesWidget.Open();
		}

		public bool IsPatchNotesShowing()
		{
			return this.patchNotesWidget.isOpen;
		}

		public void ShowUpgradeFlasksWidget(float price, Action onUpgradeFlask, Action onContinueWithoutUpgrading)
		{
			this.upgradeFlasksWidget.Open(price, onUpgradeFlask, onContinueWithoutUpgrading);
		}

		public void ShowChoosePenitenceWidget(Action onChoosingPenitence, Action onContinueWithoutChoosingPenitence)
		{
			this.choosePenitenceWidget.Open(onChoosingPenitence, onContinueWithoutChoosingPenitence);
		}

		public void ShowAbandonPenitenceWidget(Action onAbandoningPenitence, Action onContinueWithoutAbandoningPenitence)
		{
			this.abandonPenitenceWidget.Open(onAbandoningPenitence, onContinueWithoutAbandoningPenitence);
		}

		public void ShowQuoteWidget(float fadeInTime, float timeActive, float fadeOutTime, Action onFinish)
		{
			this.quoteWidget.Open(fadeInTime, timeActive, fadeOutTime, onFinish);
		}

		public void ShowConfirmationWidget(string infoMessage, Action onAccept, Action onBack)
		{
			this.confirmationWidget.Open(infoMessage, onAccept, onBack);
		}

		public void ShowConfirmationWidget(string infoMessage, string acceptMessage, string dissentMessage, Action onAccept, Action onBack)
		{
			this.confirmationWidget.Open(infoMessage, acceptMessage, dissentMessage, onAccept, onBack);
		}

		public void ShowIntroDemakeWidget(Action onAccept)
		{
			this.introDemakeWidget.Open(onAccept);
		}

		public void HideIntroDemakeWidget()
		{
			this.introDemakeWidget.Close();
		}

		public void ShowModeUnlockedWidget(ModeUnlockedWidget.ModesToUnlock modeUnlocked)
		{
			this.modeUnlockedWidget.Open(modeUnlocked);
		}

		public void HideModeUnlockedWidget()
		{
			this.modeUnlockedWidget.Close();
		}

		public bool IsModeUnlockedShowing()
		{
			return this.modeUnlockedWidget.isOpen;
		}

		public void HideAllNotInGameUI()
		{
			this.HideInventory();
			this.HideBossHealth();
			this.HideMainMenu();
			this.HidePauseMenu();
			foreach (BasicUIBlockingWidget basicUIBlockingWidget in this.allUIBLockingWidgets)
			{
				basicUIBlockingWidget.FadeHide();
			}
		}

		public void ShowBossHealth(Entity entity)
		{
			this.bossHealth.gameObject.SetActive(true);
			this.bossHealth.SetTarget(entity.gameObject);
			string text = string.Empty;
			if (entity.displayName == null)
			{
				text = "NAME NOT SET, PLEASE FIX SCENE";
			}
			else
			{
				text = entity.displayName.ToString();
				if (text == string.Empty)
				{
					text = "[!LOC_" + entity.displayName.mTerm.ToUpper() + "]";
				}
			}
			this.bossHealth.SetName(text);
			this.bossHealth.Show();
		}

		public void HideBossHealth()
		{
			this.bossHealth.Hide();
			this.bossHealth.gameObject.SetActive(false);
		}

		public void ShowMainMenu(string newInitialScene = "")
		{
			this.mainMenuWidget.gameObject.SetActive(true);
			this.gameplayWidget.RestoreDefaultPanelsStatus();
			this.mainMenuWidget.ShowMenu(newInitialScene);
		}

		public IEnumerator ShowMainMenuToChooseBackground()
		{
			yield return new WaitForEndOfFrame();
			this.mainMenuWidget.gameObject.SetActive(true);
			this.mainMenuWidget.ShowChooseBackground();
			yield break;
		}

		public void HideMainMenu()
		{
			this.mainMenuWidget.gameObject.SetActive(false);
		}

		public bool IsShowingMenu
		{
			get
			{
				return this.newInventoryMenu.currentlyActive || this.pauseWidget.IsActive();
			}
		}

		public bool IsShowingInventory
		{
			get
			{
				return this.newInventoryMenu.currentlyActive;
			}
		}

		public GlowWidget GetGlow()
		{
			return this.glowWidget;
		}

		public void ShowGlow(Color haloColor, float haloDuration)
		{
			this.glowWidget.color = haloColor;
			this.glowWidget.Show(haloDuration, 1);
		}

		public DialogWidget GetDialog()
		{
			return this.dialogWidget;
		}

		public void HidePauseMenu()
		{
			if (this.pauseWidget.IsActive())
			{
				this.pauseWidget.FadeHide();
			}
			this.HideKneelMenu();
		}

		public void ShowLoad(bool show, Color? background = null)
		{
			Color color = (background == null) ? Color.black : background.Value;
			if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))
			{
				this.loadWidgetDemake.gameObject.GetComponent<Image>().color = color;
				this.loadWidgetDemake.gameObject.SetActive(show);
				this.loadWidget.gameObject.SetActive(false);
			}
			else
			{
				this.loadWidget.gameObject.GetComponent<Image>().color = color;
				this.loadWidget.gameObject.SetActive(show);
				this.loadWidgetDemake.gameObject.SetActive(false);
			}
			if (show && this.fadeWidget.IsActive)
			{
				this.fadeWidget.SetOnColor(color);
				this.fadeWidget.Fade(false, 0.2f, 0f, delegate
				{
					this.fadeWidget.ResetToBlack();
				});
			}
		}

		public bool Paused
		{
			get
			{
				return Math.Abs(Core.Logic.CurrentLevelConfig.TimeScale) < Mathf.Epsilon;
			}
		}

		public void UpdatePurgePoints()
		{
			this.gameplayWidget.UpdatePurgePoints();
		}

		public void UpdateGuiltLevel(bool whenDead)
		{
			this.gameplayWidget.UpdateGuiltLevel(whenDead);
		}

		public void ShowPurgePoints()
		{
			this.gameplayWidget.ShowPurgePoints();
		}

		public void HidePurgePoints()
		{
			this.gameplayWidget.HidePurgePoints();
		}

		public void ShowBossRushTimer()
		{
			this.gameplayWidget.ShowBossRushTimer();
		}

		public void HideBossRushTimer()
		{
			this.gameplayWidget.HideBossRushTimer();
		}

		public void NotEnoughFervour()
		{
			this.gameplayWidget.NotEnoughFervour();
		}

		public void StartMiriamTimer()
		{
			this.gameplayWidget.StartMiriamTimer();
		}

		public void StopMiriamTimer()
		{
			this.gameplayWidget.StopMiriamTimer();
		}

		public void SetMiriamTimerTargetTime(float targetTime)
		{
			this.gameplayWidget.SetMiriamTimerTargetTime(targetTime);
		}

		public void ShowMiriamTimer()
		{
			this.gameplayWidget.ShowMiriamTimer();
		}

		public void HideMiriamTimer()
		{
			this.gameplayWidget.HideMiriamTimer();
		}

		public IEnumerator ShowOptions()
		{
			this.pauseWidget.InitialWidget = PauseWidget.ChildWidgets.OPTIONS;
			this.pauseWidget.FadeShow(false, true, true);
			while (this.pauseWidget.IsActive())
			{
				yield return null;
			}
			yield break;
		}

		public IEnumerator ShowMapTeleport()
		{
			this.pauseWidget.InitialWidget = PauseWidget.ChildWidgets.MAP;
			this.pauseWidget.InitialMapMode = PauseWidget.MapModes.TELEPORT;
			this.pauseWidget.FadeShow(false, true, true);
			while (this.pauseWidget.IsActive())
			{
				yield return null;
			}
			yield break;
		}

		public void ShowExtras()
		{
			this.EnableCanvas(this.extrasMenu.gameObject, true);
			this.extrasMenu.ShowExtras();
		}

		public SubtitleWidget GetSubtitleWidget()
		{
			return this.subtitleWidget;
		}

		public OptionsWidget GetOptionsWidget()
		{
			return this.pauseWidget.OptionsWidget;
		}

		public OptionsWidget.SCALING_STRATEGY GetScalingStrategy()
		{
			return this.pauseWidget.GetScalingStrategy();
		}

		public void ShowGameplayLeftPart()
		{
			this.gameplayWidget.ShowLeftPart();
		}

		public void HideGameplayLeftPart()
		{
			this.gameplayWidget.HideLeftPart();
		}

		public void ShowGameplayRightPart()
		{
			this.gameplayWidget.ShowRightPart();
		}

		public void HideGameplayRightPart()
		{
			this.gameplayWidget.HideRightPart();
		}

		private bool CanOpenBlockWidget()
		{
			bool flag = !this.newInventoryMenu.currentlyActive && !this.mainMenuWidget.currentlyActive;
			if (flag)
			{
				foreach (BasicUIBlockingWidget basicUIBlockingWidget in this.allUIBLockingWidgets)
				{
					flag = (flag && !basicUIBlockingWidget.IsActive());
				}
			}
			bool flag2 = string.Equals(Core.LevelManager.currentLevel.LevelName, "d07z01s03", StringComparison.CurrentCultureIgnoreCase);
			bool flag3 = Core.Input.HasBlocker("INVENTORY");
			bool insideChangeLevel = Core.LevelManager.InsideChangeLevel;
			return flag && !flag2 && !flag3 && !insideChangeLevel;
		}

		private IEnumerator SafeGoBackInventory()
		{
			yield return new WaitForEndOfFrame();
			this.newInventoryMenu.GoBack();
			if (!this.newInventoryMenu.currentlyActive)
			{
				EventSystem.current.SetSelectedGameObject(null);
				this.EnableCanvas(this.newInventoryMenu.gameObject, false);
				this.CheckPrayerTutorial();
			}
			yield break;
		}

		private IEnumerator SafeGoBackExtras()
		{
			yield return new WaitForEndOfFrame();
			this.extrasMenu.GoBack();
			yield break;
		}

		private void EnableCanvas(GameObject widget, bool enabled)
		{
			CanvasGroup component = widget.GetComponent<CanvasGroup>();
			component.interactable = enabled;
			component.blocksRaycasts = enabled;
		}

		private void CheckPrayerTutorial()
		{
			if (Core.InventoryManager.GetPrayerInSlot(0) != null && !Core.TutorialManager.IsTutorialUnlocked(this.TutorialPrayer))
			{
				base.StartCoroutine(Core.TutorialManager.ShowTutorial(this.TutorialPrayer, true));
			}
		}

		public static UIController instance;

		[SerializeField]
		private DeadScreenWidget deadScreen;

		[SerializeField]
		public Image fade;

		[SerializeField]
		private ExtrasMenuWidget extrasMenu;

		[SerializeField]
		private NewInventoryWidget newInventoryMenu;

		[SerializeField]
		private PopUpWidget popUpWidget;

		[SerializeField]
		private BossHealth bossHealth;

		[SerializeField]
		private GameplayWidget gameplayWidget;

		[SerializeField]
		private FadeWidget fadeWidget;

		[SerializeField]
		private GlowWidget glowWidget;

		[SerializeField]
		private DialogWidget dialogWidget;

		[SerializeField]
		private GameObject loadWidget;

		[SerializeField]
		private GameObject loadWidgetDemake;

		[SerializeField]
		private NewMainMenu mainMenuWidget;

		[SerializeField]
		private GameObject content;

		[SerializeField]
		private SubtitleWidget subtitleWidget;

		[SerializeField]
		private GameObject tutorialWidget;

		[SerializeField]
		private GameObject unlockWidget;

		[SerializeField]
		private GameObject fullMessages;

		[SerializeField]
		private KneelPopUpWidget kneelMenuWidget;

		[SerializeField]
		private PauseWidget pauseWidget;

		[SerializeField]
		private PatchNotesWidget patchNotesWidget;

		[SerializeField]
		private UpgradeFlasksWidget upgradeFlasksWidget;

		[SerializeField]
		private ChoosePenitenceWidget choosePenitenceWidget;

		[SerializeField]
		private QuoteWidget quoteWidget;

		[SerializeField]
		private ConfirmationWidget confirmationWidget;

		[SerializeField]
		private AbandonPenitenceWidget abandonPenitenceWidget;

		[SerializeField]
		private AlmsWidget almsWidget;

		[SerializeField]
		private TeleportWidget teleportWidget;

		[SerializeField]
		private BossRushRankWidget bossRushRankWidget;

		[SerializeField]
		private PopupAchievementWidget popupAchievementWidget;

		[SerializeField]
		private IntroDemakeWidget introDemakeWidget;

		[SerializeField]
		private ModeUnlockedWidget modeUnlockedWidget;

		private List<BasicUIBlockingWidget> allUIBLockingWidgets = new List<BasicUIBlockingWidget>();

		public List<FontsByLanguage> fontsByLanguage;

		private bool _canEquipSwordHearts;

		public bool CanOpenInventory = true;

		[SerializeField]
		private Dictionary<UIController.FullMensages, GameObject> fullMessagesConfig = new Dictionary<UIController.FullMensages, GameObject>();

		[SerializeField]
		private float waitTimeToShowZone = 1f;

		[TutorialId]
		[SerializeField]
		[BoxGroup("Tutorial", true, false, 0)]
		private string TutorialPrayer;

		private bool paused;

		private Player rewired;

		private CanvasGroup fullMessageCanvas;

		[SerializeField]
		[BoxGroup("Sound", true, false, 0)]
		private string sfxOpenOptions = "event:/SFX/UI/ChangeTab";

		private bool firstRun = true;

		public enum FullMensages
		{
			BossDefeated,
			ConfessorArea,
			EndBossDefeated
		}

		public enum PopupItemAction
		{
			GetObejct,
			GiveObject
		}
	}
}
