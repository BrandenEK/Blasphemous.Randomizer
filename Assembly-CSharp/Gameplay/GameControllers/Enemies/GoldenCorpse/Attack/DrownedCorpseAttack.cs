using System;
using Framework.Util;
using Gameplay.GameControllers.Enemies.DrownedCorpse;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Enemies.GoldenCorpse.Attack
{
	public class DrownedCorpseAttack : EnemyAttack
	{
		private ContactDamage _contactDamage { get; set; }

		private DrownedCorpse DrownedCorpse { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<Weapon>();
			this._contactDamage = base.EntityOwner.GetComponentInChildren<ContactDamage>();
			this.DrownedCorpse = (DrownedCorpse)base.EntityOwner;
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.CurrentEnemyWeapon.AttackAreas[0].OnStay += this.OnStayAttackArea;
			base.CurrentEnemyWeapon.AttackAreas[0].OnExit += this.OnOnExitAttackArea;
			base.SetContactDamage(this.ContactDamageAmount);
		}

		private void OnStayAttackArea(object sender, Collider2DParam e)
		{
			bool isChasing = this.DrownedCorpse.Behaviour.IsChasing;
			if (base.EntityOwner.Status.Dead || !isChasing)
			{
				return;
			}
			this.CurrentWeaponAttack();
		}

		private void OnOnExitAttackArea(object sender, Collider2DParam e)
		{
			this._attackDone = false;
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			if (this._attackDone)
			{
				return;
			}
			this._attackDone = true;
			Hit simpleAttack = this.GetSimpleAttack();
			base.CurrentEnemyWeapon.Attack(simpleAttack);
		}

		private Hit GetSimpleAttack()
		{
			this._attackHit.AttackingEntity = base.EntityOwner.gameObject;
			this._attackHit.DamageType = DamageArea.DamageType.Normal;
			this._attackHit.DamageAmount = base.EntityOwner.Stats.Strength.Final;
			this._attackHit.Force = this.Force;
			this._attackHit.forceGuardslide = true;
			this._attackHit.HitSoundId = this.HitSound;
			this._attackHit.OnGuardCallback = new Action<Hit>(this.OnGuardedAttack);
			return this._attackHit;
		}

		private void OnGuardedAttack(Hit h)
		{
			if (!this.DrownedCorpse.Behaviour)
			{
				return;
			}
			this.DrownedCorpse.Behaviour.OnGuarded();
		}

		private void OnDestroy()
		{
			base.CurrentEnemyWeapon.AttackAreas[0].OnEnter -= this.OnStayAttackArea;
			base.CurrentEnemyWeapon.AttackAreas[0].OnExit -= this.OnOnExitAttackArea;
		}

		private bool _attackDone;

		private Hit _attackHit;

		private float cooldown;

		private float damageContactCooldown = 0.1f;
	}
}
