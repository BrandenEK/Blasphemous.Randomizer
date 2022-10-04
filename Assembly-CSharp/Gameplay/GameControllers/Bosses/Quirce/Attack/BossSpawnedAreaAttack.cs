using System;
using System.Collections.Generic;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class BossSpawnedAreaAttack : Weapon, IDirectAttack
	{
		public AttackArea AttackArea { get; set; }

		public SpriteRenderer SpriteRenderer { get; set; }

		public Animator Animator { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.Animator = base.GetComponentInChildren<Animator>();
			this.SpriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
			this._bottomHits = new RaycastHit2D[1];
			this.CreateHit();
			if (this.onHitEffect != null)
			{
				PoolManager.Instance.CreatePool(this.onHitEffect, 10);
			}
			this._cdCounter = this.firstTickDealy;
		}

		public void CreateHit()
		{
			this._hit = new Hit
			{
				AttackingEntity = base.gameObject,
				DamageType = this.damageType,
				DamageAmount = this.DamageAmount * this._hitStrength,
				HitSoundId = this.HitSoundId,
				Force = this.force,
				ThrowbackDirByOwnerPosition = true,
				Unnavoidable = this.unavoidable,
				Unparriable = this.unparriable,
				Unblockable = this.unblockable,
				DamageElement = this.damageElement
			};
		}

		public void SetDamage(int dmg)
		{
			this.DamageAmount = (float)dmg;
			this.CreateHit();
		}

		public void SetDamageStrength(float strength)
		{
			this._hitStrength = strength;
			this.CreateHit();
		}

		public void SetCallbackOnLoopFinish(Action callbackOnLoopFinish)
		{
			this.callbackOnLoopFinish = callbackOnLoopFinish;
		}

		private void PlaySound(string s)
		{
			if (s != string.Empty)
			{
				Core.Audio.PlaySfx(s, 0f);
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this._cdCounter > 0f && this._state == BossSpawnedAreaAttack.AREA_STATES.LOOPING)
			{
				this._cdCounter -= Time.deltaTime;
			}
			BossSpawnedAreaAttack.AREA_STATES state = this._state;
			if (state != BossSpawnedAreaAttack.AREA_STATES.PREPARATION)
			{
				if (state != BossSpawnedAreaAttack.AREA_STATES.LOOPING)
				{
					if (state != BossSpawnedAreaAttack.AREA_STATES.ENDING)
					{
					}
				}
				else
				{
					this.timer += Time.deltaTime;
					this.CheckTick();
					if (this.timer > this.actualLoopSeconds)
					{
						this._state = BossSpawnedAreaAttack.AREA_STATES.ENDING;
						this.OnEnterEnding();
						if (this.callbackOnLoopFinish != null)
						{
							this.callbackOnLoopFinish();
						}
						if (this.Animator != null)
						{
							this.Animator.SetTrigger("END");
						}
					}
				}
			}
			else
			{
				this.timer += Time.deltaTime;
				if (this.timer > this.actualPreparationSeconds)
				{
					this.timer = 0f;
					this._state = BossSpawnedAreaAttack.AREA_STATES.LOOPING;
					this.OnEnterDamageLoop();
					if (this.Animator != null)
					{
						this.Animator.SetTrigger("LOOP");
					}
				}
			}
		}

		protected virtual void OnEnterDamageLoop()
		{
			this.PlaySound(this.AttackLoopSoundId);
			if (this.screenshake)
			{
				Core.Logic.CameraManager.ProCamera2DShake.Shake(this.shakeDuration, this.shakeOffset, this.vibrato, 0.1f, 0f, default(Vector3), 0.06f, false);
			}
		}

		protected virtual void OnEnterEnding()
		{
			this.PlaySound(this.EndingSoundId);
		}

		private void SnapToGround()
		{
			Vector2 vector = base.transform.position;
			bool flag = Physics2D.LinecastNonAlloc(vector, vector + Vector2.down * this.RangeGroundDetection, this._bottomHits, this.groundMask) > 0;
			if (flag)
			{
				base.transform.position += Vector3.down * this._bottomHits[0].distance;
			}
			else
			{
				base.transform.position += Vector3.down * this.RangeGroundDetection;
			}
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
			if (this.onHitEffect == null)
			{
				return;
			}
			if (this.prohibitEffectInSomeScenes)
			{
				if (this.prohibitedScenesForEffect.Exists((string x) => Core.LevelManager.currentLevel.LevelName.StartsWith(x)))
				{
					return;
				}
			}
			foreach (IDamageable damageable in this.DamageableEntities)
			{
				PoolManager.Instance.ReuseObject(this.onHitEffect, (damageable as Component).transform.position, Quaternion.identity, false, 1);
			}
		}

		private void CheckTick()
		{
			if (this._cdCounter > 0f || base.GetDamageableEntities().Count == 0)
			{
				return;
			}
			this._cdCounter = this.timeBetweenTicks;
			this.Attack(this._hit);
		}

		public void SetOwner(Entity owner)
		{
			this.WeaponOwner = owner;
			if (this.AttackArea == null)
			{
				this.AttackArea = base.GetComponentInChildren<AttackArea>();
			}
			this.AttackArea.Entity = owner;
		}

		public void SetRemainingPreparationTime(float remainingTime)
		{
			if (this._state == BossSpawnedAreaAttack.AREA_STATES.PREPARATION)
			{
				this.timer = this.actualPreparationSeconds - remainingTime;
			}
		}

		public void SetCustomTimes(float newPreparationSeconds, float newLoopSeconds)
		{
			this.actualPreparationSeconds = newPreparationSeconds;
			this.actualLoopSeconds = newLoopSeconds;
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public void OnEndAnimationFinished()
		{
			this.Recycle();
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this._cdCounter = this.firstTickDealy;
			if (this.flipRandomly)
			{
				this.SpriteRenderer.flipX = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
			}
			if (this.Animator != null)
			{
				this.timer = 0f;
				this.Animator.ResetTrigger("LOOP");
				this.Animator.ResetTrigger("END");
				this.Animator.Play(this.firstState, 0, 0f);
			}
			this.SetCustomTimes(this.preparationSeconds, this.loopSeconds);
			this._state = BossSpawnedAreaAttack.AREA_STATES.PREPARATION;
			if (this.snapToGround)
			{
				this.SnapToGround();
			}
			this.PlaySound(this.PreparationLoopSoundId);
		}

		public void Recycle()
		{
			base.Destroy();
		}

		[FoldoutGroup("Design settings", 0)]
		public GameObject onHitEffect;

		[FoldoutGroup("Design settings", 0)]
		public bool prohibitEffectInSomeScenes;

		[FoldoutGroup("Design settings", 0)]
		[ShowIf("prohibitEffectInSomeScenes", true)]
		public List<string> prohibitedScenesForEffect = new List<string>();

		[FoldoutGroup("Design settings", 0)]
		public bool unavoidable;

		[FoldoutGroup("Design settings", 0)]
		public bool unparriable;

		[FoldoutGroup("Design settings", 0)]
		public bool unblockable;

		[FoldoutGroup("Design settings", 0)]
		public string firstState = "Intro";

		[FoldoutGroup("Design settings", 0)]
		public bool flipRandomly;

		[FoldoutGroup("Time settings", 0)]
		public float preparationSeconds = 1f;

		[FoldoutGroup("Time settings", 0)]
		public float loopSeconds = 3f;

		[FoldoutGroup("Time settings", 0)]
		public float timeBetweenTicks = 0.1f;

		[FoldoutGroup("Time settings", 0)]
		public float firstTickDealy;

		[FoldoutGroup("Damage settings", 0)]
		public float DamageAmount = 10f;

		[FoldoutGroup("Damage settings", 0)]
		public DamageArea.DamageType damageType;

		[FoldoutGroup("Damage settings", 0)]
		public DamageArea.DamageElement damageElement;

		[FoldoutGroup("Damage settings", 0)]
		public float force;

		[FoldoutGroup("Collision settings", 0)]
		public bool snapToGround;

		[FoldoutGroup("Collision settings", 0)]
		public LayerMask groundMask;

		[FoldoutGroup("Collision settings", 0)]
		public float RangeGroundDetection = 2f;

		[FoldoutGroup("Collision settings", 0)]
		public float yOffset;

		[FoldoutGroup("Sound settings", 0)]
		[EventRef]
		public string PreparationLoopSoundId;

		[FoldoutGroup("Sound settings", 0)]
		[EventRef]
		public string AttackLoopSoundId;

		[FoldoutGroup("Sound settings", 0)]
		[EventRef]
		public string EndingSoundId;

		[FoldoutGroup("Sound settings", 0)]
		[EventRef]
		public string HitSoundId;

		[FoldoutGroup("Design settings", 0)]
		public bool screenshake;

		[FoldoutGroup("Design settings", 0)]
		[ShowIf("screenshake", true)]
		public int vibrato = 40;

		[FoldoutGroup("Design settings", 0)]
		[ShowIf("screenshake", true)]
		public float shakeDuration = 0.5f;

		[FoldoutGroup("Design settings", 0)]
		[ShowIf("screenshake", true)]
		public Vector2 shakeOffset;

		private float actualPreparationSeconds = 1f;

		private float actualLoopSeconds = 1f;

		private BossSpawnedAreaAttack.AREA_STATES _state;

		private float _hitStrength = 1f;

		private float _cdCounter;

		private RaycastHit2D[] _bottomHits;

		private Hit _hit;

		private float timer;

		private Action callbackOnLoopFinish;

		private enum AREA_STATES
		{
			PREPARATION,
			LOOPING,
			ENDING
		}
	}
}
