using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.PietyMonster.Animation;
using Gameplay.GameControllers.Enemies.BellCarrier.Animator;
using Gameplay.GameControllers.Enemies.BellCarrier.Attack;
using Gameplay.GameControllers.Enemies.BellCarrier.Audio;
using Gameplay.GameControllers.Enemies.BellCarrier.IA;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellCarrier
{
	public class BellCarrier : Enemy, IDamageable
	{
		public BellCarrierAnimatorInyector AnimatorInyector { get; set; }

		public EnemyDamageArea DamageArea { get; set; }

		public BossBodyBarrier BodyBarrier { get; set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public bool IsVisible
		{
			get
			{
				return Entity.IsVisibleFrom(base.SpriteRenderer, Camera.main);
			}
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead)
			{
				return;
			}
			this.DamageArea.LastHit = hit;
			GameObject attackingEntity = hit.AttackingEntity;
			if (this.Status.Orientation == EntityOrientation.Left)
			{
				if (attackingEntity.transform.position.x < this.DamageArea.DamageAreaCollider.bounds.center.x)
				{
					this.Audio.PlayBellCarrierFrontHit();
					return;
				}
				this.Audio.PlayDamageSound();
				this.AnimatorInyector.TriggerColorFlash();
				this.BellCarrierBehaviour.Damage();
				this.SleepTimeByHit(hit);
				hit.HitSoundId = string.Empty;
				this.DamageArea.TakeDamage(hit, false);
			}
			else
			{
				if (attackingEntity.transform.position.x >= this.DamageArea.DamageAreaCollider.bounds.center.x)
				{
					this.Audio.PlayBellCarrierFrontHit();
					return;
				}
				this.Audio.PlayDamageSound();
				this.AnimatorInyector.TriggerColorFlash();
				this.BellCarrierBehaviour.Damage();
				this.SleepTimeByHit(hit);
				hit.HitSoundId = string.Empty;
				this.DamageArea.TakeDamage(hit, false);
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private void OnEnable()
		{
			if (base.Landing)
			{
				base.Landing = !base.Landing;
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AnimatorInyector = base.GetComponentInChildren<BellCarrierAnimatorInyector>();
			base.EnemyBehaviour = base.GetComponent<BellCarrierBehaviour>();
			this.BellCarrierBehaviour = (BellCarrierBehaviour)base.EnemyBehaviour;
			this.Inputs = base.GetComponentInChildren<NPCInputs>();
			base.Controller = base.GetComponentInChildren<PlatformCharacterController>();
			this.enemyAttack = base.GetComponentInChildren<BellCarrierAttack>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Audio = base.GetComponentInChildren<BellCarrierAudio>();
			this.MotionLerper = base.GetComponent<MotionLerper>();
			this.BodyBarrier = base.GetComponentInChildren<BossBodyBarrier>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			base.EnemyBehaviour.enabled = false;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			base.Target = penitent.gameObject;
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.EnemyBehaviour.enabled = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsVisibleOnCamera = this.IsVisible;
			this.Status.IsGrounded = base.Controller.IsGrounded;
			this.DamageArea.DamageAreaCollider.transform.localScale = Vector2.one;
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
			if (base.Landing)
			{
				this.EnablePhysics(true);
			}
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
			}
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			return this.enemyAttack;
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
			base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
		}

		protected override void EnablePhysics(bool enable)
		{
			if (enable)
			{
				if (!base.Controller.enabled)
				{
					base.Controller.enabled = true;
					base.EnemyBehaviour.StartBehaviour();
				}
			}
			else if (base.Controller.enabled)
			{
				base.Controller.enabled = false;
				base.EnemyBehaviour.PauseBehaviour();
			}
		}

		protected void SetGravityScale(float gravityScale)
		{
			if (base.Controller == null)
			{
				return;
			}
			base.Controller.PlatformCharacterPhysics.Gravity = new Vector3(0f, gravityScale, 0f);
		}

		protected IEnumerator RestoreGravityScale(float gravityScale, float lapse)
		{
			yield return new WaitForSeconds(lapse);
			this.SetGravityScale(gravityScale);
			yield break;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		public BellCarrierAudio Audio;

		public BellCarrierBehaviour BellCarrierBehaviour;

		public NPCInputs Inputs;

		public MotionLerper MotionLerper;
	}
}
