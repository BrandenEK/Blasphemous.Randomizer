using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellGhost.Attack
{
	public class BellGhostAttack : EnemyAttack
	{
		public bool EntityAttacked { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._bellGhost = (BellGhost)base.EntityOwner;
			base.CurrentEnemyWeapon = base.EntityOwner.GetComponentInChildren<Weapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._attackArea = base.EntityOwner.GetComponentInChildren<AttackArea>();
			if (this._bellGhost)
			{
				MotionLerper motionLerper = this._bellGhost.MotionLerper;
				motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
				MotionLerper motionLerper2 = this._bellGhost.MotionLerper;
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
				this.EntityAttacked = !this.EntityAttacked;
			}
		}

		private void AttackAreaOnStay(object sender, Collider2DParam e)
		{
			if (base.EntityOwner.Status.Dead || this.EntityAttacked)
			{
				return;
			}
			this.EntityAttacked = true;
			this.CurrentWeaponAttack(DamageArea.DamageType.Normal);
			if (this._bellGhost.MotionLerper.IsLerping && this._attackArea != null)
			{
				this.EnableWeaponAreaCollider = false;
			}
		}

		private void AttackAreaFloatingReposition()
		{
			if (this._bellGhost.AttackArea == null)
			{
				return;
			}
			Vector2 vector = this._bellGhost.FloatingMotion.transform.localPosition;
			this._bellGhost.AttackArea.transform.localPosition = vector;
		}

		public bool EnableWeaponAreaCollider
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

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			base.CurrentWeaponAttack(damageType);
			if (!base.CurrentEnemyWeapon)
			{
				return;
			}
			Hit weapondHit = new Hit
			{
				AttackingEntity = this._bellGhost.gameObject,
				DamageType = damageType,
				DamageAmount = this._bellGhost.Stats.Strength.Final,
				HitSoundId = this.HitSound
			};
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		private void OnLerpStart()
		{
			base.SetContactDamage(this._bellGhost.Stats.Strength.Final);
			if (this.EntityAttacked)
			{
				this.EntityAttacked = !this.EntityAttacked;
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
				this.EnableWeaponAreaCollider = true;
			}
		}

		private void OnDestroy()
		{
			if (this._bellGhost)
			{
				MotionLerper motionLerper = this._bellGhost.MotionLerper;
				motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Remove(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
				MotionLerper motionLerper2 = this._bellGhost.MotionLerper;
				motionLerper2.OnLerpStart = (Core.SimpleEvent)Delegate.Remove(motionLerper2.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
			}
			if (this._attackArea)
			{
				this._attackArea.OnStay -= this.AttackAreaOnStay;
				this._attackArea.OnExit -= this.AttackAreaOnExit;
			}
		}

		private AttackArea _attackArea;

		private BellGhost _bellGhost;
	}
}
