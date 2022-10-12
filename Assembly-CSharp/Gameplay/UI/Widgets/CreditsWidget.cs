using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FMOD.Studio;
using Framework.Managers;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Widgets
{
	public class CreditsWidget : SerializedMonoBehaviour
	{
		public bool HasEnded { get; private set; }

		private void Awake()
		{
			CreditsWidget.instance = this;
			this.canvas = base.GetComponent<CanvasGroup>();
			this.HasEnded = false;
		}

		private void OnDestroy()
		{
			CreditsWidget.instance = null;
		}

		public float Alpha
		{
			get
			{
				if (this.canvas == null)
				{
					this.canvas = base.GetComponent<CanvasGroup>();
				}
				return this.canvas.alpha;
			}
			set
			{
				if (this.canvas == null)
				{
					this.canvas = base.GetComponent<CanvasGroup>();
				}
				this.canvas.alpha = value;
			}
		}

		private void OnEnable()
		{
			this.HasEnded = false;
			Core.Input.SetBlocker("CREDITS", true);
			if (this.uguiContent == null || this.uguiScrollbar == null || this.uguiScrollRect == null || this.uguiScrollView == null)
			{
				Debug.Log("Interactive Credits cannot function until you have filled in the required fields on the script component in the inspector.");
				base.enabled = false;
			}
			else
			{
				base.Invoke("DelayedSetup", 0.1f);
			}
			this.scrollbar = this.uguiScrollbar.GetComponent<Scrollbar>();
			this.scrollbar.enabled = true;
			this.tweenFloat = 1f;
			this.screenHeight = Screen.height;
			this.creditsElements = this.uguiContent.transform.GetComponentsInChildren<RectTransform>();
		}

		private void OnDisable()
		{
			Vector3 localPosition = this.uguiContent.transform.localPosition;
			localPosition.y = 0f;
			this.uguiContent.transform.localPosition = localPosition;
			this.scrollbarTween.Kill(false);
			Core.Input.SetBlocker("CREDITS", false);
		}

		private void Update()
		{
			if (!this.isAutoScrolling)
			{
				return;
			}
			this.scrollbar.value = this.tweenFloat;
			if (this.rewired == null)
			{
				this.rewired = ReInput.players.GetPlayer(0);
			}
			if (this.rewired.GetButton(39))
			{
				this.timePressingSkip += Time.unscaledDeltaTime;
				if (this.timePressingSkip >= this.timeToSkip)
				{
					this.EndOfCredits();
				}
			}
			else
			{
				this.timePressingSkip = 0f;
			}
			this.skipMask.fillAmount = this.timePressingSkip / this.timeToSkip;
			float axisRaw = this.rewired.GetAxisRaw(49);
			if (Mathf.Abs(axisRaw) > this.axisThreshold)
			{
				this.ProcessScrollInput(axisRaw);
			}
			this.RefreshVisibleElements();
		}

		private void RefreshVisibleElements()
		{
			for (int i = 0; i < this.creditsElements.Length; i++)
			{
				if (!(this.creditsElements[i].gameObject == this.uguiContent))
				{
					this.creditsElements[i].GetWorldCorners(this.elementCorners);
					float y = this.elementCorners[1].y;
					float y2 = this.elementCorners[0].y;
					bool flag = y > -32f && y2 < (float)(this.screenHeight + 32);
					if (this.creditsElements[i].gameObject.activeInHierarchy != flag)
					{
						this.creditsElements[i].gameObject.SetActive(flag);
					}
				}
			}
		}

		private void ProcessScrollInput(float scrollAxis)
		{
			if (this.scrollbarTween == null)
			{
				return;
			}
			float axisRawPrev = this.rewired.GetAxisRawPrev(49);
			if (axisRawPrev == 0f)
			{
				this.framesSkipped = 0;
				if (scrollAxis > 0f)
				{
					this.ProcessScrollUpwards();
				}
				else
				{
					this.ProcessScrollDownwards();
				}
			}
			else
			{
				float axisTimeActive = this.rewired.GetAxisTimeActive(49);
				int num = (axisTimeActive <= this.delaySecondsForFastScroll) ? this.skippedFramesForSlowScroll : this.skippedFramesForFastScroll;
				this.framesSkipped++;
				if (this.framesSkipped % num == 0)
				{
					this.framesSkipped = 0;
					if (scrollAxis > 0f)
					{
						this.ProcessScrollUpwards();
					}
					else
					{
						this.ProcessScrollDownwards();
					}
				}
			}
		}

		private void DelayedSetup()
		{
			base.Invoke("InitialEaseIn", this.initialDelay);
			this.rtScrollRect = this.uguiScrollView.GetComponent<RectTransform>();
			Vector3 size = new Vector3(this.rtScrollRect.rect.width, this.rtScrollRect.rect.height, 1f);
			if (this.uguiScrollRect.GetComponent<ScrollRect>() != null && this.uguiScrollRect.GetComponent<BoxCollider>() == null)
			{
				BoxCollider boxCollider = this.uguiScrollRect.AddComponent<BoxCollider>();
				boxCollider.size = size;
			}
			this.rtContent = this.uguiContent.GetComponent<RectTransform>();
			this.time = (this.rtContent.rect.height - this.rtScrollRect.rect.height) / this.scrollSpeed;
			this.easeInA = (this.time - 0.5f) / this.time;
			VerticalLayoutGroup component = this.uguiContent.GetComponent<VerticalLayoutGroup>();
			ContentSizeFitter component2 = this.uguiContent.GetComponent<ContentSizeFitter>();
			if (component != null)
			{
				component.enabled = false;
			}
			if (component2 != null)
			{
				component2.enabled = false;
			}
		}

		private void ProcessScrollDownwards()
		{
			if (this.scrollbarTween.timeScale == this.timeScaleLimit && this.isScrollingDown)
			{
				return;
			}
			float num = 0f;
			if (this.isScrollingDown)
			{
				if (this.scrollbarTween.timeScale == 0f)
				{
					num = 1f;
				}
				else
				{
					num = this.scrollbarTween.timeScale * 2f;
				}
			}
			else if (this.scrollbarTween.timeScale > 1f)
			{
				num = 1f;
			}
			else if (this.scrollbarTween.timeScale == 1f)
			{
				num = 0f;
			}
			else if (this.scrollbarTween.timeScale == 0f)
			{
				this.isScrollingDown = true;
				num = 1f;
				if (this.scrollbarTween.IsBackwards())
				{
					this.scrollbarTween.PlayForward();
				}
				else
				{
					this.scrollbarTween.PlayBackwards();
				}
			}
			this.UpdateScrollSpeedIndicators(num);
			this.scrollbarTween.DOTimeScale(num, 0f);
		}

		private void ProcessScrollUpwards()
		{
			if (this.scrollbarTween.timeScale == this.timeScaleLimit && !this.isScrollingDown)
			{
				return;
			}
			float num = 0f;
			if (this.isScrollingDown)
			{
				if (this.scrollbarTween.timeScale > 1f)
				{
					num = 1f;
				}
				else if (this.scrollbarTween.timeScale == 1f)
				{
					num = 0f;
				}
				else if (this.scrollbarTween.timeScale == 0f)
				{
					this.isScrollingDown = false;
					num = 1f;
					if (this.scrollbarTween.IsBackwards())
					{
						this.scrollbarTween.PlayForward();
					}
					else
					{
						this.scrollbarTween.PlayBackwards();
					}
				}
			}
			else if (this.scrollbarTween.timeScale == 0f)
			{
				num = 1f;
			}
			else
			{
				num = this.scrollbarTween.timeScale * 2f;
			}
			this.UpdateScrollSpeedIndicators(num);
			this.scrollbarTween.DOTimeScale(num, 0f);
		}

		private void UpdateScrollSpeedIndicators(float newTimeScale)
		{
			if (!this.scrollSpeedIndicatorsRoot.activeInHierarchy)
			{
				this.scrollSpeedIndicatorsRoot.SetActive(true);
			}
			if (newTimeScale == 0f)
			{
				this.upArrow.SetActive(false);
				this.downArrow.SetActive(false);
			}
			else
			{
				this.upArrow.SetActive(!this.isScrollingDown);
				this.downArrow.SetActive(this.isScrollingDown);
			}
			this.scrollSpeedText.text = "x" + Mathf.FloorToInt(newTimeScale);
		}

		private void InitialEaseIn()
		{
			this.tweenFloat = 1f;
			this.scrollbarTween = DOTween.To(() => this.tweenFloat, delegate(float x)
			{
				this.tweenFloat = x;
			}, this.easeInA, 1f).SetEase(Ease.InQuad).OnComplete(new TweenCallback(this.InitialScroll)).SetId("InitialScroll");
		}

		private void InitialScroll()
		{
			this.isScrollingDown = true;
			this.isAutoScrolling = true;
			this.tweenFloat = this.easeInA;
			float duration = this.time * this.tweenFloat;
			this.scrollbarTween = DOTween.To(() => this.tweenFloat, delegate(float x)
			{
				this.tweenFloat = x;
			}, 0f, duration).SetEase(Ease.Linear).OnComplete(new TweenCallback(this.AutoScrollEnd)).OnRewind(new TweenCallback(this.StartScrollAgain)).SetId("ScrollPro");
		}

		private void StartScrollAgain()
		{
			this.InitialScroll();
			this.UpdateScrollSpeedIndicators(1f);
		}

		private void ScrollFromAValue(float scrollbarValue)
		{
			this.isScrollingDown = true;
			this.tweenFloat = scrollbarValue;
			float duration = this.time * this.tweenFloat;
			this.scrollbarTween.Kill(false);
			this.scrollbarTween = DOTween.To(() => this.tweenFloat, delegate(float x)
			{
				this.tweenFloat = x;
			}, 0f, duration).SetEase(Ease.Linear).OnComplete(new TweenCallback(this.AutoScrollEnd)).OnRewind(new TweenCallback(this.ScrollToStart)).SetId("ScrollPro");
			this.UpdateScrollSpeedIndicators(1f);
		}

		private void ScrollToEnd()
		{
			this.isScrollingDown = true;
			float timeScale = this.scrollbarTween.timeScale;
			float duration = this.time * this.tweenFloat;
			this.scrollbarTween.Kill(false);
			this.scrollbarTween = DOTween.To(() => this.tweenFloat, delegate(float x)
			{
				this.tweenFloat = x;
			}, 0f, duration).SetEase(Ease.Linear).OnComplete(new TweenCallback(this.AutoScrollEnd)).OnRewind(new TweenCallback(this.ScrollToStart)).SetId("ScrollPro");
			this.scrollbarTween.ForceInit();
			this.scrollbarTween.timeScale = timeScale;
			this.UpdateScrollSpeedIndicators(timeScale);
		}

		private void ScrollToStart()
		{
			this.isScrollingDown = false;
			float timeScale = this.scrollbarTween.timeScale;
			float duration = this.time * (1f - this.tweenFloat);
			this.scrollbarTween.Kill(false);
			this.scrollbarTween = DOTween.To(() => this.tweenFloat, delegate(float x)
			{
				this.tweenFloat = x;
			}, 1f, duration).SetEase(Ease.Linear).OnComplete(new TweenCallback(this.AutoScrollEnd)).OnRewind(new TweenCallback(this.ScrollToEnd)).SetId("ScrollPro");
			this.scrollbarTween.ForceInit();
			this.scrollbarTween.timeScale = timeScale;
			this.UpdateScrollSpeedIndicators(timeScale);
		}

		private void AutoScrollEnd()
		{
			if (this.timeAtEndScrollToClose > 0f)
			{
				base.StartCoroutine(this.WaitAndEnd());
			}
			else
			{
				this.EndOfCredits();
			}
		}

		private IEnumerator WaitAndEnd()
		{
			yield return new WaitForSeconds(this.timeAtEndScrollToClose);
			this.EndOfCredits();
			yield break;
		}

		private void EndOfCredits()
		{
			Core.Audio.Music.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.isAutoScrolling = false;
			this.HasEnded = true;
			if (this.autoDisable)
			{
				base.gameObject.SetActive(false);
			}
		}

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private GameObject uguiScrollView;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private GameObject uguiScrollRect;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private GameObject uguiContent;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private GameObject uguiScrollbar;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Image skipMask;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private GameObject upArrow;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private GameObject downArrow;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Text scrollSpeedText;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private GameObject scrollSpeedIndicatorsRoot;

		[BoxGroup("Config", true, false, 0)]
		[SerializeField]
		private bool autoDisable = true;

		[BoxGroup("Config", true, false, 0)]
		[SerializeField]
		[Range(0f, 500f)]
		private float scrollSpeed = 50f;

		[BoxGroup("Config", true, false, 0)]
		[SerializeField]
		private float initialDelay = 0.9f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float timeToSkip = 1.5f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float timeAtEndScrollToClose = 2f;

		private Scrollbar scrollbar;

		private Tweener scrollbarTween;

		private float tweenFloat;

		private bool isAutoScrolling;

		private float easeInA;

		private float time;

		private Player rewired;

		private float timePressingSkip;

		private CanvasGroup canvas;

		private const string INITIAL_SCROLL_TWEEN_ID = "InitialScroll";

		private const string NORMAL_SCROLL_TWEEN_ID = "ScrollPro";

		private const string INCREASE_SPEED_TWEEN_ID = "IncreaseScrollSpeedCredits";

		private const string DECREASE_SPEED_TWEEN_ID = "DecreaseScrollSpeedCredits";

		private const string RETURN_TO_NORMAL_SPEED_TWEEN_ID = "ReturnToNormalScrollSpeedCredits";

		private const int VISIBILITY_OFFSET = 32;

		private int screenHeight = Screen.height;

		private readonly Vector3[] elementCorners = new Vector3[4];

		private RectTransform rtContent;

		private RectTransform rtScrollRect;

		private int framesSkipped;

		private float delaySecondsForFastScroll = 1f;

		private int skippedFramesForFastScroll = 15;

		private int skippedFramesForSlowScroll = 30;

		private float axisThreshold = 0.3f;

		private float timeScaleLimit = 64f;

		private bool isScrollingDown;

		public static CreditsWidget instance;

		private RectTransform[] creditsElements;
	}
}
