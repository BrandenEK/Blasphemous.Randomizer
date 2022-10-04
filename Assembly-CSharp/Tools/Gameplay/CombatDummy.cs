using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Tools.Gameplay
{
	public class CombatDummy : Enemy, IDamageable
	{
		private void Start()
		{
			this.dmgArea = base.GetComponentInChildren<EnemyDamageArea>();
		}

		public void Damage(Hit hit)
		{
			this.dmgArea.TakeDamage(hit, false);
			DamageArea.DamageType damageType = hit.DamageType;
			if (damageType != DamageArea.DamageType.Normal)
			{
				if (damageType != DamageArea.DamageType.Heavy)
				{
					if (damageType == DamageArea.DamageType.Critical)
					{
						Core.Audio.PlaySfxOnCatalog("PenitentCriticalEnemyHit");
					}
				}
				else
				{
					Core.Audio.PlaySfxOnCatalog("PenitentHeavyEnemyHit");
				}
			}
			else
			{
				Core.Audio.PlaySfxOnCatalog("PenitentSimpleEnemyHit");
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			throw new NotImplementedException();
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable)
		{
			throw new NotImplementedException();
		}

		private EnemyDamageArea dmgArea;
	}
}
