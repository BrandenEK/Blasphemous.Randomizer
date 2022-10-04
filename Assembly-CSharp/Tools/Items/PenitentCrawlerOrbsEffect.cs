using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Enemies.BellGhost;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Tools.Items
{
	public class PenitentCrawlerOrbsEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			this._owner = Core.Logic.Penitent;
			this._crawlerOrbs = this._owner.GetComponentInChildren<PrayerUse>().crawlerBallsPrayer;
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
			float final = this._owner.Stats.PrayerStrengthMultiplier.Final;
			StraightProjectile straightProjectile = this._crawlerOrbs.Shoot(Vector2.right, Vector2.right * 0.01f, final);
			straightProjectile.GetComponent<ProjectileWeapon>().SetDamage(this.DamageAmount);
			straightProjectile = this._crawlerOrbs.Shoot(Vector2.left, Vector2.left * 0.01f, final);
			straightProjectile.GetComponent<ProjectileWeapon>().SetDamage(this.DamageAmount);
			return base.OnApplyEffect();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		private void Update()
		{
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
		}

		private Penitent _owner;

		private BossStraightProjectileAttack _crawlerOrbs;

		public int DamageAmount = 1;
	}
}
