using System;
using FMODUnity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Amanecidas
{
	public class AmanecidaBeamAttack : Weapon, IDirectAttack
	{
		public AttackArea AttackArea { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.WeaponOwner = ((!(this.BeamAttackOwner == null)) ? this.BeamAttackOwner : Object.FindObjectOfType<Amanecidas>());
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.Entity = this.WeaponOwner;
			this.CreateHit();
		}

		public void FireAttack()
		{
			this.Attack(this._beamHit);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.tickCounter += Time.deltaTime;
			if (this.tickCounter > 0.15f)
			{
				this.tickCounter = 0f;
				this.FireAttack();
			}
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
		}

		public void CreateHit()
		{
			this._beamHit = new Hit
			{
				AttackingEntity = base.transform.gameObject,
				DamageAmount = (float)this.damage,
				DamageType = DamageArea.DamageType.Normal,
				DamageElement = DamageArea.DamageElement.Magic,
				Force = 0f,
				Unnavoidable = true,
				HitSoundId = this.BeamAttackSoundFx
			};
		}

		public void SetDamage(int damage)
		{
			if (damage < 0)
			{
				return;
			}
			this.damage = damage;
			this.CreateHit();
		}

		private Hit _beamHit;

		[EventRef]
		public string DivineBeamFx;

		public float tickCounter;

		public int damage = 30;

		private const float TIME_BETWEEN_TICKS = 0.15f;

		public Entity BeamAttackOwner;

		[EventRef]
		public string BeamAttackSoundFx;
	}
}
