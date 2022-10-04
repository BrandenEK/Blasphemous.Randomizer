using System;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Firethrower.Attack
{
	public class FirethrowerAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<FirethrowerWeapon>();
			this.fireStartCollider = this.fireStartArea.GetComponent<Collider2D>();
			this.fireMainCollider = this.fireMainArea.GetComponent<Collider2D>();
			this.fireEndCollider = this.fireEndArea.GetComponent<Collider2D>();
			this.fireStartArea.OnStay += this.OnStayFireArea;
			this.fireMainArea.OnStay += this.OnStayFireArea;
			this.fireEndArea.OnStay += this.OnStayFireArea;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._weaponHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				DamageType = this.DamageType,
				DamageElement = this.DamageElement,
				Force = this.Force,
				HitSoundId = this.HitSound,
				Unnavoidable = false
			};
		}

		public void SetFireLevel(FirethrowerAttack.FIRE_LEVEL fireLevel)
		{
			switch (fireLevel)
			{
			case FirethrowerAttack.FIRE_LEVEL.NONE:
				this.fireStartCollider.enabled = false;
				this.fireMainCollider.enabled = false;
				this.fireEndCollider.enabled = false;
				break;
			case FirethrowerAttack.FIRE_LEVEL.START:
				this.fireStartCollider.enabled = true;
				this.fireMainCollider.enabled = false;
				this.fireEndCollider.enabled = false;
				break;
			case FirethrowerAttack.FIRE_LEVEL.GROWING:
				this.fireStartCollider.enabled = true;
				this.fireMainCollider.enabled = true;
				this.fireEndCollider.enabled = false;
				break;
			case FirethrowerAttack.FIRE_LEVEL.LOOP:
				this.fireStartCollider.enabled = true;
				this.fireMainCollider.enabled = true;
				this.fireEndCollider.enabled = true;
				break;
			}
		}

		private void OnStayFireArea(object sender, Collider2DParam e)
		{
			this.CurrentWeaponAttack();
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this._weaponHit);
		}

		private Hit _weaponHit;

		private Collider2D fireStartCollider;

		private Collider2D fireMainCollider;

		private Collider2D fireEndCollider;

		[Header("FireThrower specific")]
		public AttackArea fireStartArea;

		public AttackArea fireMainArea;

		public AttackArea fireEndArea;

		public enum FIRE_LEVEL
		{
			NONE,
			START,
			GROWING,
			LOOP
		}
	}
}
