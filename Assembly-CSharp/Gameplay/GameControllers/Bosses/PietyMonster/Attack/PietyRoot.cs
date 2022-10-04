using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Attack
{
	public class PietyRoot : Weapon, IDamageable
	{
		private AttackArea AttackArea { get; set; }

		public PietyRootsManager Manager { get; set; }

		public bool LaunchAttack { get; set; }

		public bool CanAttack { get; set; }

		public Animator Animator { get; set; }

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		protected override void OnAwake()
		{
			this._spriteRenderer = base.GetComponent<SpriteRenderer>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.Animator = base.GetComponent<Animator>();
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if ((this.TargetLayer.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			if (this.LaunchAttack || !this.CanAttack)
			{
				return;
			}
			this.LaunchAttack = true;
			Hit weapondHit = new Hit
			{
				AttackingEntity = this.Manager.PietyMonster.gameObject,
				DamageAmount = this.Manager.RootDamage,
				DamageType = DamageArea.DamageType.Normal,
				HitSoundId = this.HitSound
			};
			this.Attack(weapondHit);
		}

		public void DisableSpriteRenderer()
		{
			if (this._spriteRenderer == null)
			{
				return;
			}
			if (this._spriteRenderer.enabled)
			{
				this._spriteRenderer.enabled = false;
			}
		}

		public void EnableSpriteRenderer()
		{
			if (this._spriteRenderer == null)
			{
				return;
			}
			if (!this._spriteRenderer.enabled)
			{
				this._spriteRenderer.enabled = true;
			}
		}

		public void AllowAttack()
		{
			if (!this.CanAttack)
			{
				this.CanAttack = true;
			}
		}

		public void DisallowAttack()
		{
			if (this.CanAttack)
			{
				this.CanAttack = !this.CanAttack;
			}
		}

		private void OnDisable()
		{
			if (this.LaunchAttack)
			{
				this.LaunchAttack = !this.LaunchAttack;
			}
		}

		public void Damage(Hit hit)
		{
			Core.Audio.PlaySfxOnCatalog("PietatSpitHit");
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public void PlayAttack()
		{
			if (this.Manager == null)
			{
				return;
			}
			this.Manager.PietyMonster.Audio.RootAttack();
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return false;
		}

		private SpriteRenderer _spriteRenderer;

		public LayerMask TargetLayer;

		[Tooltip("Damage factor based on entity damage base amount.")]
		[Range(0f, 1f)]
		public float DamageFactor;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		protected string HitSound;
	}
}
