using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.Damage
{
	public class EnemyDamageArea : DamageArea
	{
		public bool GrantsFervour
		{
			get
			{
				return this.grantsFervour;
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this._enemyEntity = (Enemy)base.OwnerEntity;
		}

		public void SetOwner(Enemy enemy)
		{
			this._enemyEntity = enemy;
			base.OwnerEntity = enemy;
		}

		protected override void OnStart()
		{
			base.OnStart();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			this.SetPenitent();
		}

		private void SetPenitent()
		{
			if (Core.Logic.Penitent == null)
			{
				return;
			}
			this._penitent = Core.Logic.Penitent;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.DeltaRecoverTime += Time.deltaTime;
			if (!this._penitent)
			{
				this.SetPenitent();
			}
		}

		private void LateUpdate()
		{
			if (this.accumDamage > 1)
			{
				Debug.LogErrorFormat("Accumulated Damage greater than 1! Was {0} for {1} ", new object[]
				{
					this.accumDamage,
					this._enemyEntity.name
				});
			}
			this.accumDamage = 0;
		}

		public void ClearAccumDamage()
		{
			this.accumDamage = 0;
		}

		public override void TakeDamage(Gameplay.GameControllers.Entities.Hit hit, bool force = false)
		{
			if (this.accumDamage > 0)
			{
				return;
			}
			base.TakeDamage(hit, force);
			this.accumDamage++;
			base.LastHit = hit;
			this.TakeDamageAmount(hit);
			if (EnemyDamageArea.OnDamagedGlobal != null)
			{
				EnemyDamageArea.OnDamagedGlobal(this._enemyEntity.gameObject, hit);
			}
			if (this.OnDamaged != null)
			{
				this.OnDamaged(this._enemyEntity.gameObject, hit);
			}
			if (this._enemyEntity.Status.Dead)
			{
				if (this.damageAreaCollider.enabled)
				{
					this.damageAreaCollider.enabled = false;
				}
				if (this._penitent)
				{
					this._penitent.PenitentAttack.ResetCombo();
				}
				return;
			}
			if (this.DeltaRecoverTime < this.RecoverTime)
			{
				return;
			}
			this.DeltaRecoverTime = 0f;
			this._enemyEntity.entityCurrentState = EntityStates.Hurt;
		}

		private void TakeDamageAmount(Gameplay.GameControllers.Entities.Hit hit)
		{
			if (!this._enemyEntity)
			{
				return;
			}
			int num = (int)hit.DamageAmount;
			int num2 = (!this._enemyEntity.Status.Unattacable) ? num : 0;
			this._enemyEntity.Damage((float)num2, hit.HitSoundId);
		}

		public static EnemyDamageArea.EnemyDamagedEvent OnDamagedGlobal;

		private Enemy _enemyEntity;

		private Penitent _penitent;

		[SerializeField]
		private bool grantsFervour = true;

		public EnemyDamageArea.EnemyDamagedEvent OnDamaged;

		private int accumDamage;

		public delegate void EnemyDamagedEvent(GameObject damaged, Gameplay.GameControllers.Entities.Hit hit);
	}
}
