using System;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Stoners.Audio;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Stoners.Rock
{
	public class StonersRock : Weapon, IDamageable
	{
		public AttackArea AttackArea { get; private set; }

		public void Damage(Hit hit)
		{
			Core.Audio.PlaySfxOnCatalog("StonersRockHit");
			if (!this.IsBroken)
			{
				this.BreakRock();
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this._animator = base.GetComponentInChildren<Animator>();
			this.Audio = base.GetComponentInChildren<StonersRockAudio>();
			this.RigidBody = base.GetComponent<Rigidbody2D>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.OnEnter += this.AttackAreaOnEnter;
		}

		private void AttackAreaOnEnter(object sender, Collider2DParam collider2DParam)
		{
			GameObject gameObject = collider2DParam.Collider2DArg.gameObject;
			if (gameObject.CompareTag("Penitent"))
			{
				float @base = this.AttackingEntity.Stats.Strength.Base;
				Hit weapondHit = new Hit
				{
					AttackingEntity = this.AttackingEntity.gameObject,
					DamageType = DamageArea.DamageType.Normal,
					DamageAmount = @base,
					HitSoundId = this.hitSound
				};
				this.Attack(weapondHit);
			}
			this.BreakRock();
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		private void BreakRock()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.SetTrigger("BREAK");
			if (!this.IsBroken)
			{
				this.IsBroken = true;
			}
			if (this.AttackArea.WeaponCollider.enabled)
			{
				this.AttackArea.WeaponCollider.enabled = false;
			}
			this.RigidBody.velocity = Vector2.zero;
			this.Audio.BrokenRock();
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public void SetOwner(Enemy enemy)
		{
			this.AttackingEntity = enemy;
			this.WeaponOwner = enemy;
		}

		private void OnDestroy()
		{
			this.AttackArea.OnEnter -= this.AttackAreaOnEnter;
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		private Animator _animator;

		public Enemy AttackingEntity;

		public StonersRockAudio Audio;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string hitSound;

		public bool IsBroken;

		private Rigidbody2D RigidBody;
	}
}
