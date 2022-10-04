using System;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Util;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[RequireComponent(typeof(CircleCollider2D))]
	public class CircleAttackArea : MonoBehaviour, ICollisionEmitter
	{
		public bool IsTargetHit { get; private set; }

		public bool EnemyIsInAttackArea { get; private set; }

		public Vector3 LocalPos { get; set; }

		public CircleCollider2D WeaponCollider { get; private set; }

		private void Awake()
		{
			this.WeaponCollider = base.GetComponent<CircleCollider2D>();
			Entity componentInParent = base.GetComponentInParent<Entity>();
			if (componentInParent != null)
			{
				this.Entity = componentInParent;
			}
			this.LocalPos = base.transform.localPosition;
		}

		private void Update()
		{
		}

		public GameObject[] OverlappedEntities()
		{
			Collider2D[] array = Physics2D.OverlapAreaAll(this.WeaponCollider.bounds.min, this.WeaponCollider.bounds.max, this.enemyLayerMask);
			GameObject[] array2 = new GameObject[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i].gameObject;
				array2.SetValue(gameObject, i);
			}
			return array2;
		}

		public void SetRadius(float radius)
		{
			if (this.WeaponCollider == null)
			{
				return;
			}
			this.WeaponCollider.radius = radius;
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

		public Entity Entity;

		private EntityOrientation _entityOrientation;
	}
}
