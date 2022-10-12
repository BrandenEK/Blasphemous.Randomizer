using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Guardian
{
	public class GuardianPrayerWeapon : Weapon
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (this.HitVfx)
			{
				PoolManager.Instance.CreatePool(this.HitVfx, 5);
			}
		}

		public override void Attack(Hit weapondHit)
		{
			List<IDamageable> damageableEntities = base.GetDamageableEntities();
			this.SpawnVfx(damageableEntities);
			base.AttackDamageableEntities(weapondHit);
			if (damageableEntities.Count > 0)
			{
				this.OnHit(weapondHit);
			}
		}

		public override void OnHit(Hit weaponHit)
		{
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("HardFall");
			this.WeaponOwner.SleepTimeByHit(weaponHit);
		}

		private void SpawnVfx(List<IDamageable> damageable)
		{
			foreach (Enemy enemy in from target in damageable
			select target as Enemy)
			{
				if (enemy == null)
				{
					break;
				}
				float y = enemy.EntityDamageArea.DamageAreaCollider.bounds.max.y - 0.25f;
				Vector2 v = new Vector2(enemy.transform.position.x, y);
				bool flipX = v.x - base.transform.position.x < 0f;
				if (this.HitVfx)
				{
					GameObject gameObject = PoolManager.Instance.ReuseObject(this.HitVfx, v, Quaternion.identity, false, 1).GameObject;
					SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						component.flipX = flipX;
					}
				}
			}
		}

		public GameObject HitVfx;
	}
}
