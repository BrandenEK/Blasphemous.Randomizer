using System;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Roller.Attack
{
	public class AxeRollerAttack : EnemyAttack
	{
		private IProjectileAttack ProjectileAttack { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.ProjectileAttack = base.GetComponent<BossCurvedProjectileAttack>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.ProjectileAttack.SetProjectileWeaponDamage((int)base.EntityOwner.Stats.Strength.Final);
		}

		public void FireProjectile()
		{
			Vector2 target = this.ClampTarget(Core.Logic.Penitent.GetPosition());
			CurvedProjectile curvedProjectile = this.launcher.Shoot(target);
		}

		private Vector2 ClampTarget(Vector2 penitentPosition)
		{
			Vector2 result = penitentPosition;
			if (Vector2.Distance(base.transform.position, penitentPosition) > this.MaxProjectileDistance)
			{
				Vector2 vector = (penitentPosition - base.transform.position).normalized * this.MaxProjectileDistance;
				result = vector + base.transform.position;
			}
			return result;
		}

		public BossCurvedProjectileAttack launcher;

		public float LaunchHeight = 2f;

		public float MaxProjectileDistance;
	}
}
