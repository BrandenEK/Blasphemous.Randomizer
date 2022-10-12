using System;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Sparks;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.UI.Others.UIGameLogic;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Attack
{
	public class PenitentSword : Weapon
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEventParam OnParry;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnSwordHit;

		protected override void OnAwake()
		{
			base.OnAwake();
			this._penitent = base.GetComponentInParent<Penitent>();
			this._swordSparkSpawner = base.GetComponentInChildren<SwordSparkSpawner>();
			this._bloodSpawner = base.GetComponentInChildren<BloodSpawner>();
			EnemyDamageArea.OnDamagedGlobal = (EnemyDamageArea.EnemyDamagedEvent)Delegate.Combine(EnemyDamageArea.OnDamagedGlobal, new EnemyDamageArea.EnemyDamagedEvent(this.OnEnemyDamaged));
			Entity.Death += this.OnEntityDead;
		}

		private void OnDestroy()
		{
			EnemyDamageArea.OnDamagedGlobal = (EnemyDamageArea.EnemyDamagedEvent)Delegate.Remove(EnemyDamageArea.OnDamagedGlobal, new EnemyDamageArea.EnemyDamagedEvent(this.OnEnemyDamaged));
			Entity.Death -= this.OnEntityDead;
		}

		protected override void OnStart()
		{
			this.SlashAnimator = base.GetComponentInChildren<SwordAnimatorInyector>();
			if (this.SlashAnimator == null)
			{
				UnityEngine.Debug.LogError("A sword slash animator is needed");
			}
		}

		private void OnEntityDead(Entity entity)
		{
			Enemy enemy = entity as Enemy;
			if (enemy)
			{
				Core.InventoryManager.OnEnemyKilled(enemy);
			}
		}

		private void OnEnemyDamaged(GameObject damaged, Hit hit)
		{
			if (this._penitent.obtainsFervour)
			{
				this._penitent.IncrementFervour(hit);
				PlayerFervour.Instance.ShowSpark();
			}
			Core.InventoryManager.OnDamageInflicted(hit);
		}

		public void GetSwordSparks(Vector2 position)
		{
			if (!this._swordSparkSpawner)
			{
				return;
			}
			this._swordSparkSpawner.GetSwordSpark(position);
		}

		public void GetSwordSparks(List<IDamageable> damageables, Hit swordHit)
		{
			if (!this._swordSparkSpawner)
			{
				return;
			}
			EntityOrientation orientation = this._penitent.Status.Orientation;
			for (int i = 0; i < damageables.Count; i++)
			{
				if (damageables[i] != null)
				{
					Collider2D componentInChildren = ((MonoBehaviour)damageables[i]).GetComponentInChildren<Collider2D>();
					if (componentInChildren)
					{
						Vector3 center = componentInChildren.bounds.center;
						if (damageables[i].SparkOnImpact())
						{
							this.SpawnSparks(center);
						}
						if (!swordHit.DontSpawnBlood && damageables[i].BleedOnImpact())
						{
							bool flag = true;
							Enemy enemy = damageables[i] as Enemy;
							if (enemy != null && enemy.IsHitGuarded(swordHit))
							{
								flag = false;
							}
							if (flag)
							{
								this.SpawnBlood(center, swordHit);
							}
						}
					}
				}
			}
		}

		public void SpawnSparks(Vector3 centerPos)
		{
			Vector3 vector = centerPos - base.AttackAreas[0].WeaponCollider.bounds.center;
			Vector3 position = base.AttackAreas[0].WeaponCollider.bounds.center + vector.normalized;
			this._swordSparkSpawner.GetSwordSpark(position);
		}

		public void SpawnBlood(Vector3 centerPos, Hit swordHit)
		{
			Vector3 position = centerPos + (centerPos - base.AttackAreas[0].WeaponCollider.bounds.center).normalized * 0.25f;
			this.SpawnSingleBloodFX(position, swordHit, this._penitent.Status.Orientation);
		}

		private BloodSpawner.BLOOD_FX_TYPES GetBloodType(Hit hit)
		{
			BloodSpawner.BLOOD_FX_TYPES result = BloodSpawner.BLOOD_FX_TYPES.SMALL;
			if (hit.DamageType != DamageArea.DamageType.Normal)
			{
				result = BloodSpawner.BLOOD_FX_TYPES.BIG;
			}
			else
			{
				float num = 0.33f;
				float num2 = UnityEngine.Random.Range(0f, 1f);
				if (num2 < num)
				{
					result = BloodSpawner.BLOOD_FX_TYPES.MEDIUM;
				}
			}
			return result;
		}

		private void SpawnSingleBloodFX(Vector3 position, Hit hit, EntityOrientation hitOrientation)
		{
			BloodSpawner.BLOOD_FX_TYPES bloodType = this.GetBloodType(hit);
			GameObject bloodFX = this._bloodSpawner.GetBloodFX(bloodType);
			bloodFX.transform.localScale = new Vector3((float)((hitOrientation != EntityOrientation.Right) ? -1 : 1), 1f, 1f);
			position += new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f), 0f);
			bloodFX.transform.position = position;
		}

		private Collider2D[] GetOverlappedDamageAreas()
		{
			Collider2D weaponCollider = base.AttackAreas[0].WeaponCollider;
			return Physics2D.OverlapAreaAll(weaponCollider.bounds.min, weaponCollider.bounds.max, this.TargetLayer);
		}

		public override void Attack(Hit swordHit)
		{
			List<IDamageable> damageableEntities = base.GetDamageableEntities();
			if (damageableEntities.Count < 1)
			{
				this._penitent.Rumble.UsePreset("AirBlow");
			}
			this.GetSwordSparks(damageableEntities, swordHit);
			base.AttackDamageableEntities(swordHit);
		}

		public override void OnHit(Hit hit)
		{
			if (this.OnSwordHit != null)
			{
				this.OnSwordHit();
			}
			this._penitent.PenitentMoveAnimations.FreezeEntity(hit.DamageType);
			if (this._penitent.CameraManager.ProCamera2DShake)
			{
				this._penitent.CameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
			}
			if (Core.InventoryManager.IsPrayerEquipped("PR04") && !this.WeaponOwner.Status.Dead)
			{
				this.WeaponOwner.Stats.Life.Current += this.PercentageDrained(hit.DamageAmount);
			}
			switch (hit.DamageType)
			{
			case DamageArea.DamageType.Normal:
				this._penitent.Rumble.UsePreset("SimpleHit");
				this._penitent.PenitentAttack.ChargeCombo();
				break;
			case DamageArea.DamageType.Heavy:
				this._penitent.Rumble.UsePreset("HeavyHit");
				break;
			}
		}

		private float PercentageDrained(float damageAmountInflicted)
		{
			return damageAmountInflicted * this._penitent.PenitentAttack.LifeDrainedByPrayerUse * 100f;
		}

		public bool SuccessParryChance(Hit hit)
		{
			bool result = false;
			Enemy component = hit.AttackingEntity.GetComponent<Enemy>();
			ParriableProjectile component2 = hit.AttackingEntity.GetComponent<ParriableProjectile>();
			if (component)
			{
				if (component.IsParryable && !hit.Unparriable && this.IsEnemySameDirection(component))
				{
					result = true;
					component.Parry();
					if (this.OnParry != null)
					{
						this.OnParry(component);
					}
				}
			}
			else if (component2 && !hit.Unparriable && this.IsEnemySameDirection(component))
			{
				result = true;
				component2.OnParry();
				if (this.OnParry != null)
				{
					this.OnParry(component);
				}
			}
			return result;
		}

		public bool IsEnemySameDirection(Entity enemy)
		{
			return !this.CheckParryEnemyDirection || (!(enemy == null) && this.WeaponOwner.Status.Orientation != enemy.Status.Orientation);
		}

		private Penitent _penitent;

		private SwordSparkSpawner _swordSparkSpawner;

		private BloodSpawner _bloodSpawner;

		public LayerMask TargetLayer;

		public SwordAnimatorInyector SlashAnimator;

		public bool CheckParryEnemyDirection = true;

		public static readonly Vector2 MaxSizeAttackCollider = new Vector2(3.5f, 2.5f);

		public static readonly Vector2 MinSizeAttackCollider = new Vector2(2.8f, 1f);

		public enum AttackType
		{
			Crouch,
			Basic1,
			Basic2,
			Combo,
			Air1,
			Air2,
			GroundUpward,
			AirUpward
		}

		public enum AttackColor
		{
			Default,
			Red
		}

		public struct SwordSlash
		{
			public SwordSlash(PenitentSword.AttackType type, int level, int color)
			{
				this.Type = type;
				this.Level = level;
				this.Color = color;
			}

			public PenitentSword.AttackType Type;

			public int Level;

			public int Color;
		}
	}
}
