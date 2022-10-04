using System;
using System.Collections;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.UI.Widgets;
using Rewired;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.UI.Others.MenuLogic
{
	public class MainMenu : MonoBehaviour
	{
		private void Awake()
		{
			MainMenu.instance = this;
			this.animator = base.GetComponent<Animator>();
			this.keepFocus = base.GetComponent<KeepFocus>();
			this.SetState(MainMenu.MenuState.IDLE);
		}

		private void Update()
		{
			switch (this.currentState)
			{
			case MainMenu.MenuState.DISCLAIMER:
				this.timeWaiting += Time.deltaTime;
				if (this.timeWaiting > this.disclaimerWait)
				{
					this.SetState(MainMenu.MenuState.DISCLAIMER_TEXT);
				}
				break;
			case MainMenu.MenuState.DISCLAIMER_TEXT:
			{
				Player player = ReInput.players.GetPlayer(0);
				bool flag = player.GetButtonDown(8) || player.GetButtonDown(5) || player.GetButtonDown(7) || player.GetButtonDown(6) || Input.anyKey;
				if (flag)
				{
					this.timeWaiting = 0f;
					this.SetState(MainMenu.MenuState.FADEOUT);
				}
				break;
			}
			case MainMenu.MenuState.FADEOUT:
				this.timeWaiting += Time.deltaTime;
				if (this.timeWaiting > this.fadingWait)
				{
					this.InternalPlay();
				}
				break;
			}
		}

		private void Start()
		{
			this.SetState(MainMenu.MenuState.IDLE);
			Core.Logic.CameraManager.ProCamera2D.gameObject.SetActive(false);
		}

		public void Continue()
		{
			if (this.currentState != MainMenu.MenuState.IDLE)
			{
				return;
			}
			this.keepFocus.enabled = false;
			EventSystem.current.SetSelectedGameObject(null);
			Log.Trace("Continue pressed, starting the game...", null);
			this.isContinue = Core.Persistence.ExistSlot(PersistentManager.GetAutomaticSlot());
			this.SetState(MainMenu.MenuState.DISCLAIMER);
		}

		public void Play()
		{
			if (this.currentState != MainMenu.MenuState.IDLE)
			{
				return;
			}
			this.keepFocus.enabled = false;
			this.isContinue = false;
			EventSystem.current.SetSelectedGameObject(null);
			Log.Trace("Play pressed, starting the game...", null);
			this.SetState(MainMenu.MenuState.DISCLAIMER);
		}

		public void OpenImportMenu()
		{
			if (this.currentState != MainMenu.MenuState.IDLE)
			{
				return;
			}
			this.importMenu.SetActive(true);
			base.gameObject.SetActive(false);
			PlayerPrefs.SetInt("SOULS_IMPORTED", 1);
		}

		public void CloseImportMenu()
		{
			if (this.currentState != MainMenu.MenuState.IDLE)
			{
				return;
			}
			this.importMenu.SetActive(false);
			base.gameObject.SetActive(true);
		}

		public void ExitSucessMenu()
		{
			if (this.currentState != MainMenu.MenuState.IDLE)
			{
				return;
			}
			this.successMenu.SetActive(false);
			base.gameObject.SetActive(true);
		}

		public void ChangeLanguage()
		{
			if (this.currentState != MainMenu.MenuState.IDLE)
			{
				return;
			}
			Core.Localization.SetNextLanguage();
		}

		public void ExitGame()
		{
			if (this.currentState != MainMenu.MenuState.IDLE)
			{
				return;
			}
			if (!FadeWidget.instance.Fading)
			{
				Process.GetCurrentProcess().Kill();
			}
		}

		private IEnumerator LoadFirstScene()
		{
			FadeWidget.instance.Fade(true, 1.5f, 0f, null);
			yield return new WaitForSeconds(0.3f);
			Core.Logic.ResetAllData();
			if (this.isContinue)
			{
			}
			yield break;
		}

		private void OnImportFinished()
		{
			this.importMenu.SetActive(false);
			this.successMenu.SetActive(true);
		}

		private void InternalPlay()
		{
			base.StartCoroutine(this.LoadFirstScene());
		}

		private void SetState(MainMenu.MenuState state)
		{
			this.currentState = state;
			this.animator.SetInteger("STATE", (int)this.currentState);
		}

		private bool axisAvailable;

		public static MainMenu instance;

		public GameObject importMenu;

		public GameObject successMenu;

		public string initialScene;

		public float disclaimerWait = 5f;

		public float fadingWait = 0.5f;

		private const string ANIMATOR_STATE = "STATE";

		private MainMenu.MenuState currentState;

		private Animator animator;

		private KeepFocus keepFocus;

		private float timeWaiting;

		private bool isContinue;

		private enum MenuState
		{
			IDLE,
			DISCLAIMER,
			DISCLAIMER_TEXT,
			FADEOUT
		}
	}
}
