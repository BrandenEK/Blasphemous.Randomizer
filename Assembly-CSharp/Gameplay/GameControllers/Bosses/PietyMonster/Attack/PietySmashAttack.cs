using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Attack
{
	public class PietySmashAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._pietyMonster = (PietyMonster)base.EntityOwner;
			base.CurrentEnemyWeapon = this.PietySmash;
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			if (base.CurrentEnemyWeapon == null || this._pietyMonster.Status.Dead)
			{
				return;
			}
			Hit weapondHit = new Hit
			{
				AttackingEntity = this._pietyMonster.gameObject,
				DamageType = this.DamageType,
				DamageAmount = this.DamageAmount,
				Force = this.Force,
				HitSoundId = this.HitSound
			};
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		public Weapon PietySmash;

		private PietyMonster _pietyMonster;

		[Range(0f, 2f)]
		public float DamageFactor = 1f;

		public float DamageAmount = 35f;
	}
}
