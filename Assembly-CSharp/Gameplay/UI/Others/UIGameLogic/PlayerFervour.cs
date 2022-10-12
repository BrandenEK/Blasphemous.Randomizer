using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class PlayerFervour : MonoBehaviour
	{
		public static PlayerFervour Instance { get; private set; }

		private void Awake()
		{
			PlayerFervour.Instance = this;
			this.anims = new List<RectTransform>();
			this.fillExactTransform = this.fillExact.GetComponent<RectTransform>();
			this.fillExactFullTransform = this.fillExactFull.GetComponent<RectTransform>();
			this.fillAnimableTransform = this.fillAnimable.GetComponent<RectTransform>();
			this.fillNotEnoughTransform = this.fillNotEnough.GetComponent<RectTransform>();
			this.ChangeImageAlpha(this.fillNotEnough, 0f);
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			LevelManager.OnBeforeLevelLoad += this.OnBeforeLevelLoad;
			this.normalPrayerInUse.SetActive(false);
			this.pe02PrayerInUse.SetActive(false);
			this.prayerTimer.fillAmount = 0f;
			base.enabled = false;
			this.fervourSpark.SetActive(false);
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			LevelManager.OnBeforeLevelLoad -= this.OnBeforeLevelLoad;
		}

		private void OnBeforeLevelLoad(Level oldLevel, Level newLevel)
		{
			base.enabled = false;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			base.enabled = true;
		}

		private void Update()
		{
			PrayerUse prayerCast = Core.Logic.Penitent.PrayerCast;
			this.normalPrayerInUse.SetActive(prayerCast.IsUsingAbility && !Core.PenitenceManager.UseStocksOfHealth);
			this.pe02PrayerInUse.SetActive(prayerCast.IsUsingAbility && Core.PenitenceManager.UseStocksOfHealth);
			float fillAmount = 0f;
			if (prayerCast.IsUsingAbility)
			{
				fillAmount = 1f - prayerCast.GetPercentTimeCasting();
			}
			this.prayerTimer.fillAmount = fillAmount;
			float barTarget = this.BarTarget;
			if (this.lastValue != barTarget)
			{
				this.fillsIncrease = (this.lastValue < barTarget);
				this.lastValue = barTarget;
				this._timeElapsed = 0f;
			}
			this.CalculateBarSize();
			this.CalculateFillsBars();
			this.CalculateMarks();
			this.CalculateNotEnough();
			Penitent penitent = Core.Logic.Penitent;
			float currentMaxWithoutFactor = penitent.Stats.Fervour.CurrentMaxWithoutFactor;
			if (currentMaxWithoutFactor != this.lastMaxFervour)
			{
				this.lastMaxFervour = currentMaxWithoutFactor;
				this.CalculateBarPentalty();
			}
		}

		public void Show()
		{
			base.gameObject.SetActive(true);
		}

		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		public void RefreshGuilt(bool whenDead)
		{
			if (whenDead)
			{
				base.StartCoroutine(this.RefreshGuiltWhenDead());
			}
			else
			{
				Penitent penitent = Core.Logic.Penitent;
				float maxFactor = penitent.Stats.Fervour.MaxFactor;
				Image component = this.guiltBar.gameObject.GetComponent<Image>();
				if (maxFactor >= 1f)
				{
					component.DOFade(0f, 1f);
					this.guiltEnd.DOFade(0f, 1f);
					this.guiltStart.DOFade(0f, 1f);
				}
				this.CalculateBarPentalty();
			}
		}

		public void NotEnoughFervour()
		{
			if (this.isNotEnough)
			{
				return;
			}
			this.isNotEnough = true;
			this.currentNotEnough = 0f;
			Prayer prayerInSlot = Core.InventoryManager.GetPrayerInSlot(0);
			if (prayerInSlot == null)
			{
				if (!string.IsNullOrEmpty(this.notEnoughSoundNotEquipped))
				{
					Core.Audio.PlayOneShot(this.notEnoughSoundNotEquipped, default(Vector3));
				}
			}
			else if (!string.IsNullOrEmpty(this.notEnoughSound))
			{
				Core.Audio.PlayOneShot(this.notEnoughSound, default(Vector3));
			}
		}

		private IEnumerator RefreshGuiltWhenDead()
		{
			Penitent penitent = Core.Logic.Penitent;
			float factor = penitent.Stats.Fervour.MaxFactor;
			Image bar = this.guiltBar.gameObject.GetComponent<Image>();
			bar.color = new Color(1f, 1f, 1f, 0f);
			this.guiltEnd.color = new Color(1f, 1f, 1f, 0f);
			this.guiltStart.color = new Color(1f, 1f, 1f, 0f);
			this.guiltBar.sizeDelta = new Vector2(0f, this.guiltBar.sizeDelta.y);
			yield return new WaitForSeconds(2f);
			if (factor < 1f)
			{
				bar.DOFade(1f, 1f);
				this.guiltEnd.DOFade(1f, 1f);
				this.guiltStart.DOFade(1f, 1f);
			}
			this.CalculateBarPentalty();
			yield break;
		}

		private float BarTarget
		{
			get
			{
				Penitent penitent = Core.Logic.Penitent;
				float num = penitent.Stats.Fervour.Current;
				float currentMaxWithoutFactor = penitent.Stats.Fervour.CurrentMaxWithoutFactor;
				return num / currentMaxWithoutFactor;
			}
		}

		private void CalculateBarSize()
		{
			Penitent penitent = Core.Logic.Penitent;
			float currentMaxWithoutFactor = penitent.Stats.Fervour.CurrentMaxWithoutFactor;
			if (currentMaxWithoutFactor != this.lastBarWidth)
			{
				this.lastBarWidth = currentMaxWithoutFactor;
				float num = currentMaxWithoutFactor - this.backgroundStartSize - this.endFillSize;
				num = ((num <= 0f) ? 0f : num);
				this.backgroundMid.sizeDelta = new Vector2(num, this.backgroundMid.sizeDelta.y);
				this.fillExactTransform.sizeDelta = new Vector2(currentMaxWithoutFactor, this.fillExactTransform.sizeDelta.y);
				this.fillExactFullTransform.sizeDelta = new Vector2(currentMaxWithoutFactor, this.fillExactFullTransform.sizeDelta.y);
				this.fillAnimableTransform.sizeDelta = new Vector2(currentMaxWithoutFactor, this.fillAnimableTransform.sizeDelta.y);
				this.background.sizeDelta = new Vector2(currentMaxWithoutFactor, this.background.sizeDelta.y);
				this.fillNotEnoughTransform.sizeDelta = new Vector2(currentMaxWithoutFactor, this.fillNotEnoughTransform.sizeDelta.y);
			}
		}

		private void CalculateBarPentalty()
		{
			Penitent penitent = Core.Logic.Penitent;
			float currentMaxWithoutFactor = penitent.Stats.Fervour.CurrentMaxWithoutFactor;
			float maxFactor = penitent.Stats.Fervour.MaxFactor;
			this.guiltRoot.SetActive(maxFactor < 1f);
			if (maxFactor < 1f)
			{
				float x = (1f - maxFactor) * currentMaxWithoutFactor - (float)this.positionOcupedByBack;
				this.guiltBar.DOSizeDelta(new Vector2(x, this.guiltBar.sizeDelta.y), 2f, false);
			}
		}

		private void CalculateFillsBars()
		{
			this._timeElapsed += Time.deltaTime;
			if (this.fillsIncrease)
			{
				if (Mathf.Approximately(this.fillExact.fillAmount, this.BarTarget))
				{
					this.fillExact.fillAmount = this.BarTarget;
					this._timeElapsed = 0f;
				}
				else
				{
					this.fillExact.fillAmount = Mathf.Lerp(this.fillExact.fillAmount, this.BarTarget, this.AddAnimationCurve.Evaluate(this._timeElapsed));
					float x = (float)((int)Core.Logic.Penitent.Stats.Fervour.CurrentMaxWithoutFactor) * this.fillExact.fillAmount - 1f;
					this.fervourSpark.transform.localPosition = new Vector3(x, this.fervourSpark.transform.localPosition.y);
				}
				this.fillAnimable.fillAmount = this.fillExact.fillAmount;
			}
			else
			{
				this.fillExact.fillAmount = this.BarTarget;
				if (Mathf.Approximately(this.fillAnimable.fillAmount, this.BarTarget))
				{
					this.fillAnimable.fillAmount = this.BarTarget;
					this._timeElapsed = 0f;
				}
				else
				{
					this.fillAnimable.fillAmount = Mathf.Lerp(this.fillAnimable.fillAmount, this.BarTarget, this.LossAnimationCurve.Evaluate(this._timeElapsed));
				}
			}
			this.fillNotEnough.fillAmount = this.fillExact.fillAmount;
		}

		private void CalculateMarks()
		{
			int num = 0;
			int num2 = 0;
			Prayer prayerInSlot = Core.InventoryManager.GetPrayerInSlot(0);
			int num3 = (!prayerInSlot) ? 0 : (prayerInSlot.fervourNeeded + (int)Core.Logic.Penitent.Stats.PrayerCostAddition.Final);
			if (num3 > 0)
			{
				num = (int)Core.Logic.Penitent.Stats.Fervour.CurrentMax / num3;
				num2 = (int)Core.Logic.Penitent.Stats.Fervour.Current / num3;
				this.fillExactFull.fillAmount = (float)(num2 * num3) / Core.Logic.Penitent.Stats.Fervour.CurrentMaxWithoutFactor;
			}
			else
			{
				this.fillExactFull.fillAmount = 0f;
			}
			bool flag = Core.Logic.Penitent.Stats.Fervour.CurrentMax - (float)(num3 * num) > this.epsilonToShowLastBar;
			bool flag2 = false;
			float num4 = (float)(-(float)num3) + 1f;
			if (num != this.currentMarks || num3 != this.currentMarksSeparation || (float)num2 != this.currentSegmentsFilled)
			{
				if (num == 0)
				{
					this.currentAnimPosition = num4;
					this.currentAnimElapsed = 0f;
					flag2 = true;
				}
				this.anims.Clear();
				if (this.currentAnimPosition > (float)this.barAnimEndPosition)
				{
					this.currentAnimPosition = num4;
				}
				this.currentMarks = num;
				this.currentMarksSeparation = num3;
				this.currentSegmentsFilled = (float)num2;
				float num5 = 0f;
				for (int i = 0; i < this.marksParent.childCount; i++)
				{
					RectTransform rectTransform = (RectTransform)this.marksParent.GetChild(i);
					bool flag3 = i < this.currentMarks;
					rectTransform.gameObject.SetActive(flag3);
					if (flag3)
					{
						rectTransform.sizeDelta = new Vector2((float)num3, rectTransform.sizeDelta.y);
						rectTransform.localPosition = new Vector3(num5, 0f, 0f);
						num5 += (float)this.currentMarksSeparation;
						RectTransform rectTransform2 = (RectTransform)rectTransform.Find(this.barMaskChildName);
						rectTransform2.sizeDelta = new Vector2((float)num3 - 1f, rectTransform2.sizeDelta.y);
						RectTransform rectTransform3 = (RectTransform)rectTransform.Find(this.barBarChildName);
						rectTransform3.gameObject.SetActive(flag || i != this.currentMarks - 1);
						bool flag4 = (float)i < this.currentSegmentsFilled;
						RectTransform rectTransform4 = (RectTransform)rectTransform2.Find(this.barAnimChildName);
						rectTransform4.gameObject.SetActive(flag4);
						if (flag4)
						{
							this.SetBarPosition(rectTransform4);
							this.anims.Add(rectTransform4);
						}
					}
				}
			}
			if (!flag2 && num > 0)
			{
				this.currentAnimElapsed += Time.deltaTime;
				if (this.currentAnimElapsed >= this.barAnimUpdatedElapsed)
				{
					this.currentAnimElapsed = 0f;
					this.currentAnimPosition += this.barAnimMovementPerElapsed;
					if (this.currentAnimPosition > (float)this.barAnimEndPosition)
					{
						this.currentAnimPosition = num4;
					}
					this.anims.ForEach(delegate(RectTransform anim)
					{
						this.SetBarPosition(anim);
					});
				}
			}
		}

		private void CalculateNotEnough()
		{
			if (!this.isNotEnough)
			{
				return;
			}
			float alpha = 0f;
			this.currentNotEnough += Time.deltaTime;
			if (this.currentNotEnough >= this.barNotEnoughDuration)
			{
				this.isNotEnough = false;
				this.currentNotEnough = 0f;
			}
			else
			{
				alpha = this.NotEnoughAlphaAnimationCurve.Evaluate(this.currentNotEnough);
			}
			this.ChangeImageAlpha(this.fillNotEnough, alpha);
		}

		private void SetBarPosition(RectTransform bar)
		{
			bar.localPosition = new Vector3(this.currentAnimPosition, bar.localPosition.y);
		}

		private void ChangeImageAlpha(Image img, float alpha)
		{
			Color color = img.color;
			color.a = alpha;
			img.color = color;
		}

		public void ShowSpark()
		{
			base.StartCoroutine(this.ShowSparkCoroutine());
		}

		private IEnumerator ShowSparkCoroutine()
		{
			yield return null;
			PlayerFervour.Instance.fervourSpark.SetActive(true);
			float securityTimeLeft = 2f;
			while (!Mathf.Approximately(this.fillExact.fillAmount, this.BarTarget) && securityTimeLeft > 0f)
			{
				securityTimeLeft -= Time.deltaTime;
				float posx = (float)((int)Core.Logic.Penitent.Stats.Fervour.CurrentMaxWithoutFactor) * this.fillExact.fillAmount - 1f;
				this.fervourSpark.transform.localPosition = new Vector3(posx, this.fervourSpark.transform.localPosition.y);
				yield return null;
			}
			this.fervourSpark.SetActive(false);
			yield break;
		}

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private Image fillAnimable;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private Image fillExact;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private Image fillExactFull;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private Image fillNotEnough;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private AnimationCurve LossAnimationCurve;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private AnimationCurve AddAnimationCurve;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private AnimationCurve NotEnoughAlphaAnimationCurve;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private RectTransform background;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private RectTransform backgroundMid;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private float backgroundStartSize = 60f;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private float endFillSize = 10f;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		[Range(0f, 10f)]
		private float speed;

		[BoxGroup("Bar", true, false, 0)]
		[SerializeField]
		private Transform marksParent;

		[BoxGroup("Prayer Info", true, false, 0)]
		[SerializeField]
		private GameObject normalPrayerInUse;

		[BoxGroup("Prayer Info", true, false, 0)]
		[SerializeField]
		private GameObject pe02PrayerInUse;

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		private GameObject fervourSpark;

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		private string barAnimChildName = "Anim";

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		private string barMaskChildName = "Mask";

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		private string barBarChildName = "Bar";

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		private float epsilonToShowLastBar = 5f;

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		private int barAnimEndPosition = 7;

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		private float barAnimUpdatedElapsed = 0.4f;

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		private float barAnimMovementPerElapsed = 4f;

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		private float barNotEnoughDuration = 0.5f;

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		[EventRef]
		private string notEnoughSound = string.Empty;

		[BoxGroup("Bar Anim", true, false, 0)]
		[SerializeField]
		[EventRef]
		private string notEnoughSoundNotEquipped = string.Empty;

		[BoxGroup("Prayer Info", true, false, 0)]
		[SerializeField]
		private Image prayerTimer;

		[BoxGroup("Guilt", true, false, 0)]
		[SerializeField]
		private GameObject guiltRoot;

		[BoxGroup("Guilt", true, false, 0)]
		[SerializeField]
		private RectTransform guiltBar;

		[BoxGroup("Guilt", true, false, 0)]
		[SerializeField]
		private int positionOcupedByBack = 5;

		[BoxGroup("Guilt", true, false, 0)]
		[SerializeField]
		private Image guiltEnd;

		[BoxGroup("Guilt", true, false, 0)]
		[SerializeField]
		private Image guiltStart;

		private RectTransform fillAnimableTransform;

		private RectTransform fillExactTransform;

		private RectTransform fillExactFullTransform;

		private RectTransform fillNotEnoughTransform;

		private float _timeElapsed;

		private float lastBarWidth = -1f;

		private float lastValue = -1f;

		private float lastMaxFervour = -1f;

		private int currentMarks = -1;

		private int currentMarksSeparation = -1;

		private bool fillsIncrease;

		private List<RectTransform> anims;

		private float currentAnimPosition;

		private float currentAnimElapsed;

		private bool isNotEnough;

		private float currentNotEnough;

		private float currentSegmentsFilled = -1f;

		public const float EXECUTION_FERVOUR_BONUS = 13.3f;
	}
}
