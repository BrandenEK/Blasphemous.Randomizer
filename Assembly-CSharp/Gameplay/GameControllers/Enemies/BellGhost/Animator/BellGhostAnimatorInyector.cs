using System;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Enemies.BellGhost.AI;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellGhost.Animator
{
	public class BellGhostAnimatorInyector : EnemyAnimatorInyector
	{
		public bool IsFading { get; private set; }

		public bool IsTurning
		{
			get
			{
				if (base.EntityAnimator == null)
				{
					base.EntityAnimator = this.OwnerEntity.Animator;
				}
				return this.OwnerEntity.gameObject.activeSelf && base.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("TurnAround");
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this._bellGhost = (BellGhost)this.OwnerEntity;
			BellGhostBehaviour component = this._bellGhost.GetComponent<BellGhostBehaviour>();
			component.OnTurning = (Core.SimpleEvent)Delegate.Combine(component.OnTurning, new Core.SimpleEvent(this.OnTurning));
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
				this._healthBar = this._bellGhost.GetComponentInChildren<EnemyHealthBar>();
			}
			else
			{
				this._healthBar.transform.position = base.transform.position + this._bellGhost.healthOffset;
			}
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void Hurt()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("HURT");
			this._cameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
		}

		public void ShootAnimationEvent()
		{
			this._bellGhost.Behaviour.Shoot();
		}

		public void Attack()
		{
			if (base.EntityAnimator == null || !this.OwnerEntity.gameObject.activeSelf)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("ATTACK");
		}

		private void OnTurning()
		{
			if (base.EntityAnimator == null)
			{
				base.EntityAnimator = this.OwnerEntity.Animator;
			}
			if (!this._bellGhost.Behaviour.TurningAround && this.OwnerEntity.gameObject.activeSelf)
			{
				base.EntityAnimator.Play(this._turnAroundAnim);
			}
		}

		public void Appear(float time)
		{
			if (this.IsFading)
			{
				return;
			}
			this.Fade(1f, time);
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
			this.SpriteRenderer.DOFade(fadeValue, time).OnStart(new TweenCallback(this.OnFadeStart)).OnComplete(new TweenCallback(this.OnFadeCompleted));
		}

		public void OnFadeStart()
		{
			this.IsFading = true;
		}

		public void OnFadeCompleted()
		{
			this.IsFading = false;
		}

		public void PlayFiringProjectile()
		{
			if (this._bellGhost == null)
			{
				return;
			}
			this._bellGhost.Audio.PlayShoot();
		}

		private void OnDisable()
		{
			this.Fade(0f, 0f);
		}

		private void OnDestroy()
		{
			BellGhostBehaviour component = this._bellGhost.GetComponent<BellGhostBehaviour>();
			if (component)
			{
				BellGhostBehaviour bellGhostBehaviour = component;
				bellGhostBehaviour.OnTurning = (Core.SimpleEvent)Delegate.Remove(bellGhostBehaviour.OnTurning, new Core.SimpleEvent(this.OnTurning));
			}
		}

		private readonly int _turnAroundAnim = Animator.StringToHash("TurnAround");

		private BellGhost _bellGhost;

		private CameraManager _cameraManager;

		public SpriteRenderer SpriteRenderer;

		private EnemyHealthBar _healthBar;
	}
}
