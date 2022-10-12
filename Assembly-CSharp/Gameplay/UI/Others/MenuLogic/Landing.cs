using System;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.UI.Others.MenuLogic
{
	public class Landing : MonoBehaviour
	{
		private void Awake()
		{
			if (Application.runInBackground)
			{
				Debug.LogWarning("Run in background was true in landing! Correcting.");
			}
			Application.runInBackground = false;
			this.currentState = Landing.MenuState.FADEIN;
			this.timeWaiting = 0f;
			Time.maximumDeltaTime = 0.033f;
			Settings instance = Settings.Instance;
			if (instance.AutomaticEventLoading || instance.ImportType != ImportType.StreamingAssets)
			{
				Debug.LogError("*** FMODAudioManager, setting must be AutomaticEventLoading=false and ImportType=StreamingAssets");
			}
			else
			{
				try
				{
					foreach (string text in instance.MasterBanks)
					{
						RuntimeManager.LoadBank(text + ".strings", instance.AutomaticSampleLoading);
						RuntimeManager.LoadBank(text, instance.AutomaticSampleLoading);
					}
					foreach (string text2 in instance.Banks)
					{
						if (!text2.ToUpper().StartsWith("VOICEOVER_"))
						{
							RuntimeManager.LoadBank(text2, instance.AutomaticSampleLoading);
						}
					}
					RuntimeManager.WaitForAllLoads();
				}
				catch (BankLoadException exception)
				{
					Debug.LogException(exception);
				}
			}
			Cursor.visible = false;
		}

		private void Start()
		{
			string sceneName = "MainMenu_MAIN";
			this.preloadMenu = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
			this.preloadMenu.allowSceneActivation = false;
			if (!Core.preinit && this.preInitCore)
			{
				Singleton<Core>.Instance.PreInit();
				Time.timeScale = 1f;
			}
		}

		private void Update()
		{
			Landing.MenuState menuState = this.currentState;
			if (menuState != Landing.MenuState.FADEIN)
			{
				if (menuState != Landing.MenuState.PRESS)
				{
					if (menuState == Landing.MenuState.FADEOUT)
					{
						this.timeWaiting += Time.deltaTime;
						if (this.timeWaiting > this.waitToFadeout)
						{
							this.currentState = Landing.MenuState.NOTHING;
							this.preloadMenu.allowSceneActivation = true;
						}
					}
				}
				else
				{
					bool flag = Input.anyKey || ReInput.players.GetPlayer(0).GetAnyButtonDown();
					if (flag)
					{
						this.landingAnimator.SetTrigger("FADEOUT");
						RuntimeManager.PlayOneShot(this.soundPressKey, default(Vector3));
						this.timeWaiting = 0f;
						this.currentState = Landing.MenuState.FADEOUT;
					}
				}
			}
			else
			{
				this.timeWaiting += Time.deltaTime;
				if (this.timeWaiting >= this.waitToPress)
				{
					this.currentState = Landing.MenuState.PRESS;
				}
			}
		}

		public bool preInitCore;

		public float waitToPress = 1f;

		public float waitToFadeout = 1f;

		[EventRef]
		public string soundPressKey = "event:/Key Event/RelicCollected";

		private const string ANIMATOR_FADEOUT = "FADEOUT";

		private AsyncOperation preloadMenu;

		private Landing.MenuState currentState = Landing.MenuState.NOTHING;

		public Animator landingAnimator;

		private float timeWaiting;

		private enum MenuState
		{
			FADEIN,
			PRESS,
			FADEOUT,
			NOTHING
		}
	}
}
