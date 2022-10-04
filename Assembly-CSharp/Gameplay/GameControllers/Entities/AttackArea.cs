using System;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Util;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[RequireComponent(typeof(Collider2D))]
	public class AttackArea : MonoBehaviour, ICollisionEmitter
	{
		public bool IsTargetHit { get; private set; }

		public bool EnemyIsInAttackArea { get; private set; }

		public Vector3 LocalPos { get; set; }

		public Vector2 RayCastOrigin
		{
			get
			{
				return this._bottomRayCastOrigin;
			}
		}

		public Collider2D WeaponCollider { get; private set; }

		public void SetLayerMask(LayerMask lm)
		{
			this.enemyLayerMask = lm;
			this.SetContactFilter();
		}

		private void SetContactFilter()
		{
			this._contactFilter = new ContactFilter2D
			{
				layerMask = this.enemyLayerMask,
				useLayerMask = true,
				useTriggers = true
			};
		}

		private void Awake()
		{
			this.WeaponCollider = base.GetComponent<Collider2D>();
			Entity componentInParent = base.GetComponentInParent<Entity>();
			if (componentInParent != null)
			{
				this.Entity = componentInParent;
			}
			this.LocalPos = base.transform.localPosition;
			this.hits = new Collider2D[10];
			this.SetContactFilter();
		}

		private void Update()
		{
			if (this.EnableTargetRayCast)
			{
				this.IsTargetHit = this.IsEnemyHit();
			}
			if (this.Entity == null)
			{
				return;
			}
			if (this.Entity.Status.IsVisibleOnCamera && !this.Entity.Status.Dead)
			{
				this.Entity.EntityAttack.IsEnemyHit = this.IsTargetHit;
			}
			if (this.ChangesColliderOrientation)
			{
				this.SetColliderOrientation();
			}
		}

		private void SetColliderOrientation()
		{
			Vector3 localScale = base.transform.localScale;
			EntityOrientation orientation = this.Entity.Status.Orientation;
			if (orientation != EntityOrientation.Left)
			{
				if (orientation == EntityOrientation.Right)
				{
					if (localScale.x < 1f)
					{
						localScale.x = 1f;
					}
				}
			}
			else if (localScale.x > -1f)
			{
				localScale.x = -1f;
			}
			base.transform.localScale = localScale;
		}

		public bool IsEnemyHit()
		{
			Vector2 v = (this.Entity.Status.Orientation != EntityOrientation.Left) ? Vector2.right : (-Vector2.right);
			Vector3 vector = base.transform.TransformDirection(v);
			float x = (this.Entity.Status.Orientation != EntityOrientation.Left) ? this.WeaponCollider.bounds.min.x : this.WeaponCollider.bounds.max.x;
			this._topRayCastOrigin = new Vector2(x, this.WeaponCollider.bounds.max.y);
			this._bottomRayCastOrigin = new Vector2(x, this.WeaponCollider.bounds.min.y);
			bool flag = Physics2D.Raycast(this._topRayCastOrigin, vector, this.WeaponCollider.bounds.size.x + this.entityScopeDetection, this.enemyLayerMask);
			bool flag2 = Physics2D.Raycast(this._bottomRayCastOrigin, vector, this.WeaponCollider.bounds.size.x + this.entityScopeDetection, this.enemyLayerMask);
			UnityEngine.Debug.DrawRay(this._topRayCastOrigin, vector * (this.WeaponCollider.bounds.size.x + this.entityScopeDetection), Color.blue);
			UnityEngine.Debug.DrawRay(this._bottomRayCastOrigin, vector * (this.WeaponCollider.bounds.size.x + this.entityScopeDetection), Color.blue);
			return flag2 || flag;
		}

		private void DrawDebugCross(Vector2 point, Color c, float seconds)
		{
			float d = 0.6f;
			UnityEngine.Debug.DrawLine(point - Vector2.up * d, point + Vector2.up * d, c, seconds);
			UnityEngine.Debug.DrawLine(point - Vector2.right * d, point + Vector2.right * d, c, seconds);
		}

		public GameObject[] OverlappedEntities()
		{
			int num = this.WeaponCollider.OverlapCollider(this._contactFilter, this.hits);
			GameObject[] array = new GameObject[num];
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = this.hits[i].gameObject;
				array.SetValue(gameObject, i);
			}
			return array;
		}

		public void SetLocalHeight(float yLocalPos)
		{
			if (Math.Abs(base.transform.localPosition.y - yLocalPos) > Mathf.Epsilon)
			{
				Vector3 localPosition = new Vector3(base.transform.localPosition.x, yLocalPos, base.transform.localPosition.z);
				base.transform.localPosition = localPosition;
			}
		}

		public void SetSize(Vector2 size)
		{
			if (this.WeaponCollider == null)
			{
				return;
			}
			((BoxCollider2D)this.WeaponCollider).size = size;
		}

		public void SetOffset(Vector2 offset)
		{
			if (this.WeaponCollider == null)
			{
				return;
			}
			this.WeaponCollider.offset = offset;
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if ((this.enemyLayerMask.value & 1 << col.gameObject.layer) <= 0)
			{
				return;
			}
			this.OnTriggerEnter2DNotify(col);
		}

		private void OnTriggerStay2D(Collider2D col)
		{
			if ((this.enemyLayerMask.value & 1 << col.gameObject.layer) <= 0)
			{
				return;
			}
			this.OnTriggerStay2DNotify(col);
			if (!this.EnemyIsInAttackArea)
			{
				this.EnemyIsInAttackArea = true;
			}
		}

		private void OnTriggerExit2D(Collider2D col)
		{
			if ((this.enemyLayerMask.value & 1 << col.gameObject.layer) <= 0)
			{
				return;
			}
			this.OnTriggerExit2DNotify(col);
			if (this.EnemyIsInAttackArea)
			{
				this.EnemyIsInAttackArea = !this.EnemyIsInAttackArea;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<Collider2DParam> OnEnter;

		public void OnTriggerEnter2DNotify(Collider2D c)
		{
			this.OnEnter2DNotify(c);
		}

		public void OnEnter2DNotify(Collider2D c)
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

		public LayerMask enemyLayerMask;

		[Range(0f, 1f)]
		public float entityScopeDetection = 2.5f;

		public bool ChangesColliderOrientation = true;

		public Entity Entity;

		private EntityOrientation _entityOrientation;

		private Vector2 _bottomRayCastOrigin;

		private Vector2 _topRayCastOrigin;

		public bool EnableTargetRayCast;

		private ContactFilter2D _contactFilter;

		private Collider2D[] hits;
	}
}
