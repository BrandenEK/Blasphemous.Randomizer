using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Snake
{
	public class SnakeMeleeAttack : EnemyAttack, IDirectAttack, IPaintAttackCollider
	{
		public Hit WeaponHit { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.CreateHit();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<SnakeWeapon>();
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
			if (this.DealsDamage)
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
				DamageAmount = this.Damage,
				DamageType = this.DamageType,
				DamageElement = this.DamageElement,
				Unnavoidable = this.Unavoidable,
				Unparriable = this.Unparriable,
				Unblockable = this.Unblockable,
				ForceGuardSlideDirection = this.ForceGuardSlideDirection,
				CheckOrientationsForGuardslide = this.CheckOrientationsForGuardslide,
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
			this.Damage = (float)damage;
			this.CreateHit();
		}

		public bool IsCurrentlyDealingDamage()
		{
			return this.DealsDamage;
		}

		public void AttachShowScriptIfNeeded()
		{
		}

		[FoldoutGroup("Additional attack settings", 0)]
		public bool Unavoidable;

		[FoldoutGroup("Additional attack settings", 0)]
		public bool Unparriable;

		[FoldoutGroup("Additional attack settings", 0)]
		public bool Unblockable;

		[FoldoutGroup("Additional attack settings", 0)]
		public bool ForceGuardSlideDirection;

		[FoldoutGroup("Additional attack settings", 0)]
		public bool CheckOrientationsForGuardslide;

		[FoldoutGroup("Additional attack settings", 0)]
		public float Damage;

		[HideInInspector]
		public bool DealsDamage;

		public Core.SimpleEvent OnAttackGuarded;
	}
}
