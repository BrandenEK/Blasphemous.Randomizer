using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.Managers;
using Gameplay.UI.Widgets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.UI.Others.MenuLogic
{
	[RequireComponent(typeof(CanvasGroup))]
	public class BasicUIBlockingWidget : MonoBehaviour
	{
		public void InitializeWidget()
		{
			this.IsFading = false;
			this.GetCanvas().alpha = 0f;
			this.Deactivate();
			this.OnWidgetInitialize();
		}

		public bool IsFading { get; private set; }

		public void FadeShow(bool checkInput, bool pauseGame = true, bool checkMainFade = true)
		{
			if (this.IsActive() || this.IsFading)
			{
				return;
			}
			if (pauseGame && SceneManager.GetActiveScene().name == "MainMenu")
			{
				return;
			}
			if (checkMainFade && FadeWidget.instance.Fading)
			{
				return;
			}
			if (checkInput && Core.Input.InputBlocked && !Core.Input.HasBlocker("PLAYER_LOGIC"))
			{
				return;
			}
			this.IsFading = true;
			this.PauseGame = pauseGame;
			CanvasGroup canvas = this.GetCanvas();
			canvas.alpha = 0f;
			this.Activate();
			if (this.PauseGame)
			{
				Core.Input.SetBlocker("UIBLOCKING", true);
				Core.Logic.SetState(LogicStates.Unresponsive);
				Core.Logic.PauseGame();
				DOTween.defaultTimeScaleIndependent = true;
			}
			TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(DOTween.To(() => canvas.alpha, delegate(float x)
			{
				canvas.alpha = x;
			}, 1f, this.alphaDurantion), new TweenCallback(this.EndFadeShow));
			this.OnWidgetShow();
		}

		public void FadeHide()
		{
			if (!this.IsActive() || this.IsFading)
			{
				return;
			}
			this.IsFading = true;
			CanvasGroup canvas = this.GetCanvas();
			canvas.alpha = 1f;
			if (this.PauseGame)
			{
				Core.Logic.ResumeGame();
				Core.Logic.SetState(LogicStates.Playing);
			}
			DOTween.defaultTimeScaleIndependent = true;
			TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(DOTween.To(() => canvas.alpha, delegate(float x)
			{
				canvas.alpha = x;
			}, 0f, this.alphaDurantion), new TweenCallback(this.EndFadeHide));
		}

		public void Hide()
		{
			if (!this.IsActive() || this.IsFading)
			{
				return;
			}
			CanvasGroup canvas = this.GetCanvas();
			canvas.alpha = 0f;
			if (this.PauseGame)
			{
				Core.Logic.ResumeGame();
				Core.Logic.SetState(LogicStates.Playing);
			}
			this.EndFadeHide();
		}

		protected virtual void OnWidgetInitialize()
		{
		}

		protected virtual void OnWidgetShow()
		{
		}

		protected virtual void OnWidgetHide()
		{
		}

		public virtual bool GoBack()
		{
			return false;
		}

		public virtual bool AutomaticBack()
		{
			return true;
		}

		public virtual bool IsActive()
		{
			return base.gameObject.activeSelf;
		}

		public virtual void Activate()
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}

		public virtual void Deactivate()
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		private void EndFadeHide()
		{
			this.OnWidgetHide();
			this.IsFading = false;
			this.Deactivate();
			if (this.PauseGame)
			{
				Core.Input.SetBlocker("UIBLOCKING", false);
			}
		}

		private void EndFadeShow()
		{
			this.IsFading = false;
		}

		private CanvasGroup GetCanvas()
		{
			return base.GetComponent<CanvasGroup>();
		}

		private const string BlockerName = "UIBLOCKING";

		private bool PauseGame = true;

		private float alphaDurantion = 0.4f;
	}
}
