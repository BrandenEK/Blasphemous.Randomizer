using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Amanecidas
{
	public class AmanecidasMeleeAttack : EnemyAttack, IDirectAttack
	{
		public Hit WeaponHit { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.CreateHit();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<AmanecidasWeapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.CurrentEnemyWeapon.AttackAreas[0].OnEnter += this.OnEnterAttackArea;
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			Debug.Log("SOMETHING ENTERS AREA");
			if (this.damageOnEnterArea)
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

		[FoldoutGroup("Additional attack settings", 0)]
		public bool unavoidable;

		[FoldoutGroup("Additional attack settings", 0)]
		public float damage;

		public bool damageOnEnterArea;

		public Core.SimpleEvent OnAttackGuarded;
	}
}
