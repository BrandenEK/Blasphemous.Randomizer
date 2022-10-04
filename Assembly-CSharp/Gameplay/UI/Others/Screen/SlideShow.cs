using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.Screen
{
	[RequireComponent(typeof(Image))]
	public class SlideShow : MonoBehaviour
	{
		public bool IsActive
		{
			get
			{
				return this.isActive;
			}
			set
			{
				this.isActive = value;
			}
		}

		private void Awake()
		{
			this.currentImage = base.GetComponent<Image>();
			this.rectTransform = base.GetComponent<RectTransform>();
			this.slidesNumber = this.slides.Length;
			this.hasCountCycle = false;
			this.isActive = false;
		}

		private void Start()
		{
			this.currentImage.sprite = this.slides[this.currentSlide].sprite;
			this.rectTransform.sizeDelta = new Vector2(this.slides[this.currentSlide].sprite.textureRect.width, this.slides[this.currentSlide].sprite.textureRect.height);
			this.countCycle = 0;
			this.slideCounter++;
			this.currentSlide = this.slideCounter % this.slidesNumber;
		}

		private void Update()
		{
			if (this.isActive)
			{
				Color color = this.currentImage.color;
				color.a = this.lerpAlpha();
				this.currentImage.color = color;
				this.timeSinceLast += Time.deltaTime;
			}
		}

		private void changeImage()
		{
			this.currentImage.sprite = this.slides[this.currentSlide].sprite;
			this.rectTransform.sizeDelta = new Vector2(this.slides[this.currentSlide].sprite.textureRect.width, this.slides[this.currentSlide].sprite.textureRect.height);
			this.timeSinceLast = 0f;
			this.slideCounter++;
			this.currentSlide = this.slideCounter % this.slidesNumber;
		}

		private float lerpAlpha()
		{
			if (this.isShowing)
			{
				this.deltaTimeShowing += Time.deltaTime;
				if (this.deltaTimeShowing <= this.timeShowing)
				{
					return this.currentAlpha;
				}
				this.deltaTimeShowing = 0f;
				this.isShowing = false;
			}
			float t = Mathf.PingPong(Time.time, this.changeTime) / this.changeTime;
			this.currentAlpha = Mathf.Lerp(0f, 1f, t);
			if (this.currentAlpha < 0.03f && !this.hasCountCycle)
			{
				this.hasCountCycle = true;
				this.countCycle++;
			}
			else if (this.currentAlpha > 0.95f && this.hasCountCycle && !this.isShowing)
			{
				this.hasCountCycle = false;
				this.isShowing = true;
			}
			if (this.countCycle > 0)
			{
				this.countCycle = 0;
				this.changeImage();
			}
			return this.currentAlpha;
		}

		private const float MIN_ALPHA = 0.03f;

		private const float MAX_ALPHA = 0.95f;

		public Image[] slides;

		public float changeTime = 10f;

		private int currentSlide;

		private int slideCounter;

		private float timeSinceLast = 1f;

		private Image currentImage;

		private RectTransform rectTransform;

		private float currentAlpha;

		public float timeShowing = 2f;

		private float deltaTimeShowing;

		private bool isShowing;

		private bool isActive;

		private int slidesNumber;

		private int countCycle;

		private bool hasCountCycle;
	}
}
