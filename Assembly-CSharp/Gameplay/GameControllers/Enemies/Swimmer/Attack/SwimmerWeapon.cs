using System;
using FMODUnity;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Swimmer.Attack
{
	public class SwimmerWeapon : Weapon
	{
		protected AttackArea AttackArea { get; set; }

		private protected Rigidbody2D RigidBody { protected get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.RigidBody = base.GetComponent<Rigidbody2D>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.OnEnter += this.AttackAreaOnEnter;
			this._swimmerProjectileHit = new Hit
			{
				DamageType = DamageArea.DamageType.Normal,
				Unnavoidable = false,
				HitSoundId = this.HitSoundFx
			};
		}

		private void AttackAreaOnEnter(object sender, Collider2DParam e)
		{
			if (e.Collider2DArg.gameObject.CompareTag("Penitent"))
			{
				this._swimmerProjectileHit.AttackingEntity = this.WeaponOwner.gameObject;
				this._swimmerProjectileHit.DamageAmount = this.WeaponOwner.Stats.Strength.Final;
				this.Attack(this._swimmerProjectileHit);
			}
			base.Destroy();
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this.RigidBody.velocity = Vector2.zero;
		}

		public void SetOwner(Entity owner)
		{
			this.WeaponOwner = owner;
			this.AttackArea.Entity = owner;
		}

		private Hit _swimmerProjectileHit;

		[EventRef]
		public string HitSoundFx;
	}
}
