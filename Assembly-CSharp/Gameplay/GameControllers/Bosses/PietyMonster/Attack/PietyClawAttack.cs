using System;
using Gameplay.GameControllers.Bosses.PietyMonster.IA;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Attack
{
	public class PietyClawAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._pietyMonster = (PietyMonster)base.EntityOwner;
			base.CurrentEnemyWeapon = this.PietyClaw;
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
				HitSoundId = this.HitSound,
				Unnavoidable = this.Unavoidable
			};
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		public override void OnAttack(Hit hit)
		{
		}

		public PietyMonsterBehaviour PietyMonsterBehaviour;

		public Weapon PietyClaw;

		public Vector2 DamageAreaOffset;

		public Vector2 DamageAreaSize;

		public float DamageAmount = 25f;

		public bool Unavoidable = true;

		private PietyMonster _pietyMonster;

		[Range(0f, 2f)]
		public float DamageFactor = 1f;
	}
}
