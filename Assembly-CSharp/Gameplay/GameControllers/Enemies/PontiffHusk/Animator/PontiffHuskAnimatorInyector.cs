using System;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk.Animator
{
	public class PontiffHuskAnimatorInyector : EnemyAnimatorInyector
	{
		public bool IsFading { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			if (this.OwnerEntity is PontiffHuskRanged)
			{
				this._PontiffHuskRanged = (PontiffHuskRanged)this.OwnerEntity;
			}
			if (this.OwnerEntity is PontiffHuskMelee)
			{
				this._PontiffHuskMelee = (PontiffHuskMelee)this.OwnerEntity;
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.EntityAnimator = this.OwnerEntity.Animator;
			this._cameraManager = Core.Logic.CameraManager;
		}

		private void LateUpdate()
		{
			if (!this._healthBar)
			{
				this._healthBar = this.OwnerEntity.GetComponentInChildren<EnemyHealthBar>();
			}
			else
			{
				Enemy enemy = (Enemy)this.OwnerEntity;
				this._healthBar.transform.position = base.transform.position + enemy.healthOffset;
			}
		}

		public void Death(bool cut)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
			if (cut)
			{
				base.EntityAnimator.SetTrigger("CUT");
			}
		}

		public void PlayAppear()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("APPEAR");
		}

		public void Hurt()
		{
		}

		public void Ramming()
		{
			this._PontiffHuskMelee.Behaviour.Ramming();
		}

		public void ShootAnimationEvent()
		{
			this._PontiffHuskRanged.Behaviour.Shoot();
		}

		public void Attack()
		{
			if (base.EntityAnimator == null || !this.OwnerEntity.gameObject.activeSelf)
			{
				return;
			}
			base.EntityAnimator.SetBool("DASH", true);
		}

		public void StartShootingMines()
		{
			if (base.EntityAnimator == null || !this.OwnerEntity.gameObject.activeSelf)
			{
				return;
			}
			base.EntityAnimator.SetBool("SHOOT", true);
		}

		public void StopShootingMines()
		{
			if (base.EntityAnimator == null || !this.OwnerEntity.gameObject.activeSelf)
			{
				return;
			}
			base.EntityAnimator.SetBool("SHOOT", false);
		}

		public void StopAttack()
		{
			if (base.EntityAnimator == null || !this.OwnerEntity.gameObject.activeSelf)
			{
				return;
			}
			if (this._PontiffHuskMelee != null)
			{
				base.EntityAnimator.SetBool("DASH", false);
			}
		}

		public void Appear(float time)
		{
			if (this.IsFading)
			{
				return;
			}
			this.Fade(1f, time);
			this.PlayAppear();
		}

		public void Disappear(float time)
		{
			if (this.IsFading)
			{
				return;
			}
			this.Fade(0f, time);
		}

		public void Fade(float fadeValue, float time)
		{
			if (Math.Abs(this.SpriteRenderer.color.a - fadeValue) < Mathf.Epsilon)
			{
				return;
			}
			Tweener t = this.SpriteRenderer.DOFade(fadeValue, time).OnStart(new TweenCallback(this.OnFadeStart)).SetEase(Ease.InCirc);
			if (fadeValue > 0.5f)
			{
				t.OnComplete(new TweenCallback(this.OnFadeInCompleted));
			}
			else
			{
				t.OnComplete(new TweenCallback(this.OnFadeOutCompleted));
			}
		}

		public void OnFadeStart()
		{
			this.IsFading = true;
		}

		public void OnFadeInCompleted()
		{
			this.IsFading = false;
			if (this._PontiffHuskRanged != null)
			{
				this._PontiffHuskRanged.Behaviour.IsAppear = true;
			}
			if (this._PontiffHuskMelee != null)
			{
				this._PontiffHuskMelee.Behaviour.IsAppear = true;
			}
		}

		public void OnFadeOutCompleted()
		{
			this.IsFading = false;
			if (this._PontiffHuskRanged != null)
			{
				this._PontiffHuskRanged.Behaviour.IsAppear = false;
			}
			if (this._PontiffHuskMelee != null)
			{
				this._PontiffHuskMelee.Behaviour.IsAppear = false;
			}
		}

		public void PlayFiringProjectile()
		{
			if (this._PontiffHuskRanged == null)
			{
				return;
			}
			this._PontiffHuskRanged.Audio.PlayShoot();
		}

		private void OnDisable()
		{
			this.Fade(0f, 0f);
		}

		private PontiffHuskMelee _PontiffHuskMelee;

		private PontiffHuskRanged _PontiffHuskRanged;

		private CameraManager _cameraManager;

		public SpriteRenderer SpriteRenderer;

		private EnemyHealthBar _healthBar;
	}
}
