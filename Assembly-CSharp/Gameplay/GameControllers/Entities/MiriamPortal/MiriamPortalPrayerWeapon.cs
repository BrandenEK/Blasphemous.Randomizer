using System;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Entities.MiriamPortal
{
	public class MiriamPortalPrayerWeapon : Weapon
	{
		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isDealingDamage)
			{
				this.Attack(this.hit);
			}
		}

		public override void Attack(Hit weaponHit)
		{
			List<IDamageable> damageableEntities = base.GetDamageableEntities();
			foreach (IDamageable item in this.alreadyDamaged)
			{
				damageableEntities.Remove(item);
			}
			if (damageableEntities.Count == 0)
			{
				return;
			}
			foreach (IDamageable item2 in damageableEntities)
			{
				this.alreadyDamaged.Add(item2);
			}
			base.AttackDamageableEntities(weaponHit);
		}

		public void StartAttacking(Hit weaponHit)
		{
			this.hit = weaponHit;
			this.isDealingDamage = true;
			this.alreadyDamaged.Clear();
		}

		public void StopAttacking()
		{
			this.isDealingDamage = false;
		}

		public override void OnHit(Hit weaponHit)
		{
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("HardFall");
			this.WeaponOwner.SleepTimeByHit(weaponHit);
		}

		private Hit hit;

		private bool isDealingDamage;

		private List<IDamageable> alreadyDamaged = new List<IDamageable>();
	}
}
