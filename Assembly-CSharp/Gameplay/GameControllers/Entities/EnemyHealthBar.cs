using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class EnemyHealthBar : PoolObject
	{
		public bool IsEnabled { get; private set; }

		public void UpdateParent(Enemy parent)
		{
			this.Owner = parent;
			this.barRoot.gameObject.SetActive(false);
			this.Owner.OnDamaged += this.OnDamaged;
			this.Owner.OnDeath += this.OnDead;
			this.leftOffset = new Vector2(-this.Owner.healthOffset.x, this.Owner.healthOffset.y);
			this.rightOffset = this.Owner.healthOffset;
			this.UpdateOffset();
		}

		private void UpdateOffset()
		{
			this.currentOrientation = this.Owner.Status.Orientation;
			base.transform.localPosition = ((this.currentOrientation != EntityOrientation.Left) ? this.rightOffset : this.leftOffset);
		}

		private void Awake()
		{
			this.IsEnabled = false;
			this.SetBarSizeAndPos(this.instantBar, this.MaxSizeBar);
			this.SetBarSizeAndPos(this.animatedBar, this.MaxSizeBar);
			this.currentAnimated = this.MaxSizeBar;
			this.backgroundSprite = this.barRoot.GetComponent<SpriteRenderer>();
			this.instantBarSprite = this.instantBar.GetComponent<SpriteRenderer>();
			this.animatedBarSprite = this.animatedBar.GetComponent<SpriteRenderer>();
			this.SpritesAlpha = 0f;
		}

		private void Update()
		{
			if (this.IsEnabled || (this.tweenOut != null && TweenExtensions.IsActive(this.tweenOut)))
			{
				if (this.tweenOut == null || !TweenExtensions.IsActive(this.tweenOut))
				{
					this.SpritesAlpha = this.Owner.SpriteRenderer.color.a;
				}
				if (this.currentOrientation != this.Owner.Status.Orientation)
				{
					this.UpdateOffset();
				}
				int num = Mathf.RoundToInt(this.Owner.Stats.Life.Current / this.Owner.Stats.Life.Final * (float)this.MaxSizeBar);
				this.SetBarSizeAndPos(this.instantBar, num);
				if (this.currentAnimated != num)
				{
					this._damageTimeElapsed += Time.deltaTime;
					this.currentAnimated = Mathf.RoundToInt(Mathf.Lerp((float)this.currentAnimated, (float)num, this.HealthLossAnimationCurve.Evaluate(this._damageTimeElapsed)));
					this.SetBarSizeAndPos(this.animatedBar, this.currentAnimated);
				}
			}
		}

		private void OnDamaged()
		{
			if (!Core.Events.GetFlag("SHOW_ENEMY_BAR"))
			{
				return;
			}
			if (!this.Owner.UseHealthBar)
			{
				this.barRoot.gameObject.SetActive(false);
				return;
			}
			if (!this.IsEnabled)
			{
				this.UpdateOffset();
				DOTween.To(new DOGetter<float>(this.get_SpritesAlpha), delegate(float x)
				{
					this.SpritesAlpha = x;
				}, 1f, this.FadeInTime);
			}
			this.IsEnabled = true;
			this._damageTimeElapsed = 0f;
			this.barRoot.gameObject.SetActive(true);
		}

		private void OnDead()
		{
			this.IsEnabled = false;
			this.tweenOut = DOTween.To(new DOGetter<float>(this.get_SpritesAlpha), delegate(float x)
			{
				this.SpritesAlpha = x;
			}, 0f, this.FadeOutTime);
			this.Owner.OnDamaged -= this.OnDamaged;
			this.Owner.OnDeath -= this.OnDead;
		}

		private void SetBarSizeAndPos(Transform bar, int size)
		{
			bar.localScale = new Vector3((float)size, 1f, 1f);
			bar.localPosition = new Vector3((float)(this.MaxSizeBar - size) * this.PositionStep, 0f, 0f);
		}

		public float SpritesAlpha
		{
			get
			{
				return this._spritesAlpha;
			}
			set
			{
				this._spritesAlpha = value;
				Color color;
				color..ctor(1f, 1f, 1f, this._spritesAlpha);
				this.backgroundSprite.color = color;
				this.instantBarSprite.color = color;
				this.animatedBarSprite.color = color;
			}
		}

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private AnimationCurve HealthLossAnimationCurve;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private int MaxSizeBar = 16;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float PositionStep = -0.03125f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float FadeInTime = 0.2f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float FadeOutTime = 0.5f;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private GameObject barRoot;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Transform instantBar;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Transform animatedBar;

		private int currentAnimated;

		private float _damageTimeElapsed;

		private SpriteRenderer backgroundSprite;

		private SpriteRenderer instantBarSprite;

		private SpriteRenderer animatedBarSprite;

		private TweenerCore<float, float, FloatOptions> tweenOut;

		private const string FLAG_ID = "SHOW_ENEMY_BAR";

		private Enemy Owner;

		private EntityOrientation currentOrientation;

		private Vector2 leftOffset = new Vector2(0f, 2f);

		private Vector2 rightOffset = new Vector2(0f, 2f);

		private float _spritesAlpha;
	}
}
