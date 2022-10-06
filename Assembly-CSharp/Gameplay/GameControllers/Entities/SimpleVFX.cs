using System;
using DG.Tweening;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Pooling;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class SimpleVFX : PoolObject
	{
		private void Awake()
		{
			this._animator = base.GetComponent<Animator>();
			this._spriteRenderers = base.GetComponentsInChildren<SpriteRenderer>();
			if (this._spriteRenderers.Length > 0)
			{
				this.originAlpha = this._spriteRenderers[0].color.a;
			}
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			if (this._spriteRenderers.Length > 0 && this.fadeWithAlpha)
			{
				foreach (SpriteRenderer spriteRenderer in this._spriteRenderers)
				{
					Color color = spriteRenderer.color;
					color..ctor(color.r, color.g, color.b, this.originAlpha);
					spriteRenderer.color = color;
				}
			}
			if (this._animator && this.setTriggerOnReuse)
			{
				this._animator.SetTrigger(this.trigger);
			}
			if (this.snapToGround)
			{
				this._bottomHits = new RaycastHit2D[1];
				this.SnapToGround();
			}
			this.currentTTL = this.maxTTL;
			if (this.FlipByPlayerOrientation)
			{
				this.SetOrientationByPlayer();
			}
			this.PlayExplosionFx();
		}

		private void Update()
		{
			this.currentTTL -= Time.deltaTime;
			if (this.currentTTL < 0f)
			{
				base.Destroy();
				return;
			}
			if (this._spriteRenderers.Length > 0 && this.currentTTL < this.alphaTime && this.fadeWithAlpha && (this.fadeTween == null || !TweenExtensions.IsPlaying(this.fadeTween)))
			{
				foreach (SpriteRenderer spriteRenderer in this._spriteRenderers)
				{
					this.fadeTween = ShortcutExtensions43.DOFade(spriteRenderer, 0f, this.currentTTL);
				}
			}
		}

		protected void SetOrientationByPlayer()
		{
			this.SetOrientation(Core.Logic.Penitent.GetOrientation(), this.useReversedPlayerOrientation);
		}

		public void SetOrientation(EntityOrientation orientation, bool reverse = false)
		{
			EntityOrientation entityOrientation = (!reverse) ? EntityOrientation.Left : EntityOrientation.Right;
			if (this._spriteRenderers.Length > 0)
			{
				if (this.useScaleToFlip)
				{
					base.transform.localScale = new Vector3((float)((orientation != entityOrientation) ? 1 : -1), 1f, 1f);
				}
				else
				{
					foreach (SpriteRenderer spriteRenderer in this._spriteRenderers)
					{
						spriteRenderer.flipX = (orientation == entityOrientation);
					}
				}
			}
			else
			{
				Debug.LogWarning("FlipX shouldn't be activated if the SimpleVFX doesn't have a SpriteRenderer controller");
			}
		}

		private void SnapToGround()
		{
			Vector2 vector = base.transform.position;
			bool flag = Physics2D.LinecastNonAlloc(vector, vector + Vector2.down * this.rangeGroundDetection, this._bottomHits, this.groundMask) > 0;
			if (flag)
			{
				base.transform.position += Vector3.down * this._bottomHits[0].distance;
			}
		}

		public void SetMaxTTL(float seconds)
		{
			this.maxTTL = seconds;
			this.currentTTL = seconds;
		}

		private void PlayExplosionFx()
		{
			if (!string.IsNullOrEmpty(this.fxOneshotSound))
			{
				Core.Audio.PlayOneShot(this.fxOneshotSound, default(Vector3));
			}
		}

		public float maxTTL;

		private float currentTTL;

		[FoldoutGroup("Snap settings", false, 0)]
		public bool snapToGround;

		[ShowIf("snapToGround", true)]
		[FoldoutGroup("Snap settings", false, 0)]
		public LayerMask groundMask;

		[ShowIf("snapToGround", true)]
		[FoldoutGroup("Snap settings", false, 0)]
		public float rangeGroundDetection = 10f;

		public bool setTriggerOnReuse;

		[FoldoutGroup("Orientation settings", false, 0)]
		public bool FlipByPlayerOrientation;

		[ShowIf("FlipByPlayerOrientation", true)]
		[FoldoutGroup("Orientation settings", false, 0)]
		public bool useReversedPlayerOrientation;

		[ShowIf("FlipByPlayerOrientation", true)]
		[FoldoutGroup("Orientation settings", false, 0)]
		public bool useScaleToFlip;

		public string trigger;

		[EventRef]
		public string fxOneshotSound;

		private RaycastHit2D[] _bottomHits;

		private Animator _animator;

		private Tween fadeTween;

		protected SpriteRenderer[] _spriteRenderers;

		public bool fadeWithAlpha;

		public float alphaTime = 1f;

		private float originAlpha;
	}
}
