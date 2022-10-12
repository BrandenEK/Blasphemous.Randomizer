using System;
using DG.Tweening;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class GuardSlide : Ability
	{
		public bool IsGuard { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			SpawnManager.OnPlayerSpawn += this.OnPenitentReady;
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPenitentReady;
			if (this._penitent)
			{
				MotionLerper motionLerper = this._penitent.MotionLerper;
				motionLerper.OnLerpStart = (Core.SimpleEvent)Delegate.Remove(motionLerper.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
				MotionLerper motionLerper2 = this._penitent.MotionLerper;
				motionLerper2.OnLerpStop = (Core.SimpleEvent)Delegate.Remove(motionLerper2.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			PoolManager.Instance.CreatePool(this.GuardSlideImpactEffect, 1);
		}

		private void OnPenitentReady(Penitent penitent)
		{
			this._penitent = penitent;
			MotionLerper motionLerper = this._penitent.MotionLerper;
			motionLerper.OnLerpStart = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
			MotionLerper motionLerper2 = this._penitent.MotionLerper;
			motionLerper2.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper2.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this._penitent || !this._penitent.Status.IsGrounded)
			{
				return;
			}
			if (!base.Casting)
			{
				return;
			}
			this._currentLapseBeforeDash += Time.deltaTime;
			this._timeLerpingRemaining -= Time.deltaTime;
			if (this._penitent.MotionLerper.IsLerping)
			{
				this.SetOwnerOrientation();
			}
			if (!this.CanSlide)
			{
				this._penitent.MotionLerper.StopLerping();
			}
			if (this._timeLerpingRemaining <= 0.35f && this.IsGuard && this._penitent.MotionLerper.IsLerping)
			{
				this.IsGuard = false;
				base.EntityOwner.Animator.Play(this._guardToIdleAnim);
			}
			if (this._penitent.PlatformCharacterInput.Rewired.GetButton(7) && base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("GuardToIdle"))
			{
				this.CastDash();
				this._penitent.MotionLerper.StopLerping();
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			this.IsGuard = true;
			this.ImpactGamePause();
			Vector3 position = base.EntityOwner.transform.position;
			PoolManager.Instance.ReuseObject(this.GuardSlideImpactEffect, position, Quaternion.identity, false, 1);
			this._penitent.Parry.StopCast();
			this.EnableParticleSystem(true);
			Core.Audio.EventOneShotPanned(this.GuardSlideSoundFx, position);
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			this._currentLapseBeforeDash = 0f;
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			base.EntityOwner.Animator.Play(this._guardToIdleAnim);
			this.EnableParticleSystem(false);
		}

		private void OnLerpStart()
		{
			if (base.Casting)
			{
				base.EntityOwner.Animator.Play(this._guardSlideAnim);
			}
		}

		private void OnLerpStop()
		{
			this.orientationBeingForced = false;
			base.StopCast();
		}

		private bool CanSlide
		{
			get
			{
				return !this._penitent.FloorChecker.IsSideBlocked && this._penitent.FloorChecker.IsGrounded;
			}
		}

		public void CastSlide(Hit hit)
		{
			if (base.Casting)
			{
				return;
			}
			if (hit.OnGuardCallback != null)
			{
				hit.OnGuardCallback(hit);
			}
			this.guardingHitPosition = hit.AttackingEntity.transform.position;
			if (hit.ForceGuardSlideDirection && hit.AttackingEntity.GetComponent<Entity>() != null)
			{
				this.SetOwnerOrientationByAttackingEntity(hit);
			}
			else
			{
				this.SetOwnerOrientation();
			}
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			this.SetLerping(hit);
			base.Cast();
		}

		private void CastDash()
		{
			base.EntityOwner.Animator.SetBool(GuardSlide.Dashing, true);
			base.EntityOwner.Animator.SetTrigger(GuardSlide.Dash);
			this._penitent.Dash.Cast();
		}

		private void SetOwnerOrientationByAttackingEntity(Hit hit)
		{
			this.orientationBeingForced = true;
			EntityOrientation orientation = hit.AttackingEntity.GetComponent<Entity>().Status.Orientation;
			this.forcedOrientation = ((orientation != EntityOrientation.Left) ? EntityOrientation.Left : EntityOrientation.Right);
			base.EntityOwner.SetOrientation(this.forcedOrientation, true, false);
		}

		private void SetOwnerOrientation()
		{
			if (this.orientationBeingForced)
			{
				base.EntityOwner.SetOrientation(this.forcedOrientation, true, false);
			}
			else
			{
				base.EntityOwner.SetOrientation((this.guardingHitPosition.x <= base.EntityOwner.transform.position.x) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
			}
		}

		private void SetLerping(Hit hit)
		{
			if (!this._penitent.MotionLerper)
			{
				return;
			}
			this._penitent.MotionLerper.distanceToMove = hit.Force * this.DistanceFactorByImpactForce;
			this._penitent.MotionLerper.speedCurve = this.SlideAnimationCurve;
			this._penitent.MotionLerper.TimeTakenDuringLerp = this.TimeTakenDuringLerp;
		}

		private void Slide()
		{
			if (!this._penitent.MotionLerper)
			{
				return;
			}
			Vector2 v = Vector2.right;
			if ((this.orientationBeingForced && this.forcedOrientation == EntityOrientation.Right) || (!this.orientationBeingForced && this.guardingHitPosition.x > base.EntityOwner.transform.position.x))
			{
				v = Vector2.left;
			}
			if (this._penitent.SlopeAngle >= 5f)
			{
				v.y += 0.25f;
			}
			else if (this._penitent.SlopeAngle <= -5f)
			{
				v.y -= 0.25f;
			}
			this._penitent.MotionLerper.StartLerping(v);
		}

		private void EnableParticleSystem(bool enableParticleSystem = true)
		{
			Core.Logic.Penitent.ParticleSystem.emission.enabled = enableParticleSystem;
			if (enableParticleSystem)
			{
				Core.Logic.Penitent.ParticleSystem.Play();
			}
			else
			{
				Core.Logic.Penitent.ParticleSystem.Stop();
			}
		}

		private void ImpactGamePause()
		{
			DOTween.Sequence().SetDelay(this.HitLapse).OnStart(delegate
			{
				Core.Logic.CurrentLevelConfig.sleepTime = this.HitLapse;
				Core.Logic.CurrentLevelConfig.SleepTime();
			}).OnComplete(delegate
			{
				this._currentLapseBeforeDash = 0f;
				this._timeLerpingRemaining = this.TimeTakenDuringLerp;
				this.Slide();
			});
		}

		private Penitent _penitent;

		private readonly int _guardToIdleAnim = Animator.StringToHash("GuardToIdle");

		private readonly int _guardSlideAnim = Animator.StringToHash("GuardSlide");

		[FoldoutGroup("Slide Settings", 0)]
		public AnimationCurve SlideAnimationCurve;

		[FoldoutGroup("Slide Settings", 0)]
		public float LapseBeforeDash;

		[FoldoutGroup("Slide Settings", 0)]
		public float DistanceFactorByImpactForce;

		[FoldoutGroup("Slide Settings", 0)]
		public float HitLapse;

		[FoldoutGroup("Slide Settings", 0)]
		public float TimeTakenDuringLerp = 0.5f;

		[FoldoutGroup("Slide Settings", 0)]
		public GameObject GuardSlideImpactEffect;

		[FoldoutGroup("Audio Settings", 0)]
		[EventRef]
		public string GuardSlideSoundFx;

		private Vector3 guardingHitPosition;

		public const float GuardToIdleDuration = 0.35f;

		private float _currentLapseBeforeDash;

		private float _timeLerpingRemaining;

		private static readonly int Dashing = Animator.StringToHash("DASHING");

		private static readonly int Dash = Animator.StringToHash("DASH");

		private bool orientationBeingForced;

		private EntityOrientation forcedOrientation;
	}
}
