using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Entities.Guardian
{
	public class GuardianPrayerAttack : Attack, IDirectAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this.guardianWeapon = base.GetComponentInChildren<GuardianPrayerWeapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.CreateHit();
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			this.guardianWeapon.Attack(this.guardianHit);
		}

		public void CreateHit()
		{
			this.guardianHit = new Hit
			{
				DamageAmount = base.EntityOwner.Stats.Strength.Final * Core.Logic.Penitent.Stats.DamageMultiplier.Final,
				AttackingEntity = base.EntityOwner.gameObject,
				DamageElement = DamageArea.DamageElement.Normal,
				DamageType = DamageArea.DamageType.Heavy,
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
			this.guardianHit.DamageAmount = (float)damage;
		}

		[EventRef]
		public string HitSound;

		private Hit guardianHit;

		private Weapon guardianWeapon;
	}
}
