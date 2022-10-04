using System;
using System.Collections.Generic;
using Framework.FrameworkCore.Attributes;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class BossHealth : MonoBehaviour
	{
		private float BarTarget
		{
			get
			{
				Life life = this.target.Stats.Life;
				float num = life.Current;
				float final = life.Final;
				return num / final;
			}
		}

		private void Start()
		{
			this.canvasGroup = base.GetComponent<CanvasGroup>();
			this.Hide();
		}

		private void Update()
		{
			if (this.target == null)
			{
				return;
			}
			this.CalculateLossBar();
			this.CalculateHealthBar();
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
			this.health.fillAmount = Mathf.Lerp(this.health.fillAmount, this.BarTarget, this._damageTimeElapsed * this.ConsumptionSpeeed);
		}

		public void Show()
		{
			this.canvasGroup.alpha = 1f;
			this.canvasGroup.blocksRaycasts = true;
		}

		public void Hide()
		{
			this.canvasGroup.alpha = 0f;
			this.canvasGroup.blocksRaycasts = false;
			this.UnsubscribeToDamage();
		}

		public void SetName(string name)
		{
			this.text.text = name;
			this.textShadow.text = name;
		}

		public void SetTarget(GameObject entity)
		{
			this.target = entity.GetComponent<Entity>();
			this.SubscribeToDamage();
		}

		private void SubscribeToDamage()
		{
			if (this.target == null)
			{
				return;
			}
			List<EnemyDamageArea> list = new List<EnemyDamageArea>(this.target.GetComponentsInChildren<EnemyDamageArea>(true));
			list.ForEach(delegate(EnemyDamageArea x)
			{
				x.OnDamaged = (EnemyDamageArea.EnemyDamagedEvent)Delegate.Combine(x.OnDamaged, new EnemyDamageArea.EnemyDamagedEvent(this.OnDamaged));
			});
		}

		private void UnsubscribeToDamage()
		{
			if (this.target == null)
			{
				return;
			}
			List<EnemyDamageArea> list = new List<EnemyDamageArea>(this.target.GetComponentsInChildren<EnemyDamageArea>(true));
			list.ForEach(delegate(EnemyDamageArea x)
			{
				x.OnDamaged = (EnemyDamageArea.EnemyDamagedEvent)Delegate.Remove(x.OnDamaged, new EnemyDamageArea.EnemyDamagedEvent(this.OnDamaged));
			});
		}

		private void OnDamaged(GameObject damaged, Hit hit)
		{
			this._damageTimeElapsed = 0f;
		}

		public Text text;

		public Text textShadow;

		public Image loss;

		public Image health;

		public float ConsumptionSpeeed;

		private float _damageTimeElapsed;

		public AnimationCurve HealthLossAnimationCurve;

		private CanvasGroup canvasGroup;

		private Entity target;
	}
}
