using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.Attack
{
	public class BejeweledDivineBeam : Weapon
	{
		public AttackArea AttackArea { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.WeaponOwner = Object.FindObjectOfType<BejeweledSaintHead>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.Entity = this.WeaponOwner;
			this._beamHit = new Hit
			{
				AttackingEntity = base.transform.gameObject,
				DamageAmount = this.WeaponOwner.Stats.Strength.Final * 0.8f,
				DamageType = DamageArea.DamageType.Normal,
				Force = 0f,
				Unnavoidable = true,
				HitSoundId = this.DivineBeamHitFx
			};
		}

		public void FireAttack()
		{
			this.Attack(this._beamHit);
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public void Dispose()
		{
			base.Destroy();
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			Core.Audio.PlaySfx(this.DivineBeamFx, 0f);
		}

		private Hit _beamHit;

		[EventRef]
		public string DivineBeamFx;

		[EventRef]
		public string DivineBeamHitFx;
	}
}
