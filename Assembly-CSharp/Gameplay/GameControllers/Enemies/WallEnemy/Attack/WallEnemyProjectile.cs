using System;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WallEnemy.Attack
{
	public class WallEnemyProjectile : Weapon
	{
		public AttackArea AttackArea { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.SpriteRenderer = base.GetComponent<SpriteRenderer>();
			this.AttackArea.OnEnter += this.OnEnterAttackArea;
			this._rangedProjectileHit = new Hit
			{
				AttackingEntity = this.WeaponOwner.gameObject,
				DamageAmount = this.WeaponOwner.Stats.Strength.Final,
				DamageType = DamageArea.DamageType.Normal,
				Unnavoidable = false
			};
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			e.Collider2DArg.GetComponentInParent<IDamageable>().Damage(this._rangedProjectileHit);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.SpriteRenderer.isVisible)
			{
				base.Destroy();
			}
		}

		public override void Attack(Hit weapondHit)
		{
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
		}

		public void SetOwner(Entity owner)
		{
			this.WeaponOwner = owner;
			this.AttackArea.Entity = owner;
		}

		public void Dispose()
		{
			base.Destroy();
		}

		public SpriteRenderer SpriteRenderer;

		private Hit _rangedProjectileHit;
	}
}
