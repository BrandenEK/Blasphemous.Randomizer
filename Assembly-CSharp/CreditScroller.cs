using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class CreditScroller : MonoBehaviour
{
	private void Awake()
	{
		Input.multiTouchEnabled = false;
	}

	private void OnEnable()
	{
		this.letsGo = false;
		if (this.uguiCamera == null || this.uguiContent == null || this.uguiScrollbar == null || this.uguiScrollRect == null || this.uguiScrollView == null)
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
		this.isAutoScrolling = true;
	}

	private void OnDisable()
	{
		Vector3 localPosition = this.uguiContent.transform.localPosition;
		localPosition.y = 0f;
		this.uguiContent.transform.localPosition = localPosition;
		DOTween.Complete("ScrollPro", false);
		DOTween.Kill("ScrollPro", false);
	}

	private void Update()
	{
		if (this.letsGo && !this.touchInside && (double)Mathf.Abs(this.srComponent.velocity.y) <= 0.1)
		{
			this.letsGo = false;
			this.PressReleased();
		}
		if (this.isAutoScrolling)
		{
			this.scrollbar.value = this.tweenFloat;
		}
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = this.uguiCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray))
			{
				base.CancelInvoke("PressReleased");
				base.CancelInvoke("InitialEaseIn");
				this.scrollbarTween.Kill(false);
				this.isAutoScrolling = false;
				this.touchInside = true;
			}
			else
			{
				this.touchInside = false;
			}
		}
		if (Input.GetMouseButtonUp(0) && this.touchInside)
		{
			this.tweenFloat = this.scrollbar.value;
			this.letsGo = true;
			this.touchInside = false;
		}
	}

	private void DelayedSetup()
	{
		base.Invoke("InitialEaseIn", this.initialDelay);
		RectTransform component = this.uguiScrollView.GetComponent<RectTransform>();
		Vector3 size = new Vector3(component.rect.width, component.rect.height, 1f);
		if (this.uguiScrollRect.GetComponent<ScrollRect>() != null && this.uguiScrollRect.GetComponent<BoxCollider>() == null)
		{
			BoxCollider boxCollider = this.uguiScrollRect.AddComponent<BoxCollider>();
			boxCollider.size = size;
		}
		this.srComponent = this.uguiScrollRect.GetComponent<ScrollRect>();
		RectTransform component2 = this.uguiContent.GetComponent<RectTransform>();
		this.mathHelperA = component2.rect.height - component.rect.height;
		this.initialTime = this.mathHelperA / this.scrollSpeed;
		this.mathHelperA /= this.scrollSpeed / 2f;
		this.mathHelperB = this.mathHelperA - 1f;
		this.easeInA = this.mathHelperB / this.mathHelperA;
		this.easeInB = 1f - this.easeInA;
		this.time = this.initialTime;
		this.easeStart = 1f;
		this.currentStart = 1f;
		this.percentage = 0f;
		VerticalLayoutGroup component3 = this.uguiContent.GetComponent<VerticalLayoutGroup>();
		ContentSizeFitter component4 = this.uguiContent.GetComponent<ContentSizeFitter>();
		if (component3 != null)
		{
			component3.enabled = false;
		}
		if (component4 != null)
		{
			component4.enabled = false;
		}
	}

	private void InitialEaseIn()
	{
		this.tweenFloat = this.currentStart;
		this.scrollbarTween = DOTween.To(() => this.tweenFloat, delegate(float x)
		{
			this.tweenFloat = x;
		}, this.easeInA, 1f).SetEase(Ease.InQuad).OnComplete(new TweenCallback(this.InitialScroll)).SetId("ScrollPro");
	}

	private void InitialScroll()
	{
		this.tweenFloat = this.easeInA;
		this.scrollbarTween = DOTween.To(() => this.tweenFloat, delegate(float x)
		{
			this.tweenFloat = x;
		}, 0f, this.time).SetEase(Ease.Linear).OnComplete(new TweenCallback(this.AutoScrollEnd)).SetId("ScrollPro");
	}

	private void PressReleased()
	{
		this.currentStart = this.scrollbar.value;
		this.time = this.initialTime;
		this.percentage = this.currentStart;
		this.time *= this.percentage;
		this.easeStart = this.currentStart - this.easeInB;
		this.tweenFloat = this.currentStart;
		this.isAutoScrolling = true;
		if (this.tweenFloat != 0f)
		{
			this.scrollbarTween = DOTween.To(() => this.tweenFloat, delegate(float x)
			{
				this.tweenFloat = x;
			}, this.easeStart, 1f).SetEase(Ease.InQuad).OnComplete(new TweenCallback(this.AutoScrolling)).SetId("ScrollPro");
		}
	}

	private void AutoScrolling()
	{
		this.scrollbarTween = DOTween.To(() => this.tweenFloat, delegate(float x)
		{
			this.tweenFloat = x;
		}, 0f, this.time).SetEase(Ease.Linear).OnComplete(new TweenCallback(this.AutoScrollEnd)).SetId("ScrollPro");
	}

	private void AutoScrollEnd()
	{
		this.isAutoScrolling = false;
	}

	public Camera uguiCamera;

	public GameObject uguiScrollView;

	public GameObject uguiScrollRect;

	public GameObject uguiContent;

	public GameObject uguiScrollbar;

	private Scrollbar scrollbar;

	private Tweener scrollbarTween;

	private float tweenFloat;

	private bool isAutoScrolling;

	private bool touchInside;

	private float currentStart;

	private float easeInA;

	private float easeInB;

	private float easeStart;

	private int fingerID;

	private float initialTime;

	private float mathHelperA;

	private float mathHelperB;

	private float percentage;

	private float time;

	private bool letsGo;

	private ScrollRect srComponent;

	[Range(0f, 500f)]
	public float scrollSpeed = 50f;

	public float initialDelay = 0.9f;
}
