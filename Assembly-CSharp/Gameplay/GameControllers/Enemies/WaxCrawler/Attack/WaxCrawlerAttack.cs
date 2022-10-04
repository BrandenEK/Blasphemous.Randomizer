using System;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WaxCrawler.Attack
{
	public class WaxCrawlerAttack : EnemyAttack
	{
		public bool EnableAttackArea
		{
			get
			{
				return this._attackArea.WeaponCollider.enabled;
			}
			set
			{
				Collider2D weaponCollider = this._attackArea.WeaponCollider;
				weaponCollider.enabled = value;
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this._waxCrawler = (WaxCrawler)base.EntityOwner;
			base.CurrentEnemyWeapon = base.EntityOwner.GetComponentInChildren<Weapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._attackArea = base.EntityOwner.GetComponentInChildren<AttackArea>();
			this._attackArea.OnEnter += this.AttackAreaOnEnter;
			this._attackArea.OnStay += this.AttackAreaOnStay;
			this._attackArea.OnExit += this.AttackAreaOnExit;
		}

		private void AttackAreaOnEnter(object sender, Collider2DParam collider2DParam)
		{
			this._accumulatedAttackTime = 0f;
			this.CurrentWeaponAttack(DamageArea.DamageType.Normal);
		}

		private void AttackAreaOnStay(object sender, Collider2DParam e)
		{
			this._accumulatedAttackTime += Time.deltaTime;
			if (this._accumulatedAttackTime >= this.AttackLapse)
			{
				this._accumulatedAttackTime = 0f;
				this.CurrentWeaponAttack(DamageArea.DamageType.Normal);
			}
		}

		private void AttackAreaOnExit(object sender, Collider2DParam e)
		{
			this._accumulatedAttackTime = 0f;
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			base.CurrentWeaponAttack(damageType);
			bool flag = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Appear") || base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Disappear");
			if (base.CurrentEnemyWeapon == null || base.EntityOwner.Status.Dead || flag)
			{
				return;
			}
			Hit weapondHit = default(Hit);
			float final = this._waxCrawler.Stats.Strength.Final;
			weapondHit.AttackingEntity = this._waxCrawler.gameObject;
			weapondHit.DamageType = damageType;
			weapondHit.DamageAmount = final;
			weapondHit.HitSoundId = this.HitSound;
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		private float _accumulatedAttackTime;

		private AttackArea _attackArea;

		public WaxCrawler _waxCrawler;

		public float AttackLapse = 0.75f;
	}
}
