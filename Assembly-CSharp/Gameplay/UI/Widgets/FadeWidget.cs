using System;
using System.Collections;
using System.Diagnostics;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Widgets
{
	public class FadeWidget : UIWidget
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.SimpleEvent OnFadeShowStart;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.SimpleEvent OnFadeShowEnd;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.SimpleEvent OnFadeHidedStart;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.SimpleEvent OnFadeHidedEnd;

		public bool FadingIn { get; private set; }

		public bool FadingOut { get; private set; }

		public bool IsActive
		{
			get
			{
				return this.black.IsActive() && this.black.color.a > 0f;
			}
		}

		public void Awake()
		{
			this.black = base.GetComponent<Image>();
			FadeWidget.instance = this;
			this.black.color = this.ON_COLOR;
		}

		public void Fade(bool toBlack, float duration = 1.5f, float delay = 0f, Core.SimpleEvent callback = null)
		{
			if (this.currentTween != null)
			{
				this.currentTween.Kill(false);
				this.currentTween = null;
			}
			if (toBlack)
			{
				Core.Input.SetBlocker("FADE", true);
			}
			this.FadingIn = !toBlack;
			this.FadingOut = toBlack;
			if (toBlack && FadeWidget.OnFadeShowStart != null)
			{
				FadeWidget.OnFadeShowStart();
			}
			if (!toBlack && FadeWidget.OnFadeHidedStart != null)
			{
				FadeWidget.OnFadeHidedStart();
			}
			Color target = (!toBlack) ? this.OFF_COLOR : this.ON_COLOR;
			this.black.color = (toBlack ? this.OFF_COLOR : this.ON_COLOR);
			base.StartCoroutine(this.FadeAfterDelay(toBlack, duration, delay, target, callback));
		}

		public IEnumerator FadeCoroutine(bool toBlack, float duration = 1.5f, float delay = 0f)
		{
			if (this.currentTween != null)
			{
				this.currentTween.Kill(false);
				this.currentTween = null;
			}
			if (toBlack)
			{
				Core.Input.SetBlocker("FADE", true);
			}
			this.FadingIn = !toBlack;
			this.FadingOut = toBlack;
			if (toBlack && FadeWidget.OnFadeShowStart != null)
			{
				FadeWidget.OnFadeShowStart();
			}
			if (!toBlack && FadeWidget.OnFadeHidedStart != null)
			{
				FadeWidget.OnFadeHidedStart();
			}
			Color target = (!toBlack) ? this.OFF_COLOR : this.ON_COLOR;
			this.black.color = (toBlack ? this.OFF_COLOR : this.ON_COLOR);
			yield return this.FadeAfterDelay(toBlack, duration, delay, target, null);
			yield break;
		}

		public void ClearFade()
		{
			this.black.color = this.OFF_COLOR;
		}

		private IEnumerator FadeAfterDelay(bool toBlack, float duration, float delay, Color target, Core.SimpleEvent callback)
		{
			yield return new WaitForSeconds(delay);
			this.currentTween = this.black.DOColor(target, duration).OnComplete(delegate
			{
				this.currentTween = null;
				this.FadingIn = false;
				this.FadingOut = false;
				if (!toBlack)
				{
					Core.Input.SetBlocker("FADE", false);
				}
				if (toBlack && FadeWidget.OnFadeShowEnd != null)
				{
					FadeWidget.OnFadeShowEnd();
				}
				if (!toBlack && FadeWidget.OnFadeHidedEnd != null)
				{
					FadeWidget.OnFadeHidedEnd();
				}
				this.ON_COLOR = this.ON_COLOR_BASE;
				this.black.color = target;
				if (callback != null)
				{
					callback();
				}
			});
			yield return this.currentTween;
			Log.Trace("Fade", "Called fade function. Active: " + toBlack, null);
			yield break;
		}

		public IEnumerator FadeCoroutine(Color originColor, Color targetColor, float seconds, bool toBlack, Action<bool> callback)
		{
			for (float counter = 0f; counter < seconds; counter += Time.deltaTime)
			{
				this.black.color = Color.Lerp(originColor, targetColor, counter / seconds);
				yield return null;
			}
			this.black.color = targetColor;
			if (callback != null)
			{
				callback(toBlack);
			}
			yield break;
		}

		public void StartEasyFade(Color originColor, Color targetColor, float seconds, bool toBlack)
		{
			if (toBlack)
			{
				Core.Input.SetBlocker("FADE", true);
			}
			this.FadingIn = !toBlack;
			this.FadingOut = toBlack;
			if (toBlack && FadeWidget.OnFadeShowStart != null)
			{
				FadeWidget.OnFadeShowStart();
			}
			if (!toBlack && FadeWidget.OnFadeHidedStart != null)
			{
				FadeWidget.OnFadeHidedStart();
			}
			if (seconds > 0f)
			{
				base.StartCoroutine(this.FadeCoroutine(originColor, targetColor, seconds, toBlack, new Action<bool>(this.OnEasyFadeComplete)));
			}
			else
			{
				this.black.color = targetColor;
				this.OnEasyFadeComplete(toBlack);
			}
		}

		private void OnEasyFadeComplete(bool toBlack)
		{
			if (!toBlack)
			{
				Core.Input.SetBlocker("FADE", false);
			}
			this.FadingIn = false;
			this.FadingOut = false;
			if (toBlack && FadeWidget.OnFadeShowEnd != null)
			{
				FadeWidget.OnFadeShowEnd();
			}
			if (!toBlack && FadeWidget.OnFadeHidedEnd != null)
			{
				FadeWidget.OnFadeHidedEnd();
			}
		}

		public void FadeInOut(float duration, float delay, Core.SimpleEvent outEnd = null, Core.SimpleEvent inEnd = null)
		{
			Core.Input.SetBlocker("FADE", true);
			if (this.currentTween != null)
			{
				this.currentTween.Kill(false);
				this.currentTween = null;
			}
			Tweener t = this.black.DOColor(this.ON_COLOR, duration).OnComplete(delegate
			{
				if (outEnd != null)
				{
					outEnd();
				}
				this.ON_COLOR = this.ON_COLOR_BASE;
			});
			Tweener t2 = this.black.DOColor(this.OFF_COLOR, duration).OnComplete(delegate
			{
				if (inEnd != null)
				{
					inEnd();
				}
				if (!Core.Events.GetFlag("CHERUB_RESPAWN"))
				{
					Core.Input.SetBlocker("FADE", false);
				}
			});
			Sequence sequence = DOTween.Sequence();
			sequence.SetDelay(delay);
			sequence.Append(t);
			sequence.Append(t2);
			sequence.Play<Sequence>();
		}

		public bool Fading
		{
			get
			{
				return this.currentTween != null;
			}
		}

		public void SetOffColor(Color color)
		{
			this.OFF_COLOR = color;
		}

		public void SetOnColor(Color color)
		{
			this.ON_COLOR = color;
			this.ON_COLOR_BASE = color;
		}

		public void ResetToBlack()
		{
			this.ON_COLOR = Color.black;
			this.ON_COLOR_BASE = Color.black;
		}

		private Color ON_COLOR_BASE = new Color(0f, 0f, 0f, 1f);

		private Color ON_COLOR = new Color(0f, 0f, 0f, 1f);

		private Color OFF_COLOR = new Color(0f, 0f, 0f, 0f);

		public static FadeWidget instance;

		private Tween currentTween;

		private Image black;
	}
}
