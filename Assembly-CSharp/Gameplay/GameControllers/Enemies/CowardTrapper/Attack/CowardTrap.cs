using System;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.CowardTrapper.AI;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CowardTrapper.Attack
{
	public class CowardTrap : Weapon, IDamageable
	{
		public AttackArea AttackArea { get; private set; }

		public Animator Animator { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea = base.AttackAreas[0];
			this.Animator = base.GetComponent<Animator>();
			this.AttackArea.OnEnter += this.OnEnter;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.UseLifeTime)
			{
				this._lifeTime += Time.deltaTime;
			}
			if (this._lifeTime >= this.SelfDestructTime)
			{
				this.Dispose();
			}
		}

		private Hit GetHit
		{
			get
			{
				return new Hit
				{
					DamageAmount = this.WeaponOwner.Stats.Strength.Final,
					AttackingEntity = base.gameObject,
					DamageType = DamageArea.DamageType.Normal,
					HitSoundId = this.PlayerDamage,
					Unnavoidable = true
				};
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
			if (this.AttackArea == null)
			{
				this.AttackArea = base.GetComponentInChildren<AttackArea>();
			}
			this.AttackArea.WeaponCollider.enabled = true;
			this._lifeTime = 0f;
			this._destroyed = false;
			Core.Audio.PlaySfx(this.GrowSound, 0f);
		}

		private void OnEnter(object sender, Collider2DParam e)
		{
			if (!e.Collider2DArg.CompareTag("Penitent"))
			{
				return;
			}
			e.Collider2DArg.GetComponentInParent<IDamageable>().Damage(this.GetHit);
			this.Dispose();
		}

		private void Dispose()
		{
			if (this._destroyed)
			{
				return;
			}
			this._destroyed = true;
			Core.Audio.PlaySfx(this.HitSound, 0f);
			this.Animator.SetTrigger("DESTROY");
			this.CowardTrapperBehaviour.RemoveTrap(this);
			this.AttackArea.WeaponCollider.enabled = false;
		}

		public void SetOwner(Entity owner)
		{
			this.CowardTrapperBehaviour = owner.GetComponent<CowardTrapperBehaviour>();
			this.AttackArea.Entity = owner;
			this.WeaponOwner = owner;
		}

		public void Damage(Hit hit)
		{
			Core.Audio.PlaySfx(this.DamageSound, 0f);
			this.Dispose();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return false;
		}

		[Tooltip("Destroy the trap after the destruction time")]
		public bool UseLifeTime = true;

		public float SelfDestructTime = 4f;

		private float _lifeTime;

		private bool _destroyed;

		protected CowardTrapperBehaviour CowardTrapperBehaviour;

		[EventRef]
		public string HitSound;

		[EventRef]
		public string GrowSound;

		[EventRef]
		public string DamageSound;

		[EventRef]
		public string PlayerDamage;
	}
}
