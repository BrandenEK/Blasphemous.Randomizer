using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.Managers;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class ChoosePenitenceWidget : BaseMenuScreen
	{
		public void Open(Action onChoosingPenitence, Action onContinueWithoutChoosingPenitence)
		{
			this.onChoosingPenitence = onChoosingPenitence;
			this.onContinueWithoutChoosingPenitence = onContinueWithoutChoosingPenitence;
			this.Open();
		}

		public override void Open()
		{
			base.Open();
			Core.Input.SetBlocker("UIBLOCKING_PENITENCE", true);
			base.gameObject.SetActive(true);
			this.canvasGroup = base.GetComponent<CanvasGroup>();
			DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 1f, 1f);
		}

		public override void Close()
		{
			TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 0f, 1f), new TweenCallback(this.OnClose));
			base.Close();
		}

		protected override void OnClose()
		{
			Core.Input.SetBlocker("UIBLOCKING_PENITENCE", false);
			base.gameObject.SetActive(false);
			this.ResetButtonsNavigation();
		}

		public void Option_SelectPE01()
		{
			this.penitenceTitle.text = ScriptLocalization.UI_Penitences.PE01_NAME;
			this.penitenceInfoText.text = ScriptLocalization.UI_Penitences.PE01_INFO;
			this.penitenceScroll.NewContentSetted();
		}

		public void Option_SelectPE02()
		{
			this.penitenceTitle.text = ScriptLocalization.UI_Penitences.PE02_NAME;
			this.penitenceInfoText.text = ScriptLocalization.UI_Penitences.PE02_INFO;
			this.penitenceScroll.NewContentSetted();
		}

		public void Option_SelectPE03()
		{
			this.penitenceTitle.text = ScriptLocalization.UI_Penitences.PE03_NAME;
			this.penitenceInfoText.text = ScriptLocalization.UI_Penitences.PE03_INFO;
			this.penitenceScroll.NewContentSetted();
		}

		public void Option_SelectNoPenitence()
		{
			this.penitenceTitle.text = ScriptLocalization.UI_Penitences.NO_PENITENCE;
			this.penitenceInfoText.text = ScriptLocalization.UI_Penitences.NO_PENITENCE_INFO;
			this.penitenceScroll.NewContentSetted();
		}

		public void Option_ActivatePE01()
		{
			this.SetButtonsNavigationMode(this.buttons, 0);
			UIController.instance.ShowConfirmationWidget(ScriptLocalization.UI_Penitences.CHOOSE_PENITENCE_CONFIRMATION, new Action(this.ActivatePenitencePE01AndClose), new Action(this.ResetButtonsNavigation));
		}

		public void Option_ActivatePE02()
		{
			this.SetButtonsNavigationMode(this.buttons, 0);
			UIController.instance.ShowConfirmationWidget(ScriptLocalization.UI_Penitences.CHOOSE_PENITENCE_CONFIRMATION, new Action(this.ActivatePenitencePE02AndClose), new Action(this.ResetButtonsNavigation));
		}

		public void Option_ActivatePE03()
		{
			this.SetButtonsNavigationMode(this.buttons, 0);
			UIController.instance.ShowConfirmationWidget(ScriptLocalization.UI_Penitences.CHOOSE_PENITENCE_CONFIRMATION, new Action(this.ActivatePenitencePE03AndClose), new Action(this.ResetButtonsNavigation));
		}

		public void Option_ContinueWithNoPenitence()
		{
			this.SetButtonsNavigationMode(this.buttons, 0);
			UIController.instance.ShowConfirmationWidget(ScriptLocalization.UI_Penitences.CHOOSE_NO_PENITENCE_CONFIRMATION, new Action(this.ContinueWithNoPenitenceAndClose), new Action(this.ResetButtonsNavigation));
		}

		private void ActivatePenitencePE01AndClose()
		{
			Core.PenitenceManager.ActivatePE01();
			this.onChoosingPenitence();
			this.Close();
		}

		private void ActivatePenitencePE02AndClose()
		{
			Core.PenitenceManager.ActivatePE02();
			this.onChoosingPenitence();
			this.Close();
		}

		private void ActivatePenitencePE03AndClose()
		{
			Core.PenitenceManager.ActivatePE03();
			this.onChoosingPenitence();
			this.Close();
		}

		private void ContinueWithNoPenitenceAndClose()
		{
			this.onContinueWithoutChoosingPenitence();
			this.Close();
		}

		private void ResetButtonsNavigation()
		{
			this.SetButtonsNavigationMode(this.buttons, 4);
		}

		private void SetButtonsNavigationMode(List<Button> buttons, Navigation.Mode mode)
		{
			foreach (Button button in buttons)
			{
				Navigation navigation = button.navigation;
				navigation.mode = mode;
				button.navigation = navigation;
			}
		}

		[SerializeField]
		private Text penitenceTitle;

		[SerializeField]
		private Text penitenceInfoText;

		[SerializeField]
		private CustomScrollView penitenceScroll;

		[SerializeField]
		private List<Button> buttons;

		private const string BLOCKER_NAME = "UIBLOCKING_PENITENCE";

		private CanvasGroup canvasGroup;

		private Action onChoosingPenitence;

		private Action onContinueWithoutChoosingPenitence;
	}
}
