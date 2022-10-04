using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class IsidoraMeleeAttack : EnemyAttack, IDirectAttack, IPaintAttackCollider
	{
		public Hit WeaponHit { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.CreateHit();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<IsidoraWeapon>();
			this.AttachShowScriptIfNeeded();
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.CurrentEnemyWeapon.AttackAreas[0].OnEnter += this.OnEnterOrStayAttackArea;
			base.CurrentEnemyWeapon.AttackAreas[0].OnStay += this.OnEnterOrStayAttackArea;
		}

		private void OnEnterOrStayAttackArea(object sender, Collider2DParam e)
		{
			if (this.dealsDamage)
			{
				this.CurrentWeaponAttack();
			}
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this.WeaponHit);
		}

		private void OnGuardCallback(Hit obj)
		{
			if (this.OnAttackGuarded != null)
			{
				this.OnAttackGuarded();
			}
		}

		public void CreateHit()
		{
			this.WeaponHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = this.damage,
				DamageType = this.DamageType,
				DamageElement = this.DamageElement,
				Unnavoidable = this.unavoidable,
				ForceGuardSlideDirection = this.forceGuardSlideDirection,
				HitSoundId = this.HitSound,
				Force = this.Force,
				OnGuardCallback = new Action<Hit>(this.OnGuardCallback)
			};
		}

		public void SetDamage(int damage)
		{
			if (damage < 0)
			{
				return;
			}
			this.damage = (float)damage;
			this.CreateHit();
		}

		public bool IsCurrentlyDealingDamage()
		{
			return this.dealsDamage;
		}

		public void AttachShowScriptIfNeeded()
		{
		}

		[FoldoutGroup("Additional attack settings", 0)]
		public bool unavoidable;

		[FoldoutGroup("Additional attack settings", 0)]
		public bool forceGuardSlideDirection;

		[FoldoutGroup("Additional attack settings", 0)]
		public float damage;

		[HideInInspector]
		public bool dealsDamage;

		public Core.SimpleEvent OnAttackGuarded;
	}
}
