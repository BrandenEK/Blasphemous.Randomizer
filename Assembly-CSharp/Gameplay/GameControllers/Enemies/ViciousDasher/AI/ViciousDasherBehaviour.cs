using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ViciousDasher.AI
{
	public class ViciousDasherBehaviour : EnemyBehaviour
	{
		protected ViciousDasher ViciousDasher { get; set; }

		public bool IsDashBlocked { get; private set; }

		public bool CanBeExecuted { get; private set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.ViciousDasher = (ViciousDasher)this.Entity;
		}

		public override void OnStart()
		{
			base.OnStart();
			this._parryRecoverTime = 0f;
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnPlayerDead));
		}

		private void LateUpdate()
		{
			this.IsDashBlocked = (!this.ViciousDasher.MotionChecker.HitsFloor || this.ViciousDasher.MotionChecker.HitsBlock);
			if (this.IsDashBlocked)
			{
				this.ViciousDasher.MotionLerper.StopLerping();
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this._parryRecoverTime -= Time.deltaTime;
			if (this.ViciousDasher.Target == null || this.ViciousDasher.IsAttacking)
			{
				return;
			}
			if (this.ViciousDasher.Status.Dead || base.GotParry || this._parryRecoverTime > 0f)
			{
				this.ViciousDasher.StateMachine.SwitchState<ViciousDasherDeathState>();
				return;
			}
			if (this.ViciousDasher.IsTargetVisible)
			{
				this.LookAtTarget(this.ViciousDasher.Target.transform.position);
				if (this.ViciousDasher.DistanceToTarget < this.DashDistance && !this._targetIsDead)
				{
					this.ViciousDasher.StateMachine.SwitchState<ViciousDasherAttackState>();
				}
			}
			else
			{
				this.ViciousDasher.StateMachine.SwitchState<ViciousDasherIdleState>();
			}
		}

		public void Dash()
		{
			if (this.ViciousDasher.MotionLerper.IsLerping || this.IsDashBlocked)
			{
				return;
			}
			this.ViciousDasher.MotionLerper.distanceToMove = this.ViciousDasher.DistanceToTarget - 1f;
			Vector2 v = (this.ViciousDasher.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			this.ViciousDasher.MotionLerper.StartLerping(v);
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
			base.GotParry = true;
			this.ViciousDasher.IsAttacking = false;
			this.ViciousDasher.AnimatorInjector.IsParried(true);
		}

		public override void Execution()
		{
			base.Execution();
			base.GotParry = true;
			this.CanBeExecuted = true;
			this.StopMovement();
			this.ViciousDasher.AnimatorInjector.StopAttack();
			this.ViciousDasher.gameObject.layer = LayerMask.NameToLayer("Default");
			this.ViciousDasher.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.ViciousDasher.EntExecution.InstantiateExecution();
			if (this.ViciousDasher.EntExecution != null)
			{
				this.ViciousDasher.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this.ViciousDasher.gameObject.layer = LayerMask.NameToLayer("Enemy");
			base.GotParry = false;
			this.CanBeExecuted = false;
			this.ViciousDasher.SpriteRenderer.enabled = true;
			this.ViciousDasher.Animator.SetBool(ViciousDasherBehaviour.IsParried, false);
			this.ViciousDasher.Animator.Play("Idle");
			this.ViciousDasher.CurrentLife = this.ViciousDasher.Stats.Life.Base / 2f;
			if (this.ViciousDasher.EntExecution != null)
			{
				this.ViciousDasher.EntExecution.enabled = false;
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
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			if (this.ViciousDasher.MotionLerper.IsLerping)
			{
				this.ViciousDasher.MotionLerper.StopLerping();
			}
		}

		public float DashDistance;

		public float CloseRange = 2f;

		public float AttackTime = 3f;

		private float _parryRecoverTime;

		private bool _targetIsDead;

		private static readonly int IsParried = Animator.StringToHash("IS_PARRIED");
	}
}
