using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.MiriamPortal
{
	public class MiriamPortalPrayerAttack : Attack, IDirectAttack, IPaintAttackCollider
	{
		[HideInInspector]
		public bool DealsDamage
		{
			get
			{
				return this.dealsDamage;
			}
			set
			{
				this.dealsDamage = value;
				if (this.dealsDamage)
				{
					this.miriamPortalWeapon.StartAttacking(this.miriamPortalHit);
				}
				else
				{
					this.miriamPortalWeapon.StopAttacking();
				}
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.miriamPortalWeapon = base.GetComponentInChildren<MiriamPortalPrayerWeapon>();
			this.AttachShowScriptIfNeeded();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.CreateHit();
		}

		public void CreateHit()
		{
			this.miriamPortalHit = new Hit
			{
				DamageAmount = base.EntityOwner.Stats.Strength.Final * Core.Logic.Penitent.Stats.DamageMultiplier.Final * 0.5f,
				AttackingEntity = base.EntityOwner.gameObject,
				DamageElement = DamageArea.DamageElement.Normal,
				DamageType = DamageArea.DamageType.OptionalStunt,
				DestroysProjectiles = true,
				HitSoundId = this.HitSound
			};
		}

		public void SetDamage(int damage)
		{
			if (damage < 0)
			{
				return;
			}
			this.miriamPortalHit.DamageAmount = (float)damage;
		}

		public bool IsCurrentlyDealingDamage()
		{
			return this.DealsDamage;
		}

		public void AttachShowScriptIfNeeded()
		{
		}

		[EventRef]
		public string HitSound;

		private Hit miriamPortalHit;

		private MiriamPortalPrayerWeapon miriamPortalWeapon;

		private bool dealsDamage;
	}
}
