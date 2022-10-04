using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Roller.Attack
{
	public class RollerAttack : EnemyAttack
	{
		private IProjectileAttack ProjectileAttack { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.ProjectileAttack = base.GetComponent<BossStraightProjectileAttack>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.ProjectileAttack.SetProjectileWeaponDamage((int)base.EntityOwner.Stats.Strength.Final);
		}

		public void FireProjectile()
		{
			Vector2 dir = (base.EntityOwner.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			AcceleratedProjectile acceleratedProjectile = this.launcher.Shoot(dir) as AcceleratedProjectile;
			acceleratedProjectile.SetBouncebackData(this.launcher.projectileSource, Vector2.zero, 10);
		}

		public BossStraightProjectileAttack launcher;

		public float LaunchHeight = 2f;
	}
}
