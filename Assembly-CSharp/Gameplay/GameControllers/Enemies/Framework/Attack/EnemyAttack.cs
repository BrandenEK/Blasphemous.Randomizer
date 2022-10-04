using System;
using FMODUnity;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.Attack
{
	public class EnemyAttack : Attack
	{
		public Weapon CurrentEnemyWeapon { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.ContactHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageType = this.ContactDamageType,
				DamageElement = DamageArea.DamageElement.Contact,
				DamageAmount = this.ContactDamageAmount,
				Force = this.ContactAttackForce,
				Unparriable = true,
				HitSoundId = this.HitSoundByContact
			};
		}

		public override void ContactAttack(IDamageable damageable)
		{
			base.ContactAttack(damageable);
			damageable.Damage(this.ContactHit);
		}

		public void SetContactDamageType(DamageArea.DamageType damageType)
		{
			this.ContactHit.DamageType = damageType;
		}

		public void SetContactDamage(float contactDamage)
		{
			float damageAmount = (contactDamage > 0f) ? contactDamage : 0f;
			this.ContactHit.DamageAmount = damageAmount;
		}

		[Tooltip("Displacement that the attack will apply to the penitent position")]
		public float AttackGroundDisplacement = 5f;

		[Range(1f, 20f)]
		public float Force = 2f;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		protected string HitSound;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		protected string HitSoundByContact = "event:/SFX/Penitent/Damage/PenitentSimpleEnemyDamage";

		[FoldoutGroup("Contact Damage Settings", 0)]
		public DamageArea.DamageType ContactDamageType;

		[FoldoutGroup("Contact Damage Settings", 0)]
		public float ContactDamageAmount = 10f;

		[FoldoutGroup("Contact Damage Settings", 0)]
		public float ContactAttackForce;

		protected Hit ContactHit;
	}
}
