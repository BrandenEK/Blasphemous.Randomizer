using System;
using System.Diagnostics;
using Com.LuisPedroFonseca.ProCamera2D;
using CreativeSpore.SmartColliders;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Environment.MovingPlatforms;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Sensor
{
	public class FloorDistanceChecker : Trait, ICollisionEmitter
	{
		public bool IsOnFloorPlatform { get; private set; }

		public Vector3 BottonNormalCollision
		{
			get
			{
				return this.bottomNormalCollision;
			}
		}

		public bool OneWayDownCollision
		{
			get
			{
				return this.oneWayDownCollision;
			}
		}

		public bool OnMovingPlatform { get; set; }

		public bool IsGrounded { get; private set; }

		public bool IsSideBlocked { get; private set; }

		private void Start()
		{
			this._penitent = (Penitent)base.EntityOwner;
			this.playerBoxCollider = this._penitent.DamageArea.GetComponent<BoxCollider2D>();
			this.playerSmartCollider = this._penitent.GetComponent<SmartPlatformCollider>();
			this.playerSmartColliderDefaultSize = this.playerSmartCollider.Size;
			this.playerSmartColliderReducedSize = new Vector2(0.1f, this.playerSmartColliderDefaultSize.y);
			SmartPlatformCollider smartPlatformCollider = this.playerSmartCollider;
			smartPlatformCollider.OnSideCollision = (SmartRectCollider2D.OnSideCollisionDelegate)Delegate.Combine(smartPlatformCollider.OnSideCollision, new SmartRectCollider2D.OnSideCollisionDelegate(this.player_OnSideCollision));
			this._smartPlatformCollider = this._penitent.GetComponentInChildren<SmartPlatformCollider>();
			this.maxHitsAllocated = 1;
			this.bottomHits = new RaycastHit2D[this.maxHitsAllocated];
			this.forwardHits = new RaycastHit2D[this.maxHitsAllocated];
		}

		private void Update()
		{
			this.CheckBlock();
			this._penitent.PlatformCharacterInput.canAirAttack = !this.IsGrounded;
			if (this._penitent.Status.Dead)
			{
				this._penitent.DamageArea.IncludeEnemyLayer(false);
			}
			else
			{
				this.EvaluateEnemyCollision();
			}
		}

		private void OnDestroy()
		{
			if (this.playerSmartCollider != null)
			{
				SmartPlatformCollider smartPlatformCollider = this.playerSmartCollider;
				smartPlatformCollider.OnSideCollision = (SmartRectCollider2D.OnSideCollisionDelegate)Delegate.Remove(smartPlatformCollider.OnSideCollision, new SmartRectCollider2D.OnSideCollisionDelegate(this.player_OnSideCollision));
			}
		}

		private void CheckBlock()
		{
			this.halfWidth = this.playerBoxCollider.bounds.extents.x / 2f;
			this.myHeight = this.playerBoxCollider.bounds.extents.y;
			this.topCenterBoxCollider = new Vector2(this.playerBoxCollider.bounds.center.x, this.playerBoxCollider.bounds.max.y);
			float num = 1.2f;
			if (this._penitent.Status.Orientation == EntityOrientation.Left)
			{
				Vector2 vector = base.transform.position + base.transform.right * this.halfWidth;
				Debug.DrawLine(vector, vector - Vector2.up * num, Color.magenta);
				this.IsGrounded = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * num, this.bottomHits, this.layerGroundedCollision) > 0);
				this.oneWayDownCollision = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * num, this.bottomHits, this.oneWayDownLayers) > 0);
				Vector2 vector2 = this.topCenterBoxCollider - (base.transform.right * (this.halfWidth * 0.5f) + Vector2.up * this.myHeight);
				Debug.DrawLine(vector2, vector2 - base.transform.right * this.rayLength, Color.cyan);
				this.isEnemyBlocked = (Physics2D.LinecastNonAlloc(vector2, vector2 - base.transform.right * this.rayLength, this.forwardHits, this.layerEnemyCollision) > 0);
				if (this.forwardHits.Length > 0)
				{
					this.isFrontBlocked = this.IsFrontBlocked(this.forwardHits[0]);
				}
				Vector2 vector3 = this.topCenterBoxCollider + (base.transform.right * this.halfWidth - Vector2.up * this.myHeight);
				Debug.DrawLine(vector3, vector3 + base.transform.right * this.rayLength, Color.yellow);
				this.IsSideBlocked = (Physics2D.LinecastNonAlloc(vector3, vector3 + base.transform.right * this.rayLength, this.forwardHits, this.layerGroundedCollision) > 0);
			}
			else
			{
				Vector2 vector = base.transform.position - base.transform.right * this.halfWidth;
				Debug.DrawLine(vector, vector - Vector2.up * num, Color.magenta);
				this.IsGrounded = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * num, this.bottomHits, this.layerGroundedCollision) > 0);
				this.oneWayDownCollision = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * num, this.bottomHits, this.oneWayDownLayers) > 0);
				Vector2 vector2 = this.topCenterBoxCollider + (base.transform.right * (this.halfWidth * 0.5f) - Vector2.up * this.myHeight);
				Debug.DrawLine(vector2, vector2 + base.transform.right * this.rayLength, Color.cyan);
				this.isEnemyBlocked = (Physics2D.LinecastNonAlloc(vector2, vector2 + base.transform.right * this.rayLength, this.forwardHits, this.layerEnemyCollision) > 0);
				if (this.forwardHits.Length > 0)
				{
					this.isFrontBlocked = this.IsFrontBlocked(this.forwardHits[0]);
				}
				Vector2 vector3 = this.topCenterBoxCollider - (base.transform.right * this.halfWidth + Vector2.up * this.myHeight);
				Debug.DrawLine(vector3, vector3 - base.transform.right * this.rayLength, Color.yellow);
				this.IsSideBlocked = (Physics2D.LinecastNonAlloc(vector3, vector3 - base.transform.right * this.rayLength, this.forwardHits, this.layerGroundedCollision) > 0);
			}
			if (!this._penitent.Status.Dead && this.IsGrounded && !this._penitent.IsClimbingLadder && !this._penitent.IsStickedOnWall && !this._penitent.Dash.IsUpperBlocked && !this._penitent.TrapChecker.DeathBySpike)
			{
				Vector2 origin = base.transform.position - 3.7f * this.halfWidth * Vector2.right;
				Vector2 origin2 = base.transform.position + 3.7f * this.halfWidth * Vector2.right;
				if (this.CheckPositionToBeSafe(origin2) && this.CheckPositionToBeSafe(origin))
				{
					Vector3 position = this._penitent.GetPosition();
					if (this.bottomHits[0])
					{
						position.y = this.bottomHits[0].collider.bounds.max.y;
					}
					Core.LevelManager.SetPlayerSafePosition(position);
				}
			}
			if (this._penitent.HasFlag("SIDE_BLOCKED") != this.IsSideBlocked)
			{
				this._penitent.SetFlag("SIDE_BLOCKED", this.IsSideBlocked);
			}
			if (this._penitent.HasFlag("FRONT_BLOCKED") != this.isFrontBlocked)
			{
				this._penitent.SetFlag("FRONT_BLOCKED", this.isFrontBlocked);
			}
			if (this.forwardHits.Length > 0)
			{
				Array.Clear(this.forwardHits, 0, 1);
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(Core.LevelManager.LastSafePosition, 0.5f);
		}

		private bool CheckPositionToBeSafe(Vector2 origin)
		{
			Debug.DrawRay(origin, -Vector2.up * 1.1f, Color.white);
			RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, -Vector2.up, 1.1f, this.layerSearchSafeCollision);
			if (!raycastHit2D)
			{
				return false;
			}
			GameObject gameObject = raycastHit2D.collider.gameObject;
			if ((1 << gameObject.layer & this.layerSearchSafeCollision) == 0)
			{
				return false;
			}
			bool flag = !gameObject.GetComponent(typeof(INoSafePosition));
			flag = (flag && (!gameObject.GetComponent<StraightMovingPlatform>() || !DOTween.IsTweening(gameObject.transform, false)));
			flag = (flag && (!gameObject.GetComponent<WaypointsMovingPlatform>() || !DOTween.IsTweening(gameObject.transform, false)));
			flag = (flag && !gameObject.CompareTag("SpikeTrap"));
			flag = (flag && !gameObject.CompareTag("AbyssTrap"));
			ProCamera2DNumericBoundaries proCamera2DNumericBoundaries = Core.Logic.CameraManager.ProCamera2DNumericBoundaries;
			if (flag && proCamera2DNumericBoundaries.UseNumericBoundaries)
			{
				if (flag && proCamera2DNumericBoundaries.UseRightBoundary)
				{
					flag = (origin.x <= proCamera2DNumericBoundaries.RightBoundary);
				}
				if (flag && proCamera2DNumericBoundaries.UseLeftBoundary)
				{
					flag = (origin.x >= proCamera2DNumericBoundaries.LeftBoundary);
				}
				if (flag && proCamera2DNumericBoundaries.UseTopBoundary)
				{
					flag = (origin.y <= proCamera2DNumericBoundaries.TopBoundary);
				}
				if (flag && proCamera2DNumericBoundaries.UseBottomBoundary)
				{
					flag = (origin.y >= proCamera2DNumericBoundaries.BottomBoundary);
				}
			}
			return flag;
		}

		public void IncreaseSkinWidth(bool increase = true)
		{
			this.playerSmartCollider.Size = ((!increase) ? this.playerSmartColliderReducedSize : this.playerSmartColliderDefaultSize);
		}

		private void EvaluateEnemyCollision()
		{
			if (!this._penitent.Status.IsGrounded || this._penitent.IsJumpingOff)
			{
				this._penitent.DamageArea.IncludeEnemyLayer(false);
			}
			else if (this._penitent.Status.IsGrounded && !this._penitent.IsDashing && !this._penitent.MotionLerper.IsLerping)
			{
				this._penitent.DamageArea.IncludeEnemyLayer(true);
			}
		}

		private void player_OnSideCollision(SmartCollision2D col, GameObject go)
		{
			int layer = go.layer;
			if (this.layerGroundedCollision == (this.layerGroundedCollision | 1 << layer))
			{
				this.bottomNormalCollision = col.contacts[0].normal;
			}
			this._penitent.SetFlag("SIDE_BLOCKED", false);
		}

		private bool IsFrontBlocked(RaycastHit2D rayCastHit)
		{
			bool result = false;
			if (rayCastHit.collider)
			{
				GameObject gameObject = rayCastHit.collider.gameObject;
				if (gameObject.layer != LayerMask.NameToLayer("Enemy"))
				{
					result = (this.layerEnemyCollision == (this.layerEnemyCollision | 1 << gameObject.layer));
				}
			}
			return result;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Ladder") && FloorDistanceChecker.OnStepLadder != null)
			{
				FloorDistanceChecker.OnStepLadder(other);
				this._penitent.StepOnLadder = true;
			}
			if (!this.IsOnFloorPlatform && (1 << other.gameObject.layer & this.LayerFloor) != 0)
			{
				this.IsOnFloorPlatform = true;
			}
			this.OnTriggerEnter2DNotify(other);
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Ladder") && this._penitent.Status.IsGrounded && this._smartPlatformCollider.SkinBottomOff01 > 0f)
			{
				this._smartPlatformCollider.SkinBottomOff01 = 0f;
			}
			this.OnTriggerStay2DNotify(other);
			if (!this.IsOnFloorPlatform && (1 << other.gameObject.layer & this.LayerFloor) != 0)
			{
				this.IsOnFloorPlatform = true;
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
			{
				this._smartPlatformCollider.SkinBottomOff01 = 0.1f;
				this._penitent.StepOnLadder = false;
			}
			if (this.IsOnFloorPlatform && (1 << other.gameObject.layer & this.LayerFloor) != 0)
			{
				this.IsOnFloorPlatform = false;
			}
			this.OnTriggerExit2DNotify(other);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<Collider2DParam> OnEnter;

		public void OnTriggerEnter2DNotify(Collider2D c)
		{
			this.OnEnter2DNotify(c);
		}

		private void OnEnter2DNotify(Collider2D c)
		{
			if (this.OnEnter != null)
			{
				this.OnEnter(this, new Collider2DParam
				{
					Collider2DArg = c
				});
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<Collider2DParam> OnStay;

		public void OnTriggerStay2DNotify(Collider2D c)
		{
			this.OnStay2DNotify(c);
		}

		private void OnStay2DNotify(Collider2D c)
		{
			if (this.OnStay != null)
			{
				this.OnStay(this, new Collider2DParam
				{
					Collider2DArg = c
				});
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<Collider2DParam> OnExit;

		public void OnTriggerExit2DNotify(Collider2D c)
		{
			this.OnExit2DNotify(c);
		}

		public void OnExit2DNotify(Collider2D c)
		{
			if (this.OnExit != null)
			{
				this.OnExit(this, new Collider2DParam
				{
					Collider2DArg = c
				});
			}
		}

		public static Core.GenericEvent OnStepLadder;

		private Penitent _penitent;

		public LayerMask layerGroundedCollision;

		public LayerMask layerSearchSafeCollision;

		public LayerMask layerEnemyCollision;

		public LayerMask oneWayDownLayers;

		[Range(0f, 3f)]
		public float rayLength;

		private BoxCollider2D playerBoxCollider;

		private SmartPlatformCollider playerSmartCollider;

		private Vector2 playerSmartColliderDefaultSize;

		private Vector2 playerSmartColliderReducedSize;

		private float halfWidth;

		private float myHeight;

		private Vector2 topCenterBoxCollider;

		private bool isEnemyBlocked;

		private bool isFrontBlocked;

		private SmartPlatformCollider _smartPlatformCollider;

		private RaycastHit2D[] bottomHits;

		private RaycastHit2D[] forwardHits;

		private int maxHitsAllocated;

		[SerializeField]
		private LayerMask LayerFloor;

		private Vector3 bottomNormalCollision;

		private bool oneWayDownCollision;
	}
}
