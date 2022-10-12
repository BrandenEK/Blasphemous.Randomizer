using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Bishop.AI
{
	public class BishopBehaviour : EnemyBehaviour
	{
		public bool IsExecuted
		{
			get
			{
				return this.isExecuted;
			}
		}

		public Bishop Bishop { get; private set; }

		public bool RequestTurning { get; private set; }

		public bool IsBlock
		{
			get
			{
				return !(this.Bishop == null) && (this.Bishop.MotionChecker.HitsBlock || !this.Bishop.MotionChecker.HitsFloor);
			}
		}

		public bool CanTurn
		{
			get
			{
				return this._currentWanderingTime >= this.WanderingTime;
			}
		}

		public override void OnStart()
		{
			base.OnStart();
			this.Bishop = (Bishop)this.Entity;
			this.Bishop.OnDeath += this.OnDeath;
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			if (!this.Bishop)
			{
				return;
			}
			if (base.IsTurningAround())
			{
				this.StopMovement();
				return;
			}
			this.Bishop.AnimatorInyector.Chasing(false);
			if (!this._isMinSpeed)
			{
				this._isMinSpeed = true;
				this._isMaxSpeed = false;
				DOTween.To(delegate(float x)
				{
					this.Bishop.Controller.MaxWalkingSpeed = x;
				}, this.Bishop.Controller.MaxWalkingSpeed, this.MinSpeed, 1f);
			}
			if (base.IsChasing)
			{
				base.IsChasing = false;
			}
			this._currentWanderingTime += Time.deltaTime;
			float horizontalInput = (this.Bishop.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.Bishop.Inputs.HorizontalInput = horizontalInput;
		}

		public override void Chase(Transform targetPosition)
		{
			if (!this.Bishop || this.Bishop.Status.Dead)
			{
				this.StopMovement();
				return;
			}
			if (this.IsBlock)
			{
				this.Bishop.Controller.MaxWalkingSpeed = 0f;
				this.Bishop.AnimatorInyector.Idle();
				return;
			}
			this.Bishop.AnimatorInyector.Chasing(true);
			if (!this._isMaxSpeed)
			{
				this._isMaxSpeed = true;
				this._isMinSpeed = false;
				DOTween.To(delegate(float x)
				{
					this.Bishop.Controller.MaxWalkingSpeed = x;
				}, this.Bishop.Controller.MaxWalkingSpeed, this.MaxSpeed, UnityEngine.Random.value * 5f + 0.5f);
			}
			if (!base.IsChasing)
			{
				base.IsChasing = true;
			}
			float horizontalInput = (this.Bishop.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.Bishop.Inputs.HorizontalInput = horizontalInput;
		}

		public override void Attack()
		{
			this.Bishop.AnimatorInyector.Chasing(false);
			if (this.RequestTurning)
			{
				this.TurnAround();
			}
			else
			{
				if (base.TurningAround || this.isExecuted)
				{
					return;
				}
				this.Bishop.AnimatorInyector.Attack();
			}
		}

		public override void Damage()
		{
			base.PauseBehaviour();
			this.StopMovement();
			this.Bishop.AnimatorInyector.Damage();
		}

		public override void StopMovement()
		{
			this.Bishop.Inputs.HorizontalInput = 0f;
			this.Bishop.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			this.Bishop.Controller.MaxWalkingSpeed = 0f;
			this._isMaxSpeed = (this._isMinSpeed = false);
		}

		public void HitDisplacement(Vector3 attakingEntityPos)
		{
			float num = (this.Entity.transform.position.x < attakingEntityPos.x) ? (-this.HurtDisplacement) : this.HurtDisplacement;
			this.Bishop.transform.DOMoveX(this.Bishop.transform.position.x + num, 0.55f, false);
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			this.RequestTurning = false;
			if (this.Entity.transform.position.x >= targetPos.x)
			{
				if (this.Entity.Status.Orientation != EntityOrientation.Left)
				{
					this.RequestTurning = true;
					this.TurnAround();
				}
			}
			else if (this.Entity.transform.position.x < targetPos.x && this.Entity.Status.Orientation != EntityOrientation.Right)
			{
				this.RequestTurning = true;
				this.TurnAround();
			}
		}

		public void TurnAround()
		{
			this._currentWanderingTime = 0f;
			this.StopMovement();
			this.Bishop.AnimatorInyector.TurnAround();
		}

		public override void Execution()
		{
			base.Execution();
			this.isExecuted = true;
			this.Bishop.gameObject.layer = LayerMask.NameToLayer("Default");
			this.Bishop.Audio.StopAll();
			this.Bishop.Animator.Play("Idle");
			this.StopMovement();
			base.StopBehaviour();
			this.Bishop.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.Bishop.Attack.enabled = false;
			this.Bishop.EntExecution.InstantiateExecution();
			if (this.Bishop.EntExecution != null)
			{
				this.Bishop.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this.isExecuted = false;
			this.Bishop.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this.Bishop.SpriteRenderer.enabled = true;
			this.Bishop.Animator.Play("Idle");
			this.Bishop.CurrentLife = this.Bishop.Stats.Life.Base / 2f;
			this.Bishop.Attack.enabled = true;
			base.StartBehaviour();
			if (this.Bishop.EntExecution != null)
			{
				this.Bishop.EntExecution.enabled = false;
			}
		}

		private void OnDeath()
		{
			this.StopMovement();
			base.StopBehaviour();
		}

		[FoldoutGroup("Patrol", 0)]
		[Tooltip("Time wandering before Idle")]
		public float WanderingTime;

		private float _currentWanderingTime;

		[FoldoutGroup("Patrol", 0)]
		[Tooltip("Idle time before starts wandering")]
		public float IdleTime;

		[FoldoutGroup("Patrol", 0)]
		[Tooltip("Min distance to ground")]
		public float GroundDistance;

		[FoldoutGroup("Motion", 0)]
		[Tooltip("Chasing Speed")]
		public float MaxSpeed = 3f;

		private bool _isMaxSpeed;

		[FoldoutGroup("Motion", 0)]
		[Tooltip("Wander Speed")]
		public float MinSpeed = 1f;

		private bool _isMinSpeed;

		[FoldoutGroup("Attack", 0)]
		[Tooltip("Wander Speed")]
		public float MinDistanceToTarget = 2f;

		[FoldoutGroup("Hurt", 0)]
		[Tooltip("Displacement when the enemy is hit")]
		public float HurtDisplacement = 1f;

		private bool isExecuted;

		public const float HurtAnimDuration = 0.55f;
	}
}
