using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.AI
{
	public class NewFlagellantBehaviour : EnemyBehaviour
	{
		protected NewFlagellant NewFlagellant { get; set; }

		public bool IsDashBlocked { get; private set; }

		public float _currentHurtTime { get; private set; }

		public void UpdateHurtTime()
		{
			this._currentHurtTime += Time.deltaTime;
		}

		public void ResetHurtTime()
		{
			this._currentHurtTime = 0f;
		}

		private void UpdateRememberTime()
		{
			if (this.rememberTime > 0f)
			{
				this.rememberTime -= Time.deltaTime;
			}
		}

		public void ResetRememberTime()
		{
			this.rememberTime = this.maxPlayerRememberTime;
		}

		public bool StillRemembersPlayer()
		{
			return this.rememberTime > 0f;
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this.NewFlagellant = (NewFlagellant)this.Entity;
		}

		public override void OnStart()
		{
			base.OnStart();
			this._parryRecoverTime = 0f;
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnPlayerDead));
			this.NewFlagellant.StateMachine.SwitchState<NewFlagellantIdleState>();
		}

		public bool CanSeeTarget()
		{
			return this.NewFlagellant.Target != null && this.CanSeeTarget(this.NewFlagellant.Target.transform);
		}

		public bool CanSeeTarget(Transform t)
		{
			return this.NewFlagellant.VisionCone.CanSeeTarget(t, "Penitent", false);
		}

		public bool CanReachPlayer()
		{
			return this.NewFlagellant.Target != null && this.CanReachPosition(this.NewFlagellant.Target.transform.position);
		}

		public bool CanReachPosition(Vector2 p)
		{
			int num = 10;
			float num2 = 4f;
			if (Mathf.Abs(p.y - base.transform.position.y) > num2)
			{
				return false;
			}
			Vector2 a = base.transform.position + Vector2.up * 0.25f;
			Vector2 b = p;
			b.y = a.y;
			for (int i = 0; i < num; i++)
			{
				Vector2 pos = Vector2.Lerp(a, b, (float)i / (float)num);
				Vector2 vector;
				if (!this.NewFlagellant.MotionChecker.HitsFloorInPosition(pos, 1f, out vector, true))
				{
					return false;
				}
			}
			return true;
		}

		public bool CanAttack()
		{
			return this._attackCD <= 0f;
		}

		public bool IsTargetInsideAttackRange()
		{
			return this.NewFlagellant.DistanceToTarget < 3f;
		}

		private void CheckParryCounter()
		{
			if (this._parryRecoverTime > 0f)
			{
				this._parryRecoverTime -= Time.deltaTime;
			}
			else if (this._parryRecoverTime < 0f)
			{
				this._parryRecoverTime = 0f;
				this.NewFlagellant.AnimatorInyector.IsParried(false);
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantIdleState>();
			}
		}

		public void OnBouncedBackByOverlapping()
		{
		}

		private void LateUpdate()
		{
		}

		public void CheckFall()
		{
			Vector2 vector;
			bool flag = this.NewFlagellant.MotionChecker.HitsFloorInPosition(base.transform.position + Vector3.up * 0.2f + Vector3.right * 0.5f, 1f, out vector, false);
			if (!this.NewFlagellant.MotionChecker.HitsFloorInPosition(base.transform.position + Vector3.up * 0.2f - Vector3.right * 0.5f, 1f, out vector, false) && !flag)
			{
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantFallingState>();
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.CheckParryCounter();
			this.UpdateRememberTime();
			if (this._waitTimer > 0f)
			{
				this._waitTimer -= Time.deltaTime;
			}
			if (this._attackCD > 0f)
			{
				this._attackCD -= Time.deltaTime;
			}
			if (this.NewFlagellant.Target == null || this.NewFlagellant.IsAttacking)
			{
				return;
			}
			if (this.NewFlagellant.Status.Dead || base.GotParry || this._parryRecoverTime > 0f)
			{
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantDeathState>();
				return;
			}
		}

		public void ResetCooldown()
		{
			this._attackCD = this.attackCooldown;
		}

		public void Dash()
		{
			if (this.NewFlagellant.MotionLerper.IsLerping || this.IsDashBlocked)
			{
				return;
			}
			this.NewFlagellant.MotionLerper.distanceToMove = this.NewFlagellant.DistanceToTarget - 1f;
			Vector2 v = (this.NewFlagellant.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			this.NewFlagellant.MotionLerper.StartLerping(v);
		}

		public bool CanGoDownToReachPlayer()
		{
			bool result = false;
			Vector2 b = (this.NewFlagellant.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			Vector2 a = Core.Logic.Penitent.transform.position;
			if (Math.Sign((a - base.transform.position).x) != Math.Sign(b.x) || a.y > base.transform.position.y)
			{
				return false;
			}
			Vector2 pos = base.transform.position + b;
			Vector2 vector;
			if (this.NewFlagellant.MotionChecker.HitsFloorInPosition(pos, 2f, out vector, false))
			{
				result = true;
			}
			return result;
		}

		public void Chase()
		{
			if (this.IsWaiting())
			{
				this.StopMovement();
				this.NewFlagellant.AnimatorInyector.Run(false);
				return;
			}
			if (this.NewFlagellant.MotionChecker.HitsBlock || !this.NewFlagellant.MotionChecker.HitsFloor || !this.CanReachPlayer())
			{
				this.StartWait(0.5f);
				this.StopMovement();
				return;
			}
			if (this.NewFlagellant.MotionChecker.HitsPatrolBlock)
			{
				this.StartWait(0.5f);
				return;
			}
			this.NewFlagellant.AnimatorInyector.Run(true);
			this.NewFlagellant.SetMovementSpeed(this.NewFlagellant.MAX_SPEED);
			Vector2 vector = (this.NewFlagellant.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			this.NewFlagellant.Input.HorizontalInput = vector.x;
		}

		public bool IsWaiting()
		{
			return this._waitTimer > 0f;
		}

		private void StartWait(float seconds)
		{
			this._waitTimer = seconds;
		}

		public void AttackDisplacement()
		{
			Vector2 vector = (this.NewFlagellant.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			this.NewFlagellant.EntityDisplacement.Move(1.2f * vector.x, 0.25f, Ease.OutCubic);
		}

		public void Patrol()
		{
			if (this.NewFlagellant.MotionChecker.HitsBlock || !this.NewFlagellant.MotionChecker.HitsFloor || this.NewFlagellant.MotionChecker.HitsPatrolBlock)
			{
				this.ReverseOrientation();
			}
			this.NewFlagellant.SetMovementSpeed(this.NewFlagellant.MIN_SPEED);
			Vector2 vector = (this.NewFlagellant.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			this.NewFlagellant.Input.HorizontalInput = vector.x;
			this.NewFlagellant.AnimatorInyector.Walk(true);
		}

		private void OnPlayerDead()
		{
			this._targetIsDead = true;
		}

		public void ResetParryRecover()
		{
			this._parryRecoverTime = 1f;
		}

		public override void Parry()
		{
			base.Parry();
			this.NewFlagellant.IsAttacking = false;
			this.NewFlagellant.AnimatorInyector.IsParried(true);
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("Parry");
			this._parryRecoverTime = this.maxParryTime;
		}

		public override void Execution()
		{
			base.Execution();
			this.NewFlagellant.IsAttacking = false;
			base.GotParry = true;
			this.StopMovement();
			this.NewFlagellant.gameObject.layer = LayerMask.NameToLayer("Default");
			this.NewFlagellant.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.NewFlagellant.EntExecution.InstantiateExecution();
			if (this.NewFlagellant.EntExecution != null)
			{
				this.NewFlagellant.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this.NewFlagellant.gameObject.layer = LayerMask.NameToLayer("Enemy");
			base.GotParry = false;
			this.NewFlagellant.SpriteRenderer.enabled = true;
			this.NewFlagellant.Animator.SetBool("IS_PARRIED", false);
			this.NewFlagellant.Animator.Play("Idle");
			this.NewFlagellant.CurrentLife = this.NewFlagellant.Stats.Life.Base / 2f;
			if (this.NewFlagellant.EntExecution != null)
			{
				this.NewFlagellant.EntExecution.enabled = false;
			}
			this.NewFlagellant.StateMachine.SwitchState<NewFlagellantIdleState>();
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
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			if (!base.IsAttacking || this.flinchDuringAttacks)
			{
				this.Hurt();
			}
		}

		private void Hurt()
		{
			this.LookAtPenitent();
			this.NewFlagellant.AnimatorInyector.Hurt();
			this.NewFlagellant.StateMachine.SwitchState<NewFlagellantHurtState>();
			this.ResetHurtTime();
		}

		public void LookAtPenitent()
		{
			if (this.NewFlagellant.Target != null)
			{
				this.LookAtTarget(this.NewFlagellant.Target.transform.position);
			}
		}

		public override void StopMovement()
		{
			if (this.NewFlagellant.MotionLerper.IsLerping)
			{
				this.NewFlagellant.MotionLerper.StopLerping();
			}
			this.NewFlagellant.Input.HorizontalInput = 0f;
		}

		public float DashDistance;

		public float CloseRange = 2f;

		public float attackCooldown = 3f;

		private float _parryRecoverTime;

		private bool _targetIsDead;

		private float _attackCD;

		private float _waitTimer;

		public bool flinchDuringAttacks = true;

		public float maxParryTime = 2f;

		public float maxPlayerRememberTime = 2f;

		private float rememberTime;
	}
}
