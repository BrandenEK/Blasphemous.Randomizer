using System;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.JarThrower.Attack
{
	public class JarWeapon : Weapon
	{
		public AttackArea AttackArea { get; private set; }

		public Animator Animator { get; private set; }

		public StraightProjectile Projectile { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.Animator = base.GetComponent<Animator>();
			this.Projectile = base.GetComponent<StraightProjectile>();
			this._jarImpactHit = new Hit
			{
				AttackingEntity = base.gameObject,
				DamageAmount = this.DamageAmount,
				DamageElement = DamageArea.DamageElement.Contact,
				DamageType = DamageArea.DamageType.Normal,
				Force = 0f,
				HitSoundId = this.ImpactSound
			};
			this.AttackArea.OnEnter += this.OnEnter;
		}

		private void OnEnter(object sender, Collider2DParam e)
		{
			if (e.Collider2DArg.CompareTag("Penitent"))
			{
				this.Attack(this._jarImpactHit);
			}
			else
			{
				this.Animator.SetTrigger("CRASH");
				Core.Audio.PlaySfx(this.BlastSound, 0f);
				this.Projectile.velocity = Vector2.zero;
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

		public void Recycle()
		{
			base.Destroy();
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
		}

		public float DamageAmount;

		[EventRef]
		public string ImpactSound;

		[EventRef]
		public string BlastSound;

		private Hit _jarImpactHit;
	}
}
