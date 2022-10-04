using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Damage;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class PlayerHealth : MonoBehaviour
	{
		private float BarTarget
		{
			get
			{
				Penitent penitent = Core.Logic.Penitent;
				if (penitent)
				{
					float num = penitent.Stats.Life.Current;
					float final = penitent.Stats.Life.Final;
					return num / final;
				}
				return 0f;
			}
		}

		private void Awake()
		{
			SpawnManager.OnPlayerSpawn += this.OnPenitentReady;
			this.lossTransform = this.loss.GetComponent<RectTransform>();
			this.healthTransform = this.health.GetComponent<RectTransform>();
			base.enabled = false;
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPenitentReady;
		}

		private void OnPenitentReady(Penitent penitent)
		{
			base.enabled = true;
			PenitentDamageArea damageArea = penitent.DamageArea;
			damageArea.OnDamaged = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Combine(damageArea.OnDamaged, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamaged));
		}

		private void Update()
		{
			this.CalculateHealthBarSize();
			this.CalculateLossBar();
			this.CalculateHealthBar();
		}

		private void OnDamaged(Penitent damaged, Hit hit)
		{
			this._damageTimeElapsed = 0f;
		}

		private void CalculateLossBar()
		{
			if (Mathf.Approximately(this.loss.fillAmount, this.BarTarget))
			{
				return;
			}
			this._damageTimeElapsed += Time.deltaTime;
			this.loss.fillAmount = Mathf.Lerp(this.loss.fillAmount, this.BarTarget, this.HealthLossAnimationCurve.Evaluate(this._damageTimeElapsed));
		}

		private void CalculateHealthBar()
		{
			if (Mathf.Approximately(this.health.fillAmount, this.BarTarget))
			{
				return;
			}
			this._damageTimeElapsed += Time.deltaTime;
			this.health.fillAmount = Mathf.Lerp(this.health.fillAmount, this.BarTarget, this._damageTimeElapsed * this.speed);
		}

		private void CalculateHealthBarSize()
		{
			Penitent penitent = Core.Logic.Penitent;
			if (penitent == null)
			{
				return;
			}
			float final = penitent.Stats.Life.Final;
			if (final != this.lastBarWidth)
			{
				this.lastBarWidth = final;
				float num = final - this.backgroundStartSize - this.endFillSize;
				num = ((num <= 0f) ? 0f : num);
				this.backgroundMid.sizeDelta = new Vector2(num, this.backgroundMid.sizeDelta.y);
				this.lossTransform.sizeDelta = new Vector2(final, this.lossTransform.sizeDelta.y);
				this.healthTransform.sizeDelta = new Vector2(final, this.healthTransform.sizeDelta.y);
				this.backgroundFillTransform.sizeDelta = new Vector2(final, this.healthTransform.sizeDelta.y);
			}
		}

		[SerializeField]
		private Image health;

		[SerializeField]
		private AnimationCurve HealthLossAnimationCurve;

		[SerializeField]
		private Image loss;

		[SerializeField]
		private RectTransform backgroundFillTransform;

		[SerializeField]
		[Range(0f, 10f)]
		private float speed;

		[SerializeField]
		private float backgroundStartSize = 60f;

		[SerializeField]
		private float endFillSize = 10f;

		[SerializeField]
		private RectTransform backgroundMid;

		private RectTransform healthTransform;

		private RectTransform lossTransform;

		private float _damageTimeElapsed;

		private float lastBarWidth = -1f;
	}
}
