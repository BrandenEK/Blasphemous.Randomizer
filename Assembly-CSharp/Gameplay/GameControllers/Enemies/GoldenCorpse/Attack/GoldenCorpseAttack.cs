using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GoldenCorpse.Attack
{
	public class GoldenCorpseAttack : EnemyAttack
	{
		private ContactDamage _contactDamage { get; set; }

		private GoldenCorpse GoldenCorpse { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<Weapon>();
			this._contactDamage = base.EntityOwner.GetComponentInChildren<ContactDamage>();
			this.GoldenCorpse = (GoldenCorpse)base.EntityOwner;
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.SetContactDamage(this.ContactDamageAmount);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			bool isNapping = this.GoldenCorpse.Behaviour.isNapping;
			this._contactDamage.enabled = !isNapping;
			if (this._contactDamage.IsTargetOverlapped && !isNapping)
			{
				this.cooldown += Time.deltaTime;
				if (this.cooldown >= this.damageContactCooldown)
				{
					this.ContactAttack(Core.Logic.Penitent);
				}
			}
			else
			{
				this.cooldown = 0f;
			}
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			Hit simpleAttack = this.GetSimpleAttack();
			base.CurrentEnemyWeapon.Attack(simpleAttack);
		}

		private Hit GetSimpleAttack()
		{
			this._attackHit.AttackingEntity = base.EntityOwner.gameObject;
			this._attackHit.DamageType = DamageArea.DamageType.Normal;
			this._attackHit.DamageAmount = this.ContactDamageAmount;
			this._attackHit.Force = this.Force;
			this._attackHit.HitSoundId = this.HitSound;
			return this._attackHit;
		}

		private Hit _attackHit;

		private bool _attackDone;

		private float damageContactCooldown = 0.1f;

		private float cooldown;
	}
}
