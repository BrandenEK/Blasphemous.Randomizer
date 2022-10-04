using System;
using System.Collections;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.GhostKnight.Animator;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GhostKnight.AI
{
	public class GhostKnightBehaviour : EnemyBehaviour
	{
		public bool GotCloseToPlayer { get; set; }

		public bool GotRamp { get; set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this._ghostKnight = (GhostKnight)this.Entity;
			this._animatorInyector = this._ghostKnight.GetComponentInChildren<GhostKnightAnimatorInyector>();
		}

		public override void OnStart()
		{
			base.OnStart();
			MotionLerper motionLerper = this._ghostKnight.MotionLerper;
			motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			MotionLerper motionLerper2 = this._ghostKnight.MotionLerper;
			motionLerper2.OnLerpStart = (Core.SimpleEvent)Delegate.Combine(motionLerper2.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
			this._ghostKnight.OnDeath += this.GhostKnightOnDeath;
			Core.Logic.Penitent.OnDeath += this.OnDeathPlayer;
		}

		public bool AwakeAttackBehaviour()
		{
			if (this._ghostKnight == null)
			{
				return false;
			}
			bool result = false;
			if (this._ghostKnight.DistanceToTarget <= this._ghostKnight.ActivationRange)
			{
				this._elapsedTimeBeforeStalk += Time.deltaTime;
				result = (this._elapsedTimeBeforeStalk >= this.AwaitTimeBeforeStalk);
			}
			else
			{
				this._elapsedTimeBeforeStalk = 0f;
			}
			return result;
		}

		public void GetCloseToPlayer()
		{
			if (this._ghostKnight == null || this._transitioning)
			{
				return;
			}
			if (this.GotCloseToPlayer)
			{
				return;
			}
			this.GotCloseToPlayer = true;
			Vector3 position = this._ghostKnight.Target.transform.position;
			float y = position.y + -1f;
			float horizontalRndPositionBeforeAttack = this.GetHorizontalRndPositionBeforeAttack(position);
			this._ghostKnight.transform.position = new Vector2(horizontalRndPositionBeforeAttack, y);
			this.Appear(this.TimeBecomeVisible);
		}

		public void Appear(float time)
		{
			if (this._transitioning)
			{
				return;
			}
			this._animatorInyector.Appear();
			this._ghostKnight.SpriteRenderer.DOFade(1f, time).OnComplete(new TweenCallback(this.BecomeVisible)).OnUpdate(new TweenCallback(this.OnTransition));
			this._ghostKnight.Audio.Appear();
		}

		public void Disappear(float time)
		{
			if (this._transitioning)
			{
				return;
			}
			if (time <= 0f)
			{
				this.SetSpriteInvisible();
			}
			this._ghostKnight.SpriteRenderer.DOFade(0f, time).OnComplete(new TweenCallback(this.BecomeInvisible)).OnUpdate(new TweenCallback(this.OnTransition));
			this._ghostKnight.Audio.Disappear();
		}

		private void SetSpriteInvisible()
		{
			Color white = Color.white;
			white.a = 0f;
			this._ghostKnight.SpriteRenderer.color = white;
		}

		private void BecomeVisible()
		{
			this._transitioning = false;
			this.EnableDamageArea(true);
			this._animatorInyector.AttackClue();
		}

		private void BecomeInvisible()
		{
			this._transitioning = false;
			this.EnableDamageArea(false);
			this.BackToOrigin();
			this._ghostKnight.Status.IsHurt = false;
			base.GotParry = false;
			this._ghostKnight.Audio.StopAttack();
		}

		private void OnTransition()
		{
			this.EnableDamageArea(this._ghostKnight.SpriteRenderer.color.a > 0.5f);
			this._transitioning = true;
		}

		private void OnLerpStart()
		{
			if (this._ghostKnight.Status.Dead)
			{
				return;
			}
			this._ghostKnight.Audio.Attack();
			this._ghostKnight.GhostSprites.EnableGhostTrail = true;
		}

		private void OnLerpStop()
		{
			this.Entity.IsAttacking = false;
			if (this._ghostKnight.Status.Dead)
			{
				return;
			}
			if (!base.GotParry)
			{
				this._animatorInyector.AttackToIdle();
				this.Disappear(this.TimeBecomeInVisible);
			}
			this._ghostKnight.GhostSprites.EnableGhostTrail = false;
		}

		private void BackToOrigin()
		{
			this._ghostKnight.transform.position = this._ghostKnight.StartPoint;
			this.GotCloseToPlayer = false;
			this.GotRamp = false;
			this._elapsedTimeBeforeStalk = 0f;
		}

		private float GetHorizontalRndPositionBeforeAttack(Vector3 targetPos)
		{
			float value = UnityEngine.Random.value;
			return (value < 0.5f) ? (targetPos.x - this.DistanceToPlayerBeforeAttack) : (targetPos.x + this.DistanceToPlayerBeforeAttack);
		}

		private void EnableDamageArea(bool isEnabled)
		{
			this._ghostKnight.EntityDamageArea.DamageAreaCollider.enabled = isEnabled;
		}

		public Vector2 GetDamageFallbackPosition
		{
			get
			{
				Vector3 position = this._ghostKnight.transform.position;
				float x = (this._ghostKnight.Status.Orientation != EntityOrientation.Left) ? (position.x - this.FallbackDisplacement) : (position.x + this.FallbackDisplacement);
				return new Vector2(x, this._ghostKnight.transform.position.y);
			}
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			if (this._ghostKnight == null)
			{
				return;
			}
			if (this.GotRamp)
			{
				return;
			}
			this.GotRamp = true;
			float d = (this._ghostKnight.Status.Orientation != EntityOrientation.Left) ? 1f : -1f;
			MotionLerper motionLerper = this._ghostKnight.MotionLerper;
			motionLerper.StartLerping(Vector2.right * d);
			this.Entity.IsAttacking = true;
		}

		public override void StopMovement()
		{
			if (base.GotParry)
			{
				return;
			}
			if (this._ghostKnight.MotionLerper.IsLerping)
			{
				this._ghostKnight.MotionLerper.StopLerping();
			}
		}

		public override void Damage()
		{
			this._animatorInyector.ColorFlash(Color.red);
			this._animatorInyector.EntityAnimator.speed = 1f;
			if (base.GotParry)
			{
				this.DamageDisplacement(this.FallbackDisplacement, 1f);
				this._animatorInyector.Damage();
				this._ghostKnight.Audio.StopAttack();
				if (!this._ghostKnight.Status.Dead)
				{
					this._ghostKnight.Audio.Damage();
				}
			}
			bool dead = this._ghostKnight.Status.Dead;
			if (dead)
			{
				this._ghostKnight.MotionLerper.StopLerping();
			}
			this.EnableDamageArea(false);
		}

		public void DamageDisplacement(float displacement, float time)
		{
			this.StopMovement();
			DOTween.Kill(base.transform, false);
			float num = (this._ghostKnight.Status.Orientation != EntityOrientation.Right) ? 1f : -1f;
			this._ghostKnight.transform.DOMoveX(base.transform.position.x + displacement * num, time, false).SetEase(Ease.OutSine);
		}

		public override void Parry()
		{
			base.Parry();
			base.GotParry = true;
			this._ghostKnight.Status.IsHurt = true;
			this.DamageDisplacement(this.FallbackDisplacement * 0.5f, 3f);
			this._ghostKnight.MotionLerper.StopLerping();
			this._animatorInyector.ParryReaction();
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("Parry");
		}

		public override void Alive()
		{
			base.Alive();
			base.GotParry = false;
			this._ghostKnight.Status.IsHurt = false;
			this._ghostKnight.Animator.SetTrigger("HURT");
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this._transitioning)
			{
				return;
			}
			base.LookAtTarget(targetPos);
		}

		private void GhostKnightOnDeath()
		{
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
			this._ghostKnight.Audio.Death();
			this._ghostKnight.StartCoroutine(this.CallAnimatorInyectorDeath());
		}

		private IEnumerator CallAnimatorInyectorDeath()
		{
			for (;;)
			{
				this._animatorInyector.Death();
				yield return new WaitForSeconds(1f);
			}
			yield break;
		}

		private void OnDeathPlayer()
		{
			base.BehaviourTree.StopBehaviour();
		}

		private const float YPosOffset = -1f;

		private GhostKnightAnimatorInyector _animatorInyector;

		private float _elapsedTimeBeforeStalk;

		private GhostKnight _ghostKnight;

		private bool _transitioning;

		public float AwaitTimeBeforeAttack = 0.2f;

		public float AwaitTimeBeforeStalk = 2f;

		public float DistanceToPlayerBeforeAttack = 5f;

		public float FallbackDisplacement = 2f;

		public float TimeBecomeInVisible = 0.5f;

		public float TimeBecomeVisible = 0.5f;
	}
}
