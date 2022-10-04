using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Attack
{
	public class PietyBush : Weapon, IDamageable, IDirectAttack
	{
		public BoxCollider2D Collider { get; set; }

		public Animator Animator { get; set; }

		public ColorFlash Flash { get; set; }

		public AttackArea AttackArea { get; set; }

		public PietyMonster Owner { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.Collider = base.GetComponent<BoxCollider2D>();
			this.Animator = base.GetComponentInChildren<Animator>();
			this.Flash = base.GetComponent<ColorFlash>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (this.Owner)
			{
				this.bushHit = this.GetHit();
			}
			Core.Audio.PlaySfxOnCatalog("PietatSpitGrow");
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Penitent") && this.Owner != null)
			{
				Penitent componentInParent = other.GetComponentInParent<Penitent>();
				if (componentInParent.Status.Unattacable)
				{
					this.ForcedAttackToTarget(componentInParent, this.bushHit);
					this.DestroyBush();
				}
				else
				{
					this.Attack(this.bushHit);
				}
			}
			if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !other.CompareTag("NPC"))
			{
				this.DestroyBush();
			}
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
			this.DestroyBush();
		}

		public void SetOwner(PietyMonster pietyMonster)
		{
			this.Owner = pietyMonster;
			this.WeaponOwner = this.Owner;
			this.AttackArea.Entity = this.Owner;
		}

		private void ForcedAttackToTarget(Penitent penitent, Hit rootAttack)
		{
			penitent.DamageArea.TakeDamage(rootAttack, true);
		}

		private Hit GetHit()
		{
			return new Hit
			{
				AttackingEntity = base.gameObject,
				DamageAmount = this.DamageAmount,
				DamageType = DamageArea.DamageType.Normal,
				HitSoundId = this.HitSound
			};
		}

		public void Damage(Hit hit)
		{
			this.Flash.TriggerColorFlash();
			this.Life -= hit.DamageAmount;
			Core.Audio.PlaySfxOnCatalog("PietatSpitHit");
			if (this.Life > 0f)
			{
				return;
			}
			if (this.Collider.enabled)
			{
				this.Collider.enabled = false;
			}
			this.Animator.SetTrigger("DESTROY");
			Core.Audio.PlaySfxOnCatalog("PietatSpitDestroyHit");
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public void DestroyBush()
		{
			if (this.Collider.enabled)
			{
				this.Collider.enabled = false;
			}
			this.Animator.SetTrigger("DESTROY");
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		public void CreateHit()
		{
		}

		public void SetDamage(int damage)
		{
			this.bushHit.DamageAmount = (float)damage;
			this.DamageAmount = (float)damage;
		}

		private Hit bushHit;

		public float Life = 7f;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		protected string HitSound;

		public float DamageAmount;
	}
}
