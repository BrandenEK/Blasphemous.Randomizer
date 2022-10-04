using System;
using FMODUnity;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ExplodingEnemy.Attack
{
	public class ExplodingEnemyAttack : EnemyAttack
	{
		public AttackArea AttackArea { get; private set; }

		public bool HasExplode { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<Weapon>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.OnStay += this.OnStayAttackArea;
			this.AttackArea.OnExit += this.OnExitAttackArea;
			this._defaultAttackAreaOffset = new Vector2(this.AttackArea.WeaponCollider.offset.x, this.AttackArea.WeaponCollider.offset.y);
			this._defaultAttackAreaSize = new Vector2(this.AttackArea.WeaponCollider.bounds.size.x, this.AttackArea.WeaponCollider.bounds.size.y);
		}

		private void OnStayAttackArea(object sender, Collider2DParam e)
		{
			if (this._attackDone || this.HasExplode)
			{
				return;
			}
			this._attackDone = true;
			this.CurrentWeaponAttack();
		}

		private void OnExitAttackArea(object sender, Collider2DParam e)
		{
			this._attackDone = false;
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			ExplodingEnemy explodingEnemy = (ExplodingEnemy)base.EntityOwner;
			Hit weapondHit = (!explodingEnemy.AnimatorInyector.IsExploding) ? this.GetSimpleAttack() : this.GetAreaAttack();
			if (weapondHit.DamageType == DamageArea.DamageType.Heavy)
			{
				this.SetAttackAreaExplosionCollider();
			}
			base.CurrentEnemyWeapon.Attack(weapondHit);
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

		private Hit GetAreaAttack()
		{
			this._attackHit.AttackingEntity = base.EntityOwner.gameObject;
			this._attackHit.DamageType = DamageArea.DamageType.Heavy;
			this._attackHit.DamageAmount = base.EntityOwner.Stats.Strength.Final;
			this._attackHit.Force = this.Force;
			this._attackHit.HitSoundId = this.ExplosionHitSound;
			return this._attackHit;
		}

		private void SetAttackAreaExplosionCollider()
		{
			if (this.AttackArea == null)
			{
				return;
			}
			this.AttackArea.SetSize(this.ExplodingAttackAreaSize);
			this.AttackArea.SetOffset(this.ExplodingAttackAreaOffset);
		}

		public void SetDefaultAttackAreaSize()
		{
			if (this.AttackArea == null)
			{
				return;
			}
			this.AttackArea.SetSize(this._defaultAttackAreaSize);
			this.AttackArea.SetOffset(this._defaultAttackAreaOffset);
		}

		private void OnDestroy()
		{
			this.AttackArea.OnStay -= this.OnStayAttackArea;
			this.AttackArea.OnExit -= this.OnExitAttackArea;
		}

		private Hit _attackHit;

		[SerializeField]
		[BoxGroup("Explosion Settings", true, false, 0)]
		public Vector2 ExplodingAttackAreaOffset;

		private Vector2 _defaultAttackAreaOffset;

		[SerializeField]
		[BoxGroup("Explosion Settings", true, false, 0)]
		public Vector2 ExplodingAttackAreaSize;

		private Vector2 _defaultAttackAreaSize;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		protected string ExplosionHitSound;

		private bool _attackDone;
	}
}
