using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent.Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Attack
{
	public class RangeAttackExplosion : Weapon
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._attackArea = base.GetComponentInChildren<AttackArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._explosionHit = new Hit
			{
				AttackingEntity = this.WeaponOwner.gameObject,
				DamageType = DamageArea.DamageType.Normal,
				DamageAmount = this.RangeAttackBalance.GetDamageBySwordLevel * Core.Logic.Penitent.Stats.RangedStrength.Final * this.DamageFactor,
				HitSoundId = this.BlastHitSoundFx
			};
			this.WeaponOwner = Core.Logic.Penitent;
			this._attackArea.Entity = this.WeaponOwner;
			Core.Audio.EventOneShotPanned(this.BlastSoundFx, base.transform.position);
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public void Attack()
		{
			this.Attack(this._explosionHit);
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this.WeaponOwner = Core.Logic.Penitent;
			this._attackArea.Entity = this.WeaponOwner;
			Core.Audio.EventOneShotPanned(this.BlastSoundFx, base.transform.position);
		}

		public void Dispose()
		{
			base.Destroy();
		}

		private Hit _explosionHit;

		public float DamageFactor;

		private AttackArea _attackArea;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		protected string BlastSoundFx;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		protected string BlastHitSoundFx;

		public RangeAttackBalance RangeAttackBalance;
	}
}
