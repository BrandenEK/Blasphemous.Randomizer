using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras
{
	public class EsdrasMeleeAttack : EnemyAttack, IDirectAttack, IPaintAttackCollider
	{
		public Hit WeaponHit { get; private set; }

		public bool DealsDamage
		{
			get
			{
				return this.dealsDamage;
			}
			set
			{
				if (!this.dealsDamage && value)
				{
					base.CurrentEnemyWeapon.AttackAreas[0].OnStay += this.OnEnterOrStayAttackArea;
				}
				else if (this.dealsDamage && !value)
				{
					base.CurrentEnemyWeapon.AttackAreas[0].OnStay -= this.OnEnterOrStayAttackArea;
				}
				this.dealsDamage = value;
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.CreateHit();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<EsdrasWeapon>();
			this.AttachShowScriptIfNeeded();
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.CurrentEnemyWeapon.AttackAreas[0].OnEnter += this.OnEnterOrStayAttackArea;
		}

		private void OnEnterOrStayAttackArea(object sender, Collider2DParam e)
		{
			if (this.dealsDamage)
			{
				base.CurrentEnemyWeapon.AttackAreas[0].OnStay -= this.OnEnterOrStayAttackArea;
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
		public float damage;

		private bool dealsDamage;

		public Core.SimpleEvent OnAttackGuarded;
	}
}
