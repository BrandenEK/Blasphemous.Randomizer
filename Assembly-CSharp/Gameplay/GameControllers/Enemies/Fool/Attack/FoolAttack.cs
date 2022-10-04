using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Fool.Attack
{
	public class FoolAttack : EnemyAttack
	{
		public AttackArea AttackArea { get; private set; }

		private Fool Fool { get; set; }

		private ContactDamage ContactDamage { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<Weapon>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Fool = (Fool)base.EntityOwner;
			this.ContactDamage = this.Fool.GetComponentInChildren<ContactDamage>();
			this.AttackArea.OnStay += this.AttackAreaOnStay;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.ContactDamage.IsTargetOverlapped && !this.Fool.Status.Dead)
			{
				this.contactDamageCoolDown -= Time.deltaTime;
				if (this.contactDamageCoolDown > 0f)
				{
					return;
				}
				this.contactDamageCoolDown = 0.1f;
				this.ContactAttack(Core.Logic.Penitent);
			}
			else
			{
				this.contactDamageCoolDown = 0.1f;
			}
		}

		private void AttackAreaOnStay(object sender, Collider2DParam e)
		{
			if (this.Fool.Behaviour.TurningAround || this.Fool.Status.Dead || !Core.Logic.Penitent.Status.IsGrounded)
			{
				return;
			}
			this.CurrentWeaponAttack();
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
			this._attackHit.DamageAmount = base.EntityOwner.Stats.Strength.Final;
			this._attackHit.Force = this.Force;
			this._attackHit.HitSoundId = this.HitSound;
			return this._attackHit;
		}

		private void OnDestroy()
		{
			this.AttackArea.OnEnter -= this.AttackAreaOnStay;
		}

		private Hit _attackHit;

		private bool _attackDone;

		private const float coolDown = 0.1f;

		private float contactDamageCoolDown = 0.1f;
	}
}
