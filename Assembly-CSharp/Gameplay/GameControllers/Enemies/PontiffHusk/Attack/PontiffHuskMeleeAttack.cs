using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk.Attack
{
	public class PontiffHuskMeleeAttack : EnemyAttack
	{
		public bool EntityAttacked { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._PontiffHuskMelee = (PontiffHuskMelee)base.EntityOwner;
			base.CurrentEnemyWeapon = base.EntityOwner.GetComponentInChildren<Weapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._attackArea = base.EntityOwner.GetComponentInChildren<AttackArea>();
			if (this._PontiffHuskMelee)
			{
				MotionLerper motionLerper = this._PontiffHuskMelee.MotionLerper;
				motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
				MotionLerper motionLerper2 = this._PontiffHuskMelee.MotionLerper;
				motionLerper2.OnLerpStart = (Core.SimpleEvent)Delegate.Combine(motionLerper2.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
			}
			if (this._attackArea)
			{
				this._attackArea.OnStay += this.AttackAreaOnStay;
				this._attackArea.OnExit += this.AttackAreaOnExit;
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.AttackAreaFloatingReposition();
		}

		private void AttackAreaOnExit(object sender, Collider2DParam e)
		{
			if (this.EntityAttacked)
			{
				this.EntityAttacked = false;
			}
		}

		private void AttackAreaOnStay(object sender, Collider2DParam e)
		{
			if (base.EntityOwner.Status.Dead || this.EntityAttacked)
			{
				return;
			}
			this.EntityAttacked = true;
			this.CurrentWeaponAttack(this.DamageType);
			if (this._PontiffHuskMelee.MotionLerper.IsLerping && this._attackArea != null)
			{
				this.EnableWeaponAreaCollider = false;
			}
		}

		private void AttackAreaFloatingReposition()
		{
			if (this._PontiffHuskMelee.AttackArea == null)
			{
				return;
			}
			Vector2 v = this._PontiffHuskMelee.FloatingMotion.transform.localPosition;
			this._PontiffHuskMelee.AttackArea.transform.localPosition = v;
		}

		public bool EnableWeaponAreaCollider
		{
			get
			{
				return this._attackArea.WeaponCollider.enabled;
			}
			set
			{
				this._attackArea.WeaponCollider.enabled = value;
			}
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			base.CurrentWeaponAttack(damageType);
			if (!base.CurrentEnemyWeapon)
			{
				return;
			}
			Hit weapondHit = new Hit
			{
				AttackingEntity = this._PontiffHuskMelee.gameObject,
				DamageType = damageType,
				DamageAmount = this._PontiffHuskMelee.Stats.Strength.Final,
				Force = this.Force,
				HitSoundId = this.HitSound
			};
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		private void OnLerpStart()
		{
			base.SetContactDamage(this._PontiffHuskMelee.Stats.Strength.Final);
			if (this.EntityAttacked)
			{
				this.EntityAttacked = false;
			}
			if (!this._attackArea)
			{
				return;
			}
			if (!this.EnableWeaponAreaCollider)
			{
				this.EnableWeaponAreaCollider = true;
			}
		}

		private void OnLerpStop()
		{
			base.SetContactDamage(this.ContactDamageAmount);
			if (!this._attackArea)
			{
				return;
			}
			if (!this.EnableWeaponAreaCollider)
			{
				this.EnableWeaponAreaCollider = false;
			}
		}

		private void OnDestroy()
		{
			if (this._PontiffHuskMelee)
			{
				MotionLerper motionLerper = this._PontiffHuskMelee.MotionLerper;
				motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Remove(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
				MotionLerper motionLerper2 = this._PontiffHuskMelee.MotionLerper;
				motionLerper2.OnLerpStart = (Core.SimpleEvent)Delegate.Remove(motionLerper2.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
			}
			if (this._attackArea)
			{
				this._attackArea.OnStay -= this.AttackAreaOnStay;
				this._attackArea.OnExit -= this.AttackAreaOnExit;
			}
		}

		private AttackArea _attackArea;

		private PontiffHuskMelee _PontiffHuskMelee;
	}
}
