using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Attack
{
	public class ChargedAttackProjectile : Weapon
	{
		public MotionLerper MotionLerper { get; private set; }

		public Entity Owner
		{
			get
			{
				return this.WeaponOwner;
			}
			set
			{
				this.WeaponOwner = value;
			}
		}

		public AttackArea AttackArea { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._spriteRenderer = base.GetComponent<SpriteRenderer>();
			this._animator = base.GetComponent<Animator>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._chargedAttackHit = this.GetHit();
			this._dir = ((this.WeaponOwner.Status.Orientation != EntityOrientation.Right) ? (-Vector2.right) : Vector2.right);
			this.MotionLerper = base.GetComponentInChildren<MotionLerper>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.AttackArea.OnEnter += this.AttackAreaOnEnter;
			this.AttackArea.Entity = this.WeaponOwner;
			MotionLerper motionLerper = this.MotionLerper;
			motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			this.SetSpriteOrientation();
			this.LinearMotion();
		}

		private void AttackAreaOnEnter(object sender, Collider2DParam e)
		{
			if ((this.EnemyLayer.value & 1 << e.Collider2DArg.gameObject.layer) > 0)
			{
				if (!this.MotionLerper.IsLerping)
				{
					return;
				}
				this.Impact();
				this.MotionLerper.StopLerping();
				this.Attack(this._chargedAttackHit);
			}
			if ((this.BlockLayer.value & 1 << e.Collider2DArg.gameObject.layer) > 0)
			{
				if (!this.MotionLerper.IsLerping)
				{
					return;
				}
				this.Impact();
				this.MotionLerper.StopLerping();
			}
		}

		private void SetSpriteOrientation()
		{
			if (this._spriteRenderer != null)
			{
				this._spriteRenderer.flipX = (this.WeaponOwner.Status.Orientation == EntityOrientation.Left);
			}
		}

		public void LinearMotion()
		{
			if (this.MotionLerper == null)
			{
				return;
			}
			this.MotionLerper.StartLerping(this._dir);
		}

		private void OnLerpStop()
		{
			if (!this._hasImpact)
			{
				this.Vanish();
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

		public void Vanish()
		{
			this._animator.SetTrigger("VANISH");
		}

		public void Impact()
		{
			this._hasImpact = true;
			this._animator.SetTrigger("IMPACT");
		}

		public void Dispose()
		{
			base.Destroy();
		}

		public Hit GetHit()
		{
			float damageAmount = Core.Logic.Penitent.Stats.Strength.Final * Core.Logic.Penitent.PenitentAttack.HeavyAttackMultiplier;
			return new Hit
			{
				AttackingEntity = this.Owner.gameObject,
				DamageType = DamageArea.DamageType.Heavy,
				DamageAmount = damageAmount,
				Force = 0f,
				HitSoundId = this.HitSound
			};
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this._hasImpact = false;
			if (this.WeaponOwner == null)
			{
				this.WeaponOwner = Core.Logic.Penitent;
			}
			this._dir = ((this.WeaponOwner.Status.Orientation != EntityOrientation.Right) ? (-Vector2.right) : Vector2.right);
			this.SetSpriteOrientation();
			this.LinearMotion();
		}

		[FoldoutGroup("Motion Settings", true, 0)]
		public float Speed;

		[FoldoutGroup("Motion Settings", true, 0)]
		public float MaxSpeed;

		[FoldoutGroup("Motion Settings", true, 0)]
		public float Acceleration;

		[FoldoutGroup("Motion Settings", true, 0)]
		public float Range;

		[FoldoutGroup("Motion Settings", true, 0)]
		public float Lifetime;

		private float _currentLifeTime;

		[FoldoutGroup("Hit Settings", true, 0)]
		public LayerMask EnemyLayer;

		[FoldoutGroup("Hit Settings", true, 0)]
		public LayerMask BlockLayer;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		protected string HitSound;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		protected string VanishSound;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		protected string FlightSound;

		private SpriteRenderer _spriteRenderer;

		private Animator _animator;

		private Vector2 _dir;

		private bool _vanishing;

		private Hit _chargedAttackHit;

		private bool _hasImpact;
	}
}
