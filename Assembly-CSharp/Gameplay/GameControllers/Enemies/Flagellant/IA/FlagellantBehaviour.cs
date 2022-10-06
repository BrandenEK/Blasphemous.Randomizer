using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Flagellant.IA
{
	public class FlagellantBehaviour : EnemyBehaviour
	{
		public override void OnAwake()
		{
			base.OnAwake();
			this._flagellant = base.GetComponent<Flagellant>();
			this.Entity = this._flagellant;
		}

		public override void OnStart()
		{
			base.OnStart();
			BoxCollider2D componentInChildren = base.GetComponentInChildren<BoxCollider2D>();
			this._myWidth = componentInChildren.bounds.extents.x;
			this._myHeight = componentInChildren.bounds.extents.y;
			this._currentGroundDetection = this.RangeGroundDetection;
			this._maxHitsAllocated = 2;
			this._bottomHits = new RaycastHit2D[this._maxHitsAllocated];
			this._forwardsHits = new RaycastHit2D[this._maxHitsAllocated];
			this._flagellant.OnDeath += this.FlagellantOnEntityDie;
			this._currentTimeWandering = this.GetMaxWanderingLapse();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Entity.Status.Orientation == EntityOrientation.Left)
			{
				Vector2 vector = base.transform.position - (base.transform.right * this._myWidth * 0.75f + Vector2.up * (this._myHeight * 2f));
				Debug.DrawLine(vector, vector - Vector2.up * this._currentGroundDetection, Color.yellow);
				base.SensorHitsFloor = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * this._currentGroundDetection, this._bottomHits, this.BlockLayerMask) > 0);
				Debug.DrawLine(vector, vector - base.transform.right * this.RangeBlockDectection, Color.yellow);
				this.isBlocked = (Physics2D.LinecastNonAlloc(vector, vector - base.transform.right * this.RangeBlockDectection, this._forwardsHits, this.BlockLayerMask) > 0);
			}
			else
			{
				Vector2 vector = base.transform.position + (base.transform.right * this._myWidth * 0.75f - Vector2.up * (this._myHeight * 2f));
				Debug.DrawLine(vector, vector - Vector2.up * this._currentGroundDetection, Color.yellow);
				base.SensorHitsFloor = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * this._currentGroundDetection, this._bottomHits, this.BlockLayerMask) > 0);
				Debug.DrawLine(vector, vector + base.transform.right * this.RangeBlockDectection, Color.yellow);
				this.isBlocked = (Physics2D.LinecastNonAlloc(vector, vector + base.transform.right * this.RangeBlockDectection, this._forwardsHits, this.BlockLayerMask) > 0);
			}
			this.TrapDetected = base.DetectTrap(this._bottomHits);
			this.CheckGrounded();
		}

		protected void CheckGrounded()
		{
			if (this._flagellant == null)
			{
				return;
			}
			bool isGrounded = this._flagellant.Controller.IsGrounded;
			this._flagellant.AnimatorInyector.Grounded(isGrounded);
		}

		public override void Idle()
		{
			if (this._flagellant == null)
			{
				return;
			}
			this._currentGroundDetection = this.RangeGroundDetection;
			this._flagellant.AnimatorInyector.Idle();
			this._flagellant.Inputs.HorizontalInput = 0f;
			if (this._flagellant.Controller.PlatformCharacterPhysics.Velocity.magnitude > 0f && this._flagellant.Status.IsGrounded)
			{
				this._flagellant.Controller.PlatformCharacterPhysics.Velocity = Vector3.zero;
			}
		}

		public override void Wander()
		{
			if (this._flagellant == null)
			{
				return;
			}
			this._currentTimeWandering += Time.deltaTime;
			if (this._currentTimeWandering <= 8f)
			{
				this._currentGroundDetection = this.RangeGroundDetection;
				this._flagellant.AnimatorInyector.Wander();
				float horizontalInput = (this._flagellant.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
				this._flagellant.Inputs.HorizontalInput = horizontalInput;
				this._flagellant.SetMovementSpeed(this._flagellant.MIN_SPEED);
				this._wanderTimer = Time.time;
			}
			else
			{
				this.Idle();
				if (Time.time - this._wanderTimer > 5.74f)
				{
					this._currentTimeWandering = this.GetMaxWanderingLapse();
				}
			}
		}

		public bool CanSeeTarget
		{
			get
			{
				return this._flagellant.VisionCone.CanSeeTarget(this._flagellant.Target.transform, "Penitent", false);
			}
		}

		public override void Chase(Transform target)
		{
			if (this._flagellant == null)
			{
				return;
			}
			this._currentGroundDetection = this.RangeGroundDetection * 1f;
			bool isGrounded = this._flagellant.Controller.IsGrounded;
			this._flagellant.AnimatorInyector.Chase(isGrounded);
			float num = (this._flagellant.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			if (this._flagellant.IsFalling || base.IsAttacking)
			{
				this.StopMovement();
			}
			else
			{
				this._flagellant.Inputs.HorizontalInput = num;
				if (Mathf.Abs(this._flagellant.Controller.PlatformCharacterPhysics.HSpeed) < this._flagellant.MAX_SPEED)
				{
					this._flagellant.Controller.PlatformCharacterPhysics.HSpeed = this._flagellant.MAX_SPEED * num;
				}
				this._flagellant.SetMovementSpeed(this._flagellant.MAX_SPEED);
			}
		}

		public override void StopMovement()
		{
			if (this._flagellant == null)
			{
				return;
			}
			this._flagellant.SetMovementSpeed(0f);
			this._flagellant.Inputs.HorizontalInput = 0f;
			if (Mathf.Abs(this._flagellant.Controller.PlatformCharacterPhysics.HSpeed) > 0f)
			{
				this._flagellant.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			}
		}

		public override void Attack()
		{
			if (this._flagellant == null)
			{
				return;
			}
			bool isGrounded = this._flagellant.Controller.IsGrounded;
			this._flagellant.AnimatorInyector.Attack(isGrounded);
			this.StopMovement();
		}

		public override void Damage()
		{
			if (this._flagellant == null)
			{
				return;
			}
			this.StopMovement();
			this._flagellant.AnimatorInyector.Hurt();
			this._flagellant.SetMovementSpeed(this._flagellant.MIN_SPEED);
		}

		public override void Parry()
		{
			base.Parry();
			base.GotParry = true;
			this._flagellant.AnimatorInyector.ParryReaction();
		}

		public override void Execution()
		{
			base.Execution();
			this._flagellant.gameObject.layer = LayerMask.NameToLayer("Default");
			this.StopMovement();
			base.StopBehaviour();
			this._flagellant.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this._flagellant.EntExecution.InstantiateExecution();
			if (this._flagellant.EntExecution != null)
			{
				this._flagellant.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this._flagellant.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this._flagellant.SpriteRenderer.enabled = true;
			this._flagellant.AnimatorInyector.Idle();
			this._flagellant.Animator.Play("Idle");
			this._flagellant.CurrentLife = this._flagellant.Stats.Life.Base / 2f;
			base.StartBehaviour();
			if (this._flagellant.EntExecution != null)
			{
				this._flagellant.EntExecution.enabled = false;
			}
		}

		private float GetMaxWanderingLapse()
		{
			return Random.Range(0f, 4f);
		}

		private void FlagellantOnEntityDie()
		{
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
		}

		public bool IsPatrolBlocked()
		{
			return this._flagellant.MotionChecker.HitsPatrolBlock;
		}

		public const float IdleAnimClipDuration = 2.87f;

		public const float MaxTimeWandering = 8f;

		private RaycastHit2D[] _bottomHits;

		private RaycastHit2D[] _forwardsHits;

		private float _currentGroundDetection;

		private float _currentTimeWandering;

		private Flagellant _flagellant;

		private int _maxHitsAllocated;

		[Header("Sensors variables")]
		private float _myWidth;

		[Header("Sensors variables")]
		private float _myHeight;

		private float _wanderTimer;

		public bool AllowEntityOrientation;

		public float MaxRangeGroundDetection = 5f;

		[Tooltip("The length of the block detection raycast")]
		[Range(0f, 1f)]
		public float RangeBlockDectection = 0.5f;

		[Tooltip("The length og the ground detection raycast")]
		[Range(0f, 10f)]
		public float RangeGroundDetection = 2f;
	}
}
