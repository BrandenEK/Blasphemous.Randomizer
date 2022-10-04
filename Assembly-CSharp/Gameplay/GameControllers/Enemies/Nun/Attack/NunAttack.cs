using System;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Nun.Attack
{
	public class NunAttack : EnemyAttack
	{
		public RootMotionDriver RootMotion { get; set; }

		public AttackArea AttackArea { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.RootMotion = base.EntityOwner.GetComponentInChildren<RootMotionDriver>();
			this.AttackArea = base.EntityOwner.GetComponentInChildren<AttackArea>();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<NunWeapon>();
			this.AttackArea.OnEnter += this.OnEnterAttackArea;
			this._weaponHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				Force = this.Force,
				HitSoundId = this.HitSound,
				Unnavoidable = true
			};
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			if (base.EntityOwner.Status.Dead)
			{
				return;
			}
			this._weaponHit.DamageType = this.CurrentDamageType;
			this.CurrentWeaponAttack();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			bool flipX = base.EntityOwner.SpriteRenderer.flipX;
			Vector3 position = (!flipX) ? this.RootMotion.transform.position : this.RootMotion.ReversePosition;
			this.AttackArea.transform.position = position;
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this._weaponHit);
		}

		private void OnDestroy()
		{
			if (this.AttackArea)
			{
				this.AttackArea.OnEnter -= this.OnEnterAttackArea;
			}
		}

		public DamageArea.DamageType CurrentDamageType;

		private Hit _weaponHit;
	}
}
