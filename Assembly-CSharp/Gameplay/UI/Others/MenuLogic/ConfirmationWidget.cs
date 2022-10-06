using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FMODUnity;
using Framework.Managers;
using I2.Loc;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class ConfirmationWidget : BaseMenuScreen
	{
		public void Open(string infoMessage, Action onAccept, Action onBack)
		{
			if (this.isClosing)
			{
				return;
			}
			string label_BUTTON_ACCEPT = ScriptLocalization.UI_Map.LABEL_BUTTON_ACCEPT;
			string label_BUTTON_BACK = ScriptLocalization.UI_Map.LABEL_BUTTON_BACK;
			this.Open(infoMessage, label_BUTTON_ACCEPT, label_BUTTON_BACK, onAccept, onBack);
		}

		public void Open(string infoMessage, string acceptMessage, string backMessage, Action onAccept, Action onBack)
		{
			if (this.isClosing)
			{
				return;
			}
			this.onAccept = onAccept;
			this.onBack = onBack;
			this.infoText.text = infoMessage;
			this.acceptText.text = acceptMessage;
			this.backText.text = backMessage;
			this.Open();
		}

		public override void Open()
		{
			if (this.isClosing)
			{
				return;
			}
			base.Open();
			this.accepted = false;
			this.rewiredPlayer = ReInput.players.GetPlayer(0);
			Core.Input.SetBlocker("UIBLOCKING_CONFIRMATION", true);
			base.gameObject.SetActive(true);
			this.canvasGroup = base.GetComponent<CanvasGroup>();
			this.canvasGroup.alpha = 0f;
			TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 1f, 1f), new TweenCallback(this.OnOpen));
		}

		protected override void OnOpen()
		{
			this.isOpen = true;
		}

		public override void Close()
		{
			this.isOpen = false;
			this.isClosing = true;
			if (this.accepted && this.onAccept != null)
			{
				this.onAccept();
			}
			TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 0f, 1f), new TweenCallback(this.OnClose));
			base.Close();
		}

		protected override void OnClose()
		{
			this.isClosing = false;
			Core.Input.SetBlocker("UIBLOCKING_CONFIRMATION", false);
			if (!this.accepted && this.onBack != null)
			{
				this.onBack();
			}
			base.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (this.rewiredPlayer == null || !this.isOpen)
			{
				return;
			}
			bool buttonDown = this.rewiredPlayer.GetButtonDown(51);
			if (buttonDown)
			{
				this.accepted = false;
				if (this.soundOnAccept != string.Empty)
				{
					Core.Audio.PlayOneShot(this.soundOnAccept, default(Vector3));
				}
				this.Close();
				return;
			}
			bool buttonDown2 = this.rewiredPlayer.GetButtonDown(50);
			if (buttonDown2)
			{
				this.accepted = true;
				if (this.soundOnBack != string.Empty)
				{
					Core.Audio.PlayOneShot(this.soundOnBack, default(Vector3));
				}
				this.Close();
			}
		}

		[SerializeField]
		private Text infoText;

		[SerializeField]
		private Text acceptText;

		[SerializeField]
		private Text backText;

		[SerializeField]
		[EventRef]
		private string soundOnAccept = "event:/SFX/UI/ChangeTab";

		[SerializeField]
		[EventRef]
		private string soundOnBack = "event:/SFX/UI/ChangeTab";

		private const string BLOCKER_NAME = "UIBLOCKING_CONFIRMATION";

		private CanvasGroup canvasGroup;

		private Action onAccept;

		private Action onBack;

		private Player rewiredPlayer;

		private bool isOpen;

		private bool isClosing;

		private bool accepted;
	}
}
