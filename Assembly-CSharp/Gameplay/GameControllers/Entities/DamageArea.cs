using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameControllers.Entities
{
	public class DamageArea : MonoBehaviour
	{
		public Entity OwnerEntity { get; set; }

		public Gameplay.GameControllers.Entities.Hit LastHit { get; set; }

		public float DamageAreaLenght()
		{
			return this.damageAreaCollider.bounds.max.y - this.damageAreaCollider.bounds.min.y;
		}

		public Vector3 Center()
		{
			return this.damageAreaCollider.bounds.center;
		}

		public Vector3 TopCenter
		{
			get
			{
				float x = this.damageAreaCollider.bounds.center.x;
				float y = this.damageAreaCollider.bounds.max.y;
				return new Vector2(x, y);
			}
		}

		public Collider2D DamageAreaCollider
		{
			get
			{
				return this.damageAreaCollider;
			}
		}

		private void Awake()
		{
			this.OwnerEntity = base.GetComponentInParent<Entity>();
			this.OnAwake();
		}

		protected virtual void OnAwake()
		{
		}

		private void Start()
		{
			this.OnStart();
		}

		protected virtual void OnStart()
		{
		}

		private void Update()
		{
			this.OnUpdate();
		}

		protected virtual void OnUpdate()
		{
		}

		public virtual void TakeDamage(Gameplay.GameControllers.Entities.Hit hit, bool force = false)
		{
			if (this.onTakeDamage != null)
			{
				this.onTakeDamage.Invoke(hit);
			}
		}

		public DamageArea.TakeDamageEvent onTakeDamage;

		public LayerMask enemyAttackAreaLayer;

		protected Entity Entity;

		public float RecoverTime = 0.5f;

		protected float DeltaRecoverTime;

		[SerializeField]
		protected Collider2D damageAreaCollider;

		[Serializable]
		public class TakeDamageEvent : UnityEvent<Gameplay.GameControllers.Entities.Hit>
		{
		}

		public enum DamageType
		{
			Normal,
			Heavy,
			Critical,
			Simple,
			Stunt,
			OptionalStunt
		}

		public enum DamageElement
		{
			Normal,
			Fire,
			Toxic,
			Magic,
			Lightning,
			Contact
		}

		public struct Hit
		{
			public Hit(Entity attackingEntity, Entity damagedEntity, DamageArea.DamageType damageType)
			{
				this.AttackingEntity = attackingEntity;
				this.DamagedEntity = damagedEntity;
				this.DamageType = damageType;
				this.Position = this.DamagedEntity.transform.position;
			}

			public Hit(Entity attackingEntity, Entity damagedEntity, DamageArea.DamageType damageType, Vector2 position)
			{
				this.AttackingEntity = attackingEntity;
				this.DamagedEntity = damagedEntity;
				this.DamageType = damageType;
				this.Position = position;
			}

			public Entity AttackingEntity;

			public Entity DamagedEntity;

			public DamageArea.DamageType DamageType;

			public Vector2 Position;
		}
	}
}
