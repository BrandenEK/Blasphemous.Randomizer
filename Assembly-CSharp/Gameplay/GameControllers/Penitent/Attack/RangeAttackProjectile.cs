using System;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent.Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Attack
{
	public class RangeAttackProjectile : Weapon
	{
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

		public RangeAttack RangeAttackAbility { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this._dir = ((this.WeaponOwner.Status.Orientation != EntityOrientation.Right) ? (-Vector2.right) : Vector2.right);
			this._projectileAnimator = base.GetComponent<Animator>();
			this.AttackArea = base.GetComponent<AttackArea>();
			this.AttackArea.Entity = this.WeaponOwner;
			this.RangeAttackAbility.ProjectileIsRunning = true;
			this.PlayFlightFx();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._currentLifeTime += Time.deltaTime;
			string lastUnlockedSkillId = this.RangeAttackAbility.LastUnlockedSkillId;
			if (lastUnlockedSkillId != null)
			{
				if (!(lastUnlockedSkillId == "RANGED_1"))
				{
					if (!(lastUnlockedSkillId == "RANGED_2"))
					{
						if (lastUnlockedSkillId == "RANGED_3")
						{
							this.BoomerangMotion();
						}
					}
					else
					{
						this.BoomerangMotion();
					}
				}
				else
				{
					this.BasicMotion();
				}
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if ((this.EnemyLayer.value & 1 << other.gameObject.layer) > 0)
			{
				this.Attack(this.GetHit());
			}
			if ((this.BlockLayer.value & 1 << other.gameObject.layer) > 0)
			{
				this._blocked = true;
				this.Speed = 0f;
				this.Vanish(0f);
			}
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if ((this.EnemyLayer.value & 1 << other.gameObject.layer) > 0)
			{
			}
		}

		public override void Attack(Hit weaponHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weaponHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		private Hit GetHit()
		{
			float damageAmount = this.RangeAttackDamageBySwordLevel.GetDamageBySwordLevel * Core.Logic.Penitent.Stats.RangedStrength.Final;
			return new Hit
			{
				AttackingEntity = base.gameObject,
				DamageType = DamageArea.DamageType.Normal,
				DamageAmount = damageAmount,
				Force = 0f,
				HitSoundId = this.HitSound
			};
		}

		private void BasicMotion()
		{
			if (this._blocked)
			{
				this.Vanish(0f);
			}
			else if (this._currentLifeTime >= this.Lifetime && !this._vanishing)
			{
				this.Vanish(0.3f);
			}
			else
			{
				this.LinearMotion();
			}
		}

		private void BoomerangMotion()
		{
			if (this._blocked)
			{
				this.Vanish(0f);
				this.ProjectileExplosion();
				return;
			}
			if (this._currentLifeTime >= this.Lifetime && !this._returning && !DOTween.IsTweening("Deceleration", false))
			{
				this.Deceleration(0.1f, new TweenCallback(this.OnDecelerationFinish));
			}
			if (this._returning)
			{
				this._returningTime += Time.deltaTime;
				if (this._returningTime >= this.Lifetime && !this._vanishing)
				{
					this.Vanish(0.3f);
				}
				if (this._vanishing)
				{
					this.LinearMotion();
				}
			}
			if (!this._vanishing)
			{
				this.LinearMotion();
			}
		}

		private void LinearMotion()
		{
			if (!DOTween.IsTweening("Deceleration", false))
			{
				this.Speed = Mathf.Min(this.Speed + this.Acceleration * Time.deltaTime, this.MaxSpeed);
			}
			if (!this._returning)
			{
				base.transform.Translate(this._dir * this.Speed * Time.deltaTime);
			}
			else
			{
				Vector3 a = Core.Logic.Penitent.DamageArea.Center();
				a.y = Core.Logic.Penitent.DamageArea.Center().y + 0.2f;
				base.transform.Translate((a - base.transform.position).normalized * this.Speed * Time.deltaTime);
			}
		}

		private void Deceleration(float decelerationTime, TweenCallback callback)
		{
			DOTween.To(delegate(float x)
			{
				this.Speed = x;
			}, this.Speed, 0f, decelerationTime).SetId("Deceleration").OnComplete(callback);
		}

		private void OnDecelerationFinish()
		{
			this._returning = true;
			this.ProjectileExplosion();
		}

		private void ProjectileExplosion()
		{
			if (this._isExplode)
			{
				return;
			}
			this._isExplode = true;
			if (this.RangeAttackAbility.LastUnlockedSkillId.Equals("RANGED_3"))
			{
				this.RangeAttackAbility.InstantiateExplosion(base.transform.position);
			}
		}

		public void PlayFlightFx()
		{
			if (this._rangeAttackFlightAudioInstance.isValid())
			{
				return;
			}
			this._rangeAttackFlightAudioInstance = Core.Audio.CreateEvent(this.FlightSound, default(Vector3));
			this._rangeAttackFlightAudioInstance.start();
		}

		public void StopFlightFx()
		{
			if (!this._rangeAttackFlightAudioInstance.isValid())
			{
				return;
			}
			this._rangeAttackFlightAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this._rangeAttackFlightAudioInstance.release();
			this._rangeAttackFlightAudioInstance = default(EventInstance);
		}

		public void Vanish(float vanishTime)
		{
			if (this._projectileAnimator == null)
			{
				return;
			}
			if (!this._vanishing)
			{
				this._vanishing = true;
			}
			this._projectileAnimator.SetTrigger("VANISH");
			this.AttackArea.WeaponCollider.enabled = false;
			if (!DOTween.IsTweening("Deceleration", false))
			{
				this.Deceleration(vanishTime, null);
			}
			Core.Audio.EventOneShotPanned(this.VanishSound, base.transform.position);
			this.StopFlightFx();
		}

		public void ResetMotionStatus()
		{
			this._currentLifeTime = 0f;
			this._returningTime = 0f;
			this.Speed = 0f;
			this._vanishing = false;
			this._returning = false;
			this._blocked = false;
			this._isExplode = false;
			this._dir = ((this.WeaponOwner.Status.Orientation != EntityOrientation.Right) ? (-Vector2.right) : Vector2.right);
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this.Owner = Core.Logic.Penitent;
			this.RangeAttackAbility = Core.Logic.Penitent.RangeAttack;
			this.RangeAttackAbility.ProjectileIsRunning = true;
			if (this.AttackArea)
			{
				this.AttackArea.WeaponCollider.enabled = true;
			}
			this.ResetMotionStatus();
			this.PlayFlightFx();
		}

		public void Store()
		{
			this.RangeAttackAbility.ProjectileIsRunning = false;
			base.Destroy();
		}

		private const string DecelerationTweenId = "Deceleration";

		private const float VanishingTime = 0.3f;

		[FoldoutGroup("Damage Settings", true, 0)]
		public RangeAttackBalance RangeAttackDamageBySwordLevel;

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

		private float _returningTime;

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

		private Vector2 _dir;

		private bool _vanishing;

		private bool _returning;

		private bool _blocked;

		private bool _isExplode;

		private Animator _projectileAnimator;

		private EventInstance _rangeAttackFlightAudioInstance;
	}
}
