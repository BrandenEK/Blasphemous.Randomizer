using System;
using FMODUnity;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Menina.Attack
{
	public class MeninaGroundWave : Weapon
	{
		public AttackArea AttackArea { get; set; }

		public SpriteRenderer SpriteRenderer { get; set; }

		public Animator Animator { get; set; }

		public EntityMotionChecker MotionChecker { get; set; }

		public bool IsSpawned { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.Animator = base.GetComponent<Animator>();
			this.MotionChecker = base.GetComponent<EntityMotionChecker>();
			this.SpriteRenderer = base.GetComponent<SpriteRenderer>();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._timeSpawned += Time.deltaTime;
			if (this._timeSpawned < 0.1f || !this.IsSpawned)
			{
				return;
			}
			if (!this.MotionChecker.HitsFloor)
			{
				this.Recycle();
			}
		}

		public void Attack()
		{
			this.Attack(this.GetHit);
		}

		private Hit GetHit
		{
			get
			{
				return new Hit
				{
					AttackingEntity = base.gameObject,
					DamageType = DamageArea.DamageType.Normal,
					DamageAmount = this.WeaponOwner.Stats.Strength.Final * this.damageAmountFactor,
					HitSoundId = this.HitSoundId,
					Unnavoidable = false
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

		public void SetOwner(Entity owner)
		{
			this.WeaponOwner = owner;
			this.AttackArea.Entity = owner;
			this.MotionChecker.EntityOwner = owner;
			this.SpriteRenderer.flipX = (owner.Status.Orientation == EntityOrientation.Left);
		}

		public void TriggerWave()
		{
			if (!this.Animator)
			{
				return;
			}
			this.Animator.SetTrigger(MeninaGroundWave.Fire);
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this._timeSpawned = 0f;
			this.IsSpawned = true;
		}

		public void Recycle()
		{
			this.IsSpawned = false;
			base.Destroy();
		}

		private float _timeSpawned;

		private const float TimeOffset = 0.1f;

		[Range(0f, 1f)]
		public float damageAmountFactor = 0.5f;

		[EventRef]
		public string HitSoundId;

		private static readonly int Fire = Animator.StringToHash("FIRE");
	}
}
