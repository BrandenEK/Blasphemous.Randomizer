using System;
using System.Diagnostics;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.Physics
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class EnemyFloorChecker : MonoBehaviour, ICollisionEmitter
	{
		public bool IsGrounded { get; private set; }

		public bool IsSideBlocked { get; private set; }

		public Vector3 EnemyFloorCollisionNormal { get; private set; }

		private void Awake()
		{
			this._feetBoxCollider = base.GetComponent<BoxCollider2D>();
		}

		private void Start()
		{
			this._enemy = base.GetComponentInParent<Enemy>();
			this._enemySmartPlatformCollider = this._enemy.GetComponent<SmartPlatformCollider>();
			SmartPlatformCollider enemySmartPlatformCollider = this._enemySmartPlatformCollider;
			enemySmartPlatformCollider.OnSideCollision = (SmartRectCollider2D.OnSideCollisionDelegate)Delegate.Combine(enemySmartPlatformCollider.OnSideCollision, new SmartRectCollider2D.OnSideCollisionDelegate(this.enemy_OnSideCollision));
			this._widthChecker = this._feetBoxCollider.bounds.extents.x;
			this._heightChecker = this._feetBoxCollider.bounds.extents.y;
			this._maxReturnedIntersections = 2;
			this._leftHits = new RaycastHit2D[this._maxReturnedIntersections];
			this._rightHits = new RaycastHit2D[this._maxReturnedIntersections];
			this._sideHits = new RaycastHit2D[this._maxReturnedIntersections];
		}

		protected void enemy_OnSideCollision(SmartCollision2D col, GameObject go)
		{
			int layer = go.layer;
			if (layer == LayerMask.NameToLayer("Floor") || layer == LayerMask.NameToLayer("OneWayDown"))
			{
				this.EnemyFloorCollisionNormal = col.contacts[0].normal;
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.layer == LayerMask.NameToLayer("Trap") && collision.gameObject.CompareTag("SpikeTrap") && this.OnTrapFall != null)
			{
				this.OnTrapFall();
			}
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			this.OnTriggerStay2DNotify(other);
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			this.OnTriggerExit2DNotify(other);
		}

		private void Update()
		{
			if (!this._enemy)
			{
				return;
			}
			this.IsGrounded = this.CheckGrounded();
			this._enemy.Status.IsGrounded = this._enemy.Controller.IsGrounded;
			if (this._enemy.Status.IsGrounded)
			{
				this._enemy.SlopeAngle = this._enemy.Controller.SmartPlatformCollider.CalculateSlopeAngle();
			}
			this._topCenterBoxCollider = new Vector2(this._feetBoxCollider.bounds.center.x, this._feetBoxCollider.bounds.max.y);
			if (this._enemy.Status.Orientation == EntityOrientation.Left)
			{
				Vector2 vector = this._topCenterBoxCollider + (base.transform.right * this._widthChecker + Vector2.up * this._heightChecker * 3f);
				UnityEngine.Debug.DrawLine(vector, vector + base.transform.right * this.CurrentSideRangeDetection, Color.yellow);
				this.IsSideBlocked = (Physics2D.LinecastNonAlloc(vector, vector + base.transform.right * this.CurrentSideRangeDetection, this._sideHits, this.FloorLayerMasks) > 0);
			}
			else
			{
				Vector2 vector2 = this._topCenterBoxCollider - (base.transform.right * this._widthChecker - Vector2.up * this._heightChecker * 3f);
				UnityEngine.Debug.DrawLine(vector2, vector2 - base.transform.right * this.CurrentSideRangeDetection, Color.yellow);
				this.IsSideBlocked = (Physics2D.LinecastNonAlloc(vector2, vector2 - base.transform.right * this.CurrentSideRangeDetection, this._sideHits, this.FloorLayerMasks) > 0);
			}
			if (this._sideHits.Length > 0)
			{
				Array.Clear(this._sideHits, 0, 1);
			}
		}

		private bool CheckGrounded()
		{
			Vector2 vector = this._enemy.EntityDamageArea.Center() - this._enemy.EntityDamageArea.transform.right * this._widthChecker;
			Vector2 vector2 = this._enemy.EntityDamageArea.Center() + this._enemy.EntityDamageArea.transform.right * this._widthChecker;
			UnityEngine.Debug.DrawLine(vector, vector - Vector2.up * this.CurrentGroundDetection, Color.red);
			bool flag = Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * this.CurrentGroundDetection, this._leftHits, this.FloorLayerMasks) > 0;
			UnityEngine.Debug.DrawLine(vector2, vector2 - Vector2.up * this.CurrentGroundDetection, Color.red);
			bool flag2 = Physics2D.LinecastNonAlloc(vector2, vector2 - Vector2.up * this.CurrentGroundDetection, this._rightHits, this.FloorLayerMasks) > 0;
			return flag && flag2;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<Collider2DParam> OnEnter;

		public void OnTriggerEnter2DNotify(Collider2D c)
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

		private Enemy _enemy;

		private SmartPlatformCollider _enemySmartPlatformCollider;

		private BoxCollider2D _feetBoxCollider;

		private float _heightChecker;

		private RaycastHit2D[] _leftHits;

		private RaycastHit2D[] _rightHits;

		private RaycastHit2D[] _sideHits;

		private int _maxReturnedIntersections;

		private Vector2 _topCenterBoxCollider;

		private float _widthChecker;

		public float CurrentGroundDetection = 0.15f;

		public float CurrentSideRangeDetection = 0.5f;

		public LayerMask FloorLayerMasks;

		public Core.SimpleEvent OnTrapFall;
	}
}
