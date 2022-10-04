using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.AlliedCherub.Attack
{
	public class AlliedCherubAttack : EnemyAttack
	{
		public AttackArea AttackArea { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<Weapon>();
			this.AttackArea.OnEnter += this.OnEnterAtackArea;
		}

		private void OnEnterAtackArea(object sender, Collider2DParam e)
		{
			base.CurrentEnemyWeapon.Attack(this.ContactHit);
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.ContactDamageAmount *= Core.Logic.Penitent.Stats.PrayerStrengthMultiplier.Final;
		}

		private void OnDestroy()
		{
			this.AttackArea.OnEnter -= this.OnEnterAtackArea;
		}
	}
}
