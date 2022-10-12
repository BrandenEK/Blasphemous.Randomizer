using System;
using System.Collections.Generic;
using DamageEffect;
using DG.Tweening;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Snake.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Snake
{
	public class Snake : Enemy, IDamageable, IPaintDamageableCollider
	{
		public bool IsRightHeadVisible
		{
			get
			{
				return this.HeadRightSprite.IsVisibleFrom(Camera.main);
			}
		}

		public bool IsLeftHeadVisible
		{
			get
			{
				return this.HeadLeftSprite.IsVisibleFrom(Camera.main);
			}
		}

		public SnakeBehaviour Behaviour { get; private set; }

		public SnakeAudio Audio { get; private set; }

		public SnakeAnimatorInyector SnakeAnimatorInyector { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponent<SnakeBehaviour>();
			this.Audio = base.GetComponent<SnakeAudio>();
			this.SnakeAnimatorInyector = base.GetComponent<SnakeAnimatorInyector>();
			this.TailAnimator = this.Tail.GetComponent<Animator>();
			this.TongueLeftOpenMouthRenderer = this.TongueLeftOpenMouth.GetComponentInParent<SpriteRenderer>();
			this.TongueRightOpenMouthRenderer = this.TongueRightOpenMouth.GetComponentInParent<SpriteRenderer>();
			this.TongueLeftIdleMouthRenderer = this.TongueLeftIdleMouth.GetComponentInParent<SpriteRenderer>();
			this.TongueRightIdleMouthRenderer = this.TongueRightIdleMouth.GetComponentInParent<SpriteRenderer>();
			this.baseGuardEffectOffset = this.GuardEffectOffset;
			this.AttachShowScriptIfNeeded();
		}

		internal DamageArea GetActiveDamageArea()
		{
			if (!this.IsCurrentlyDamageable())
			{
				return null;
			}
			if (this.Status.Orientation == EntityOrientation.Left)
			{
				if (this.TongueRightOpenMouth.DamageAreaCollider.enabled)
				{
					return this.TongueRightOpenMouth;
				}
				return this.TongueRightIdleMouth;
			}
			else
			{
				if (this.TongueLeftOpenMouth.DamageAreaCollider.enabled)
				{
					return this.TongueLeftOpenMouth;
				}
				return this.TongueLeftIdleMouth;
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Status.Dead)
			{
				return;
			}
			bool attacking = (this.Behaviour.CurrentMeleeAttackLeftHead && this.Behaviour.CurrentMeleeAttackLeftHead.DealsDamage) || (this.Behaviour.CurrentMeleeAttackRightHead && this.Behaviour.CurrentMeleeAttackRightHead.DealsDamage);
			Behaviour guardColliderLeftHead = this.GuardColliderLeftHead;
			bool enabled = !this.IsCurrentlyDamageable();
			this.GuardColliderRightHead.enabled = enabled;
			guardColliderLeftHead.enabled = enabled;
			EntityOrientation orientation = (!this.IsLeftHeadVisible) ? EntityOrientation.Left : EntityOrientation.Right;
			this.SetOrientation(orientation, false, false);
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(0.2f);
			sequence.OnComplete(delegate
			{
				Behaviour barrierLeftHead = this.BarrierLeftHead;
				bool enabled2 = !attacking;
				this.BarrierRightHead.enabled = enabled2;
				barrierLeftHead.enabled = enabled2;
			});
			sequence.Play<Sequence>();
		}

		public bool IsCurrentlyDamageable()
		{
			return this.TongueLeftOpenMouth.DamageAreaCollider.enabled || this.TongueRightOpenMouth.DamageAreaCollider.enabled || this.TongueLeftIdleMouth.DamageAreaCollider.enabled || this.TongueRightIdleMouth.DamageAreaCollider.enabled;
		}

		public void AttachShowScriptIfNeeded()
		{
		}

		public float GetHpPercentage()
		{
			return this.Stats.Life.Current / this.Stats.Life.CurrentMax;
		}

		public void Damage(Hit hit)
		{
			if (hit.AttackingEntity.name.StartsWith("Snake"))
			{
				return;
			}
			if (Core.Logic.Penitent.Stats.Life.Current <= 0f || this.Status.Dead)
			{
				return;
			}
			base.IsGuarding = !this.IsCurrentlyDamageable();
			this.SetGuardEffectOffset(hit);
			if (this.GuardHit(hit))
			{
				return;
			}
			if (this.WillDieByHit(hit))
			{
				hit.HitSoundId = this.FinalHit;
			}
			this.TakeDamageByVisibleHead(hit);
			if (this.Status.Dead)
			{
				this.TongueLeftOpenMouth.DamageAreaCollider.enabled = false;
				this.TongueRightOpenMouth.DamageAreaCollider.enabled = false;
				this.TongueLeftIdleMouth.DamageAreaCollider.enabled = false;
				this.TongueRightIdleMouth.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
			}
			else
			{
				this.DamageFlash();
				this.Behaviour.Damage(hit);
				this.SleepTimeByHit(hit);
			}
		}

		public Vector3 GetLeftDamagedPosition()
		{
			EnemyDamageArea enemyDamageArea = (!this.TongueLeftOpenMouth.DamageAreaCollider.enabled) ? this.TongueLeftIdleMouth : this.TongueLeftOpenMouth;
			return enemyDamageArea.DamageAreaCollider.bounds.center;
		}

		public Vector3 GetRightDamagedPosition()
		{
			EnemyDamageArea enemyDamageArea = (!this.TongueRightOpenMouth.DamageAreaCollider.enabled) ? this.TongueRightIdleMouth : this.TongueRightOpenMouth;
			return enemyDamageArea.DamageAreaCollider.bounds.center;
		}

		private void SetGuardEffectOffset(Hit hit)
		{
			GameObject gameObject = (!this.HeadLeftSprite.IsVisibleFrom(Camera.main)) ? this.HeadRight : this.HeadLeft;
			Vector3 a = Vector3.Lerp(hit.AttackingEntity.transform.position, gameObject.transform.position, 0.3f);
			this.GuardEffectOffset = this.baseGuardEffectOffset + (a - base.transform.position);
			this.GuardEffectOffset.x = this.GuardEffectOffset.x * -1f;
		}

		private void DamageFlash()
		{
			this.TongueLeftDamageEffect.Blink(0f, 0.07f);
			this.TongueRightDamageEffect.Blink(0f, 0.07f);
		}

		private void TakeDamageByVisibleHead(Hit hit)
		{
			if (this.TongueLeftOpenMouthRenderer.IsVisibleFrom(Camera.main))
			{
				this.TongueLeftOpenMouth.TakeDamage(hit, false);
			}
			else if (this.TongueRightOpenMouthRenderer.IsVisibleFrom(Camera.main))
			{
				this.TongueRightOpenMouth.TakeDamage(hit, false);
			}
			else if (this.TongueLeftIdleMouthRenderer.IsVisibleFrom(Camera.main))
			{
				this.TongueLeftIdleMouth.TakeDamage(hit, false);
			}
			else if (this.TongueRightIdleMouthRenderer.IsVisibleFrom(Camera.main))
			{
				this.TongueRightIdleMouth.TakeDamage(hit, false);
			}
		}

		public Vector3 GetPosition()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			throw new NotImplementedException();
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}

		public EnemyDamageArea TongueLeftOpenMouth;

		public EnemyDamageArea TongueRightOpenMouth;

		public EnemyDamageArea TongueLeftIdleMouth;

		public EnemyDamageArea TongueRightIdleMouth;

		public Collider2D GuardColliderLeftHead;

		public Collider2D GuardColliderRightHead;

		public Collider2D BarrierLeftHead;

		public Collider2D BarrierRightHead;

		public AnimationCurve SlowTimeCurve;

		public GameObject HeadLeft;

		public GameObject HeadRight;

		public GameObject Tail;

		public GameObject ChainLeft;

		public GameObject ChainRight;

		public List<SnakeSegmentVisualController> SnakeSegments;

		public SnakeSegmentsMovementController SnakeSegmentsMovementController;

		public SpriteRenderer HeadLeftSprite;

		public SpriteRenderer HeadRightSprite;

		public SpriteRenderer TongueLeftSprite;

		public SpriteRenderer TongueRightSprite;

		public DamageEffectScript TongueLeftDamageEffect;

		public DamageEffectScript TongueRightDamageEffect;

		public List<SpriteRenderer> ShadowMaskSprites = new List<SpriteRenderer>();

		[HideInInspector]
		public Animator TailAnimator;

		private SpriteRenderer TongueLeftOpenMouthRenderer;

		private SpriteRenderer TongueRightOpenMouthRenderer;

		private SpriteRenderer TongueLeftIdleMouthRenderer;

		private SpriteRenderer TongueRightIdleMouthRenderer;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string FinalHit;

		private Vector2 baseGuardEffectOffset;
	}
}
