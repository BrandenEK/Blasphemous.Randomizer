using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class PauseWidget : BasicUIBlockingWidget
	{
		public PauseWidget.ChildWidgets CurrentWidget { get; private set; }

		public PauseWidget.ChildWidgets InitialWidget { get; set; }

		public PauseWidget.MapModes InitialMapMode { get; set; }

		protected override void OnWidgetInitialize()
		{
			this.InitialWidget = PauseWidget.ChildWidgets.MAP;
			this.InitialMapMode = PauseWidget.MapModes.SHOW;
		}

		public void ReadOptionConfigurations()
		{
			this.OptionsWidget.Initialize();
		}

		protected override void OnWidgetShow()
		{
			this.canvasMap = this.MapWidget.GetComponent<CanvasGroup>();
			this.canvasOptions = this.OptionsWidget.GetComponent<CanvasGroup>();
			this.SetChildActive(this.canvasMap, this.InitialWidget == PauseWidget.ChildWidgets.MAP);
			this.SetChildActive(this.canvasOptions, this.InitialWidget == PauseWidget.ChildWidgets.OPTIONS);
			this.CurrentWidget = this.InitialWidget;
			this.MapWidget.Initialize();
			this.OptionsWidget.Initialize();
			if (this.InitialWidget == PauseWidget.ChildWidgets.MAP)
			{
				this.MapWidget.OnShow(this.InitialMapMode);
			}
			else
			{
				this.OptionsWidget.OnShow(true);
			}
		}

		public override bool GoBack()
		{
			if (this.isFadingChild)
			{
				return true;
			}
			if (this.CurrentWidget == PauseWidget.ChildWidgets.MAP)
			{
				if (!this.MapWidget.GoBack())
				{
					this.InitialWidget = PauseWidget.ChildWidgets.MAP;
					return false;
				}
			}
			else if (!this.OptionsWidget.GoBack())
			{
				if (this.InitialWidget != PauseWidget.ChildWidgets.MAP)
				{
					this.InitialWidget = PauseWidget.ChildWidgets.MAP;
					return false;
				}
				this.CurrentWidget = PauseWidget.ChildWidgets.MAP;
				this.SwitchElements();
			}
			return true;
		}

		public void CenterView()
		{
			if (this.CurrentWidget != PauseWidget.ChildWidgets.MAP)
			{
				return;
			}
			this.MapWidget.CenterView();
		}

		public void UITabLeft()
		{
			if (this.CurrentWidget == PauseWidget.ChildWidgets.MAP)
			{
				this.MapWidget.UITabLeft();
			}
			else
			{
				this.OptionsWidget.SelectPreviousTutorial();
			}
		}

		public void UITabRight()
		{
			if (this.CurrentWidget == PauseWidget.ChildWidgets.MAP)
			{
				this.MapWidget.UITabRight();
			}
			else
			{
				this.OptionsWidget.SelectNextTutorial();
			}
		}

		public void SubmitPressed()
		{
			if (this.CurrentWidget != PauseWidget.ChildWidgets.MAP)
			{
				return;
			}
			this.MapWidget.SubmitPressed();
		}

		public bool ChangeToOptions()
		{
			if (this.CurrentWidget != PauseWidget.ChildWidgets.MAP)
			{
				return false;
			}
			if (this.MapWidget.CurrentMapMode == PauseWidget.MapModes.TELEPORT)
			{
				return false;
			}
			this.CurrentWidget = PauseWidget.ChildWidgets.OPTIONS;
			this.SwitchElements();
			return true;
		}

		public OptionsWidget.SCALING_STRATEGY GetScalingStrategy()
		{
			return this.OptionsWidget.GetScalingStrategy();
		}

		private void SetChildActive(CanvasGroup group, bool value)
		{
			group.alpha = ((!value) ? 0f : 1f);
			group.gameObject.SetActive(value);
		}

		private void SwitchElements()
		{
			this.isFadingChild = true;
			this.canvasOptions.gameObject.SetActive(true);
			this.canvasMap.gameObject.SetActive(true);
			CanvasGroup canvasTo1 = this.canvasOptions;
			CanvasGroup canvasTo0 = this.canvasMap;
			if (this.CurrentWidget == PauseWidget.ChildWidgets.MAP)
			{
				canvasTo1 = this.canvasMap;
				canvasTo0 = this.canvasOptions;
				this.MapWidget.OnShow(this.InitialMapMode);
			}
			else
			{
				this.OptionsWidget.OnShow(false);
			}
			TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(DOTween.To(() => canvasTo0.alpha, delegate(float x)
			{
				canvasTo0.alpha = x;
			}, 0f, this.alphaDurantionChild), new TweenCallback(this.EndFading));
			DOTween.To(() => canvasTo1.alpha, delegate(float x)
			{
				canvasTo1.alpha = x;
			}, 1f, this.alphaDurantionChild);
		}

		private void EndFading()
		{
			this.isFadingChild = false;
			if (this.CurrentWidget == PauseWidget.ChildWidgets.MAP)
			{
				this.canvasOptions.gameObject.SetActive(false);
			}
			else
			{
				this.canvasMap.gameObject.SetActive(false);
			}
		}

		public override bool IsActive()
		{
			return this.isActive;
		}

		public override void Activate()
		{
			base.Activate();
			base.gameObject.transform.localPosition = Vector3.zero;
			this.isActive = true;
		}

		public override void Deactivate()
		{
			base.gameObject.transform.localPosition = Vector3.up * 1000f;
			this.isActive = false;
		}

		public NewMapMenuWidget MapWidget;

		public OptionsWidget OptionsWidget;

		private bool isFadingChild;

		private bool isActive;

		private float alphaDurantionChild = 0.2f;

		private CanvasGroup canvasMap;

		private CanvasGroup canvasOptions;

		public enum ChildWidgets
		{
			MAP,
			OPTIONS
		}

		public enum MapModes
		{
			SHOW,
			TELEPORT
		}
	}
}
