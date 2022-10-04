using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.ShieldMaiden.IA;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ShieldMaiden.Attack
{
	public class ShieldMaidenAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<ShieldMaidenWeapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._weaponHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				DamageType = this.DamageType,
				Force = this.Force,
				HitSoundId = this.HitSound,
				Unnavoidable = false
			};
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			Debug.Log("ShieldMaiden ATTACK with current weapon");
			base.CurrentEnemyWeapon.Attack(this._weaponHit);
		}

		private Hit _weaponHit;

		public Transform target;

		public ShieldMaidenBehaviour ownerBehaviour;
	}
}
