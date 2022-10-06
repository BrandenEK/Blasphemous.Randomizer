using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.UI.Widgets;
using Rewired;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class IntroDemakeWidget : BaseMenuScreen
	{
		public void Open(Action onAccept)
		{
			this.onAccept = onAccept;
			this.Open();
		}

		public override void Open()
		{
			base.Open();
			if (this.rewiredPlayer == null)
			{
				this.rewiredPlayer = ReInput.players.GetPlayer(0);
			}
			if (this.canvasGroup == null)
			{
				this.canvasGroup = base.GetComponent<CanvasGroup>();
			}
			if (this.animator == null)
			{
				this.animator = base.GetComponent<Animator>();
			}
			this.accepted = false;
			this.isClosing = false;
			Core.Input.SetBlocker("UIBLOCKING_CONFIRMATION", true);
			base.gameObject.SetActive(true);
			FadeWidget.instance.StartEasyFade(Color.black, new Color(0f, 0f, 0f, 0f), 0.2f, false);
			this.canvasGroup.alpha = 1f;
			this.OnOpen();
		}

		protected override void OnOpen()
		{
			this.isOpen = true;
			this.animator.SetTrigger("INTRO");
		}

		public override void Close()
		{
			this.isOpen = false;
			base.Close();
			this.OnClose();
		}

		protected override void OnClose()
		{
			Core.Input.SetBlocker("UIBLOCKING_CONFIRMATION", false);
			base.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (this.rewiredPlayer == null || !this.isOpen || this.isClosing)
			{
				return;
			}
			bool buttonDown = this.rewiredPlayer.GetButtonDown(50);
			if (buttonDown)
			{
				this.accepted = true;
				this.isClosing = true;
				this.onAccept();
				Core.Audio.PlaySfx(this.soundOnAccept, 0f);
				Core.Audio.StopNamedSound("DEMAKE_INTRO", 0);
				this.animator.SetTrigger("PRESS_START");
			}
		}

		[SerializeField]
		[EventRef]
		private string soundOnAccept = "event:/SFX/UI/ChangeTab";

		private const string BLOCKER_NAME = "UIBLOCKING_CONFIRMATION";

		private CanvasGroup canvasGroup;

		private Action onAccept;

		private Player rewiredPlayer;

		private Animator animator;

		private bool isOpen;

		private bool isClosing;

		private bool accepted;
	}
}
