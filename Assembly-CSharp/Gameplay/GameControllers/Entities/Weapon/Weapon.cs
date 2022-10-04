using System;
using System.Collections.Generic;
using Framework.Pooling;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Weapon
{
	public abstract class Weapon : PoolObject
	{
		private void Awake()
		{
			this._attackAreas = base.GetComponentsInChildren<AttackArea>();
			Entity componentInParent = base.GetComponentInParent<Entity>();
			if (componentInParent != null)
			{
				this.WeaponOwner = componentInParent;
			}
			this.DamageableEntities = new List<IDamageable>();
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

		private void FixedUpdate()
		{
			this.OnFixedUpdate();
		}

		protected virtual void OnFixedUpdate()
		{
		}

		public abstract void Attack(Hit weapondHit);

		public abstract void OnHit(Hit weaponHit);

		public virtual void SetHit(Hit hit)
		{
		}

		public AttackArea[] AttackAreas
		{
			get
			{
				return this._attackAreas;
			}
		}

		protected List<IDamageable> GetDamageableEntities()
		{
			if (this.AttackAreas == null)
			{
				return null;
			}
			for (int i = 0; i < this.AttackAreas.Length; i++)
			{
				GameObject[] array = this.AttackAreas[i].OverlappedEntities();
				if (array.Length > 0)
				{
					for (int j = 0; j < array.Length; j++)
					{
						IDamageable componentInParent = array[j].GetComponentInParent<IDamageable>();
						if (componentInParent != null)
						{
							this.DamageableEntities.Add(componentInParent);
						}
					}
				}
			}
			return this.DamageableEntities;
		}

		protected List<IDamageable> GetDamageableEntitiesWithCircleArea(CircleAttackArea circleArea)
		{
			GameObject[] array = circleArea.OverlappedEntities();
			if (array.Length <= 0)
			{
				return null;
			}
			for (int i = 0; i < array.Length; i++)
			{
				IDamageable componentInParent = array[i].GetComponentInParent<IDamageable>();
				if (componentInParent != null && !this.DamageableEntities.Contains(componentInParent))
				{
					this.DamageableEntities.Add(componentInParent);
				}
			}
			return this.DamageableEntities;
		}

		protected void AttackDamageableEntities(Hit weaponHit)
		{
			if (this.DamageableEntities == null)
			{
				return;
			}
			if (this.DamageableEntities.Count > 0)
			{
				this.OnHit(weaponHit);
				for (int i = 0; i < this.DamageableEntities.Count; i++)
				{
					this.DamageableEntities[i].Damage(weaponHit);
				}
				this.DamageableEntities.Clear();
			}
		}

		public void ClearDamageableEntities()
		{
			if (this.DamageableEntities.Count > 0)
			{
				this.DamageableEntities.Clear();
			}
		}

		public Entity WeaponOwner;

		protected AttackArea[] _attackAreas;

		protected List<IDamageable> DamageableEntities;
	}
}
