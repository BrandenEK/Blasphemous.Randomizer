using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class QuoteWidget : BaseMenuScreen
	{
		public bool IsOpen { get; private set; }

		public void Awake()
		{
			this.IsOpen = false;
		}

		public void Open(float fadeInTime, float timeActive, float fadeOutTime, Action onFinish)
		{
			this.fadeInTime = fadeInTime;
			this.timeActive = timeActive;
			this.fadeOutTime = fadeOutTime;
			this.onFinish = onFinish;
			this.Open();
		}

		public override void Open()
		{
			base.Open();
			this.IsOpen = true;
			base.gameObject.SetActive(true);
			this.canvasGroup = base.GetComponent<CanvasGroup>();
			this.canvasGroup.alpha = 0f;
			DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 1f, this.fadeInTime).OnComplete(new TweenCallback(this.StartToWait));
		}

		public override void Close()
		{
			if (!this.IsClosing)
			{
				this.IsClosing = true;
				DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
				{
					this.canvasGroup.alpha = x;
				}, 0f, this.fadeOutTime).OnComplete(new TweenCallback(this.OnClose));
				base.Close();
			}
		}

		protected override void OnClose()
		{
			this.IsOpen = false;
			base.gameObject.SetActive(false);
			this.IsClosing = false;
			this.onFinish();
		}

		private void StartToWait()
		{
			base.StartCoroutine(this.Wait());
		}

		private IEnumerator Wait()
		{
			yield return new WaitForSeconds(this.timeActive);
			if (this.IsOpen)
			{
				this.Close();
			}
			yield break;
		}

		private CanvasGroup canvasGroup;

		private float fadeInTime;

		private float timeActive;

		private float fadeOutTime;

		private Action onFinish;

		private bool IsClosing;
	}
}
