using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Acolyte.IA
{
	public class AcolyteBehaviour : EnemyBehaviour
	{
		public GameObject Target { get; set; }

		public bool IsTargetOnRange { get; set; }

		public bool IsTargetOnSight { get; set; }

		public bool IsAttackWindowOpen { get; set; }

		public bool IsIdleTimeElapsed { get; set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this._acolyte = base.GetComponent<Acolyte>();
		}

		public override void OnStart()
		{
			base.OnStart();
			BoxCollider2D componentInChildren = base.GetComponentInChildren<BoxCollider2D>();
			this._myWidth = componentInChildren.bounds.extents.x;
			this._myHeight = componentInChildren.bounds.extents.y;
			this._currentGroundDetection = this.rangeGroundDetection;
			this._attackArea = this._acolyte.AttackArea;
			this._maxHitsAllocated = 2;
			this._bottomHits = new RaycastHit2D[this._maxHitsAllocated];
			this._forwardsHits = new RaycastHit2D[this._maxHitsAllocated];
			this._acolyte.OnDeath += this.AcolyteOnEntityDie;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.IsIdleTimeElapsed = (this._deltaAttackTime >= this.AttackTime);
			if (this.Target)
			{
				this.TargetOnRange(this.Target);
			}
			this.IsTargetOnSight = this.TargetOnSight();
			if (this.Entity.Status.Orientation == EntityOrientation.Left)
			{
				Vector2 vector = base.transform.position - (base.transform.right * this._myWidth * 1.5f + Vector2.up * (this._myHeight * 2f));
				Debug.DrawLine(vector, vector - Vector2.up * this._currentGroundDetection, Color.yellow);
				base.SensorHitsFloor = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * this._currentGroundDetection, this._bottomHits, this.BlockLayerMask) > 0);
				Debug.DrawLine(vector, vector - base.transform.right * this.RangeBlockDectection, Color.yellow);
				this.isBlocked = (Physics2D.LinecastNonAlloc(vector, vector - base.transform.right * this.RangeBlockDectection, this._forwardsHits, this.BlockLayerMask) > 0);
			}
			else
			{
				Vector2 vector = base.transform.position + (base.transform.right * this._myWidth * 1.5f - Vector2.up * (this._myHeight * 2f));
				Debug.DrawLine(vector, vector - Vector2.up * this._currentGroundDetection, Color.yellow);
				base.SensorHitsFloor = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * this._currentGroundDetection, this._bottomHits, this.BlockLayerMask) > 0);
				Debug.DrawLine(vector, vector + base.transform.right * this.RangeBlockDectection, Color.yellow);
				this.isBlocked = (Physics2D.LinecastNonAlloc(vector, vector + base.transform.right * this.RangeBlockDectection, this._forwardsHits, this.BlockLayerMask) > 0);
			}
			this.TrapDetected = base.DetectTrap(this._bottomHits);
			this.CheckGrounded();
			if (base.SensorHitsFloor)
			{
				return;
			}
			if (this._acolyte.MotionLerper.IsLerping)
			{
				this._acolyte.MotionLerper.StopLerping();
			}
		}

		public bool CanSeeTarget
		{
			get
			{
				return this._acolyte.VisionCone.CanSeeTarget(this._acolyte.Target.transform, "Penitent", false);
			}
		}

		public void CheckGrounded()
		{
			if (this._acolyte == null)
			{
				return;
			}
			bool isGrounded = this._acolyte.Status.IsGrounded;
			this._acolyte.AnimatorInyector.Grounded(isGrounded);
		}

		public override void StopMovement()
		{
			if (this._acolyte == null)
			{
				return;
			}
			this._acolyte.SetMovementSpeed(0f);
			this._acolyte.Inputs.HorizontalInput = 0f;
			if (Mathf.Abs(this._acolyte.Controller.PlatformCharacterPhysics.HSpeed) > 0f)
			{
				this._acolyte.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			}
			this._acolyte.Controller.PlatformCharacterPhysics.Velocity = Vector3.zero;
		}

		private void AcolyteOnEntityDie()
		{
			base.BehaviourTree.StopBehaviour();
			this.StopMovement();
			this._acolyte.StopMovementLerping();
			this._acolyte.AnimatorInyector.Dead();
		}

		public bool TargetOnRange(GameObject target)
		{
			this.IsTargetOnRange = (Vector2.Distance(base.transform.position, target.transform.position) <= this.attackRange);
			return this.IsTargetOnRange;
		}

		public bool TargetOnSight()
		{
			int num = (this._acolyte.Status.Orientation != EntityOrientation.Right) ? -1 : 1;
			return Physics2D.Raycast(this._attackArea.transform.position, Vector2.right * (float)num, 10f, this.TargetLayer);
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this._acolyte.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
			{
				return;
			}
			base.LookAtTarget(targetPos);
		}

		public override void ReverseOrientation()
		{
			if (this._acolyte.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
			{
				return;
			}
			base.ReverseOrientation();
		}

		public override void Idle()
		{
			if (this._acolyte == null)
			{
				return;
			}
			if (!this._acolyte.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
			{
				this._deltaAttackTime += Time.deltaTime;
			}
			this._acolyte.AnimatorInyector.Idle();
			this.StopMovement();
		}

		public override void Wander()
		{
			if (this._acolyte == null)
			{
				return;
			}
			bool flag = this._acolyte.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
			if (!flag)
			{
				this._deltaAttackTime += Time.deltaTime;
			}
			this._acolyte.AnimatorInyector.Wander();
			float horizontalInput = (this._acolyte.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this._acolyte.Inputs.HorizontalInput = horizontalInput;
			if (flag)
			{
				return;
			}
			this._acolyte.SetMovementSpeed(this._acolyte.MinSpeed);
		}

		public override void Chase(Transform targetPosition)
		{
			if (this._acolyte == null)
			{
				return;
			}
			bool flag = this._acolyte.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
			if (!flag)
			{
				this._deltaAttackTime += Time.deltaTime;
			}
			this._acolyte.AnimatorInyector.Wander();
			if (flag)
			{
				return;
			}
			float horizontalInput = (this._acolyte.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			if (this._acolyte.IsFalling)
			{
				this.StopMovement();
			}
			else
			{
				this._acolyte.SetMovementSpeed(this._acolyte.MinSpeed);
				this._acolyte.Inputs.HorizontalInput = horizontalInput;
			}
		}

		public void StopChase()
		{
			this.StopMovement();
			this._acolyte.AnimatorInyector.StopChasing();
		}

		public override void Attack()
		{
			if (this._acolyte == null)
			{
				return;
			}
			this._deltaAttackTime = 0f;
			bool isGrounded = this._acolyte.Controller.IsGrounded;
			this._acolyte.AnimatorInyector.Attack(isGrounded);
			this.StopMovement();
		}

		public override void Damage()
		{
			if (this._acolyte == null)
			{
				return;
			}
			if (this._acolyte.Animator.speed > 1f)
			{
				this._acolyte.Animator.speed = 1f;
			}
			this.StopMovement();
			this._acolyte.SetMovementSpeed(this._acolyte.MinSpeed);
		}

		public override void Parry()
		{
			base.Parry();
			this._acolyte.AnimatorInyector.ParryReaction();
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("Parry");
			this._acolyte.Animator.speed = 1.75f;
			base.StopBehaviour();
			base.StartCoroutine(this.AdjustParryDistance());
		}

		private Vector2 GetDirFromOrientation()
		{
			return new Vector2((float)((this._acolyte.Status.Orientation != EntityOrientation.Right) ? -1 : 1), 0f);
		}

		private IEnumerator AdjustParryDistance()
		{
			float parryDistance = 2.5f;
			float curDist = Vector2.Distance(base.transform.position, Core.Logic.Penitent.transform.position);
			if (curDist > parryDistance)
			{
				this.StopMovement();
				float oldDistance = this._acolyte.MotionLerper.distanceToMove;
				Vector2 dir = this.GetDirFromOrientation();
				this._acolyte.MotionLerper.distanceToMove = curDist - parryDistance;
				this._acolyte.MotionLerper.StartLerping(dir);
				float counter = 0.5f;
				while (counter > 0f && curDist > parryDistance)
				{
					counter -= Time.deltaTime;
					yield return null;
				}
				this._acolyte.MotionLerper.StopLerping();
				this._acolyte.MotionLerper.distanceToMove = oldDistance;
			}
			yield break;
		}

		public override void Execution()
		{
			base.Execution();
			this._acolyte.gameObject.layer = LayerMask.NameToLayer("Default");
			this.StopMovement();
			base.StopBehaviour();
			this._acolyte.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this._acolyte.EntExecution.InstantiateExecution();
		}

		public override void Alive()
		{
			base.Alive();
			this._acolyte.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this._acolyte.SpriteRenderer.enabled = true;
			this._acolyte.AnimatorInyector.Idle();
			this._acolyte.Animator.speed = 1f;
			this._acolyte.Animator.Play("Idle");
			this._acolyte.CurrentLife = this._acolyte.Stats.Life.Base / 2f;
			base.StartBehaviour();
			if (this._acolyte.EntExecution != null)
			{
				this._acolyte.EntExecution.enabled = false;
			}
		}

		private Acolyte _acolyte;

		private AttackArea _attackArea;

		private RaycastHit2D[] _bottomHits;

		private RaycastHit2D[] _forwardsHits;

		private float _currentGroundDetection;

		private float _deltaAttackTime;

		private int _maxHitsAllocated;

		[Header("Sensors variables")]
		private float _myWidth;

		[Header("Sensors variables")]
		private float _myHeight;

		public bool AllowEntityOrientation;

		public float AttackTime = 1f;

		public float MaxRangeGroundDetection = 5f;

		public float attackRange = 5f;

		[Tooltip("The length of the block detection raycast")]
		[Range(0f, 1f)]
		public float RangeBlockDectection = 0.5f;

		[Tooltip("The length og the ground detection raycast")]
		[Range(0f, 10f)]
		public float rangeGroundDetection = 2f;

		public LayerMask TargetLayer;
	}
}
