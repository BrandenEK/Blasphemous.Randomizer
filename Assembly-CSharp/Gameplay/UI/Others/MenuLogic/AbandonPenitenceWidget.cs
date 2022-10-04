using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FMODUnity;
using Framework.Managers;
using Framework.Penitences;
using I2.Loc;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class AbandonPenitenceWidget : BaseMenuScreen
	{
		public void Open(Action onAbandoningPenitence, Action onContinueWithoutAbandoningPenitence)
		{
			this.onAbandoningPenitence = onAbandoningPenitence;
			this.onContinueWithoutAbandoningPenitence = onContinueWithoutAbandoningPenitence;
			this.Open();
		}

		public override void Open()
		{
			base.Open();
			Core.Input.SetBlocker("UIBLOCKING_PENITENCE", true);
			base.gameObject.SetActive(true);
			this.rewiredPlayer = ReInput.players.GetPlayer(0);
			this.UpdatePenitenceTextsAndDisplayedMedal();
			this.canvasGroup = base.GetComponent<CanvasGroup>();
			DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 1f, 1f).OnComplete(new TweenCallback(this.OnOpen));
		}

		protected override void OnOpen()
		{
			this.isOpen = true;
		}

		public override void Close()
		{
			this.isOpen = false;
			DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 0f, 1f).OnComplete(new TweenCallback(this.OnClose));
			base.Close();
		}

		protected override void OnClose()
		{
			Core.Input.SetBlocker("UIBLOCKING_PENITENCE", false);
			base.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (this.rewiredPlayer == null || !this.isOpen || this.isConfirmationPopupOpen)
			{
				return;
			}
			bool buttonDown = this.rewiredPlayer.GetButtonDown(51);
			if (buttonDown)
			{
				if (this.onContinueWithoutAbandoningPenitence != null)
				{
					this.onContinueWithoutAbandoningPenitence();
				}
				if (this.soundOnExiting != string.Empty)
				{
					Core.Audio.PlayOneShot(this.soundOnExiting, default(Vector3));
				}
				this.Close();
				return;
			}
			bool buttonDown2 = this.rewiredPlayer.GetButtonDown(50);
			if (buttonDown2)
			{
				this.ChooseToAbandonPenitence();
			}
		}

		private void UpdatePenitenceTextsAndDisplayedMedal()
		{
			IPenitence currentPenitence = Core.PenitenceManager.GetCurrentPenitence();
			if (currentPenitence is PenitencePE01)
			{
				this.penitenceTitle.text = ScriptLocalization.UI_Penitences.PE01_NAME;
				this.penitenceInfoText.text = ScriptLocalization.UI_Penitences.PE01_INFO;
				this.PE01Medal.SetActive(true);
				this.PE02Medal.SetActive(false);
				this.PE03Medal.SetActive(false);
			}
			else if (currentPenitence is PenitencePE02)
			{
				this.penitenceTitle.text = ScriptLocalization.UI_Penitences.PE02_NAME;
				this.penitenceInfoText.text = ScriptLocalization.UI_Penitences.PE02_INFO;
				this.PE01Medal.SetActive(false);
				this.PE02Medal.SetActive(true);
				this.PE03Medal.SetActive(false);
			}
			else if (currentPenitence is PenitencePE03)
			{
				this.penitenceTitle.text = ScriptLocalization.UI_Penitences.PE03_NAME;
				this.penitenceInfoText.text = ScriptLocalization.UI_Penitences.PE03_INFO;
				this.PE01Medal.SetActive(false);
				this.PE02Medal.SetActive(false);
				this.PE03Medal.SetActive(true);
			}
			else
			{
				Debug.LogError("AbandonPenitenceWidget::UpdatePenitenceTexts: Current Penitence is not one of the first three!");
			}
		}

		private void ChooseToAbandonPenitence()
		{
			if (this.soundOnAbandoning != string.Empty)
			{
				Core.Audio.PlayOneShot(this.soundOnAbandoning, default(Vector3));
			}
			UIController.instance.ShowConfirmationWidget(ScriptLocalization.UI_Penitences.CHOOSE_PENITENCE_ABANDON, new Action(this.AbandonPenitence), new Action(this.ContinueAfterClosingConfirmationPopup));
			this.isConfirmationPopupOpen = true;
		}

		private void AbandonPenitence()
		{
			this.ContinueAfterClosingConfirmationPopup();
			Core.PenitenceManager.MarkCurrentPenitenceAsAbandoned();
			if (this.onAbandoningPenitence != null)
			{
				this.onAbandoningPenitence();
			}
			this.Close();
		}

		private void ContinueAfterClosingConfirmationPopup()
		{
			this.isConfirmationPopupOpen = false;
		}

		[SerializeField]
		private Text penitenceTitle;

		[SerializeField]
		private Text penitenceInfoText;

		[SerializeField]
		private GameObject PE01Medal;

		[SerializeField]
		private GameObject PE02Medal;

		[SerializeField]
		private GameObject PE03Medal;

		[SerializeField]
		[EventRef]
		private string soundOnAbandoning = "event:/SFX/UI/ChangeTab";

		[SerializeField]
		[EventRef]
		private string soundOnExiting = "event:/SFX/UI/ChangeTab";

		private const string BLOCKER_NAME = "UIBLOCKING_PENITENCE";

		private CanvasGroup canvasGroup;

		private Action onAbandoningPenitence;

		private Action onContinueWithoutAbandoningPenitence;

		private Player rewiredPlayer;

		private bool isOpen;

		private bool isConfirmationPopupOpen;
	}
}
