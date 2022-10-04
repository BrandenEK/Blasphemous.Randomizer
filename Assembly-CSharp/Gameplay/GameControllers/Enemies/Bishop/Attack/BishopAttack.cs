using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Enemies.Bishop.Attack
{
	public class BishopAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponent<Weapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._bishopHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				DamageType = DamageArea.DamageType.Normal,
				Force = this.Force,
				forceGuardslide = true,
				HitSoundId = this.HitSound,
				Unnavoidable = false
			};
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			base.CurrentEnemyWeapon.Attack(this._bishopHit);
		}

		private Hit _bishopHit;
	}
}
