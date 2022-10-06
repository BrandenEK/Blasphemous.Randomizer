using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class CurvedProjectile : Projectile
	{
		public int OriginalDamage
		{
			get
			{
				return this.originalDamage;
			}
			set
			{
				this.originalDamage = value;
			}
		}

		public virtual void Init(Vector3 origin, Vector3 target, float speed)
		{
			CurvedProjectile.<Init>c__AnonStorey0 <Init>c__AnonStorey = new CurvedProjectile.<Init>c__AnonStorey0();
			<Init>c__AnonStorey.target = target;
			<Init>c__AnonStorey.$this = this;
			this.speed = speed;
			CurvedProjectile.<Init>c__AnonStorey0 <Init>c__AnonStorey2 = <Init>c__AnonStorey;
			<Init>c__AnonStorey2.target.x = <Init>c__AnonStorey2.target.x + Random.Range(this.xDisplacementAtTarget.x, this.xDisplacementAtTarget.y);
			Vector2 vector = <Init>c__AnonStorey.target - origin;
			Vector2 vector2;
			vector2..ctor(origin.x + vector.x / 2f, Mathf.Max(<Init>c__AnonStorey.target.y, base.transform.position.y) + this.yDisplacementAtPeak);
			float num = Vector2.Distance(origin, vector2) + Vector2.Distance(vector2, <Init>c__AnonStorey.target);
			<Init>c__AnonStorey.time = num / speed;
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(base.transform, <Init>c__AnonStorey.target.x, <Init>c__AnonStorey.time, false), 1);
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, vector2.y, <Init>c__AnonStorey.time / 2f, false), 6), delegate()
			{
				TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(<Init>c__AnonStorey.$this.transform, <Init>c__AnonStorey.target.y, <Init>c__AnonStorey.time / 2f, false), 5);
			});
			this.lastPos = base.transform.position;
			if (!this.trailRendererCleaner)
			{
				this.trailRendererCleaner = base.GetComponentInChildren<ResetTrailRendererOnEnable>();
			}
			if (this.trailRendererCleaner)
			{
				this.trailRendererCleaner.Clean();
			}
		}

		protected override void OnUpdate()
		{
			base.UpdateOrientation();
			if (DOTween.IsTweening(base.transform, false))
			{
				this.velocity = (base.transform.position - this.lastPos).normalized * this.speed;
				this.lastPos = base.transform.position;
			}
			if (this.faceVelocityDirection)
			{
				float num = 57.29578f * Mathf.Atan2(this.velocity.y, this.velocity.x);
				base.transform.eulerAngles = new Vector3(0f, 0f, num);
			}
			if (!DOTween.IsTweening(base.transform, false))
			{
				base.transform.Translate(this.velocity * Time.deltaTime, 0);
				this.velocity.y = this.velocity.y * 1.1f;
			}
			this._currentTTL -= Time.deltaTime;
			if (this._currentTTL < 0f)
			{
				base.OnLifeEnded();
			}
		}

		[MinMaxSlider(-2f, 2f, true)]
		public Vector2 xDisplacementAtTarget;

		public float yDisplacementAtPeak = 4f;

		public bool faceVelocityDirection;

		protected int originalDamage;

		private Vector2 lastPos;

		private float speed;

		private ResetTrailRendererOnEnable trailRendererCleaner;
	}
}
