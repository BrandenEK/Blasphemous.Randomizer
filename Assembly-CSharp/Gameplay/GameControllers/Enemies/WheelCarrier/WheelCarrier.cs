using System;
using CreativeSpore.SmartColliders;
using DamageEffect;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.WheelCarrier.Animator;
using Gameplay.GameControllers.Enemies.WheelCarrier.Audio;
using Gameplay.GameControllers.Enemies.WheelCarrier.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Traits;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WheelCarrier
{
	public class WheelCarrier : Enemy, IDamageable
	{
		public WheelCarrierBehaviour Behaviour { get; private set; }

		public NPCInputs Input { get; private set; }

		public SmartPlatformCollider Collider { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public WheelCarrierAnimatorInyector AnimatorInyector { get; private set; }

		public EnemyAttack Attack { get; private set; }

		public EntityExecution EntExecution { get; private set; }

		public DamageEffectScript DamageEffect { get; private set; }

		public WheelCarrierAudio Audio { get; private set; }

		public VulnerablePeriodTrait VulnerablePeriod { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponent<WheelCarrierBehaviour>();
			this.Input = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Collider = base.GetComponent<SmartPlatformCollider>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Attack = base.GetComponentInChildren<EnemyAttack>();
			this.AnimatorInyector = base.GetComponentInChildren<WheelCarrierAnimatorInyector>();
			this.EntExecution = base.GetComponent<EntityExecution>();
			this.DamageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.Audio = base.GetComponentInChildren<WheelCarrierAudio>();
			this.VulnerablePeriod = base.GetComponentInChildren<VulnerablePeriodTrait>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			base.Target = Core.Logic.Penitent.gameObject;
			this.Status.CastShadow = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			this.Status.IsGrounded = base.Controller.IsGrounded;
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
				base.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
			base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
		}

		public void Damage(Hit hit)
		{
			if (this.Execution(hit))
			{
				this.GetStun(hit);
				return;
			}
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
				this.Audio.PlayDeath();
				return;
			}
			if (this.Behaviour.GotParry)
			{
				this.Behaviour.GotParry = false;
			}
			this.DamageEffect.Blink(0f, 0.1f);
			this.SleepTimeByHit(hit);
		}

		public override void Parry()
		{
			base.Parry();
			this.Behaviour.Parry();
		}

		public override void GetStun(Hit hit)
		{
			base.GetStun(hit);
			if (base.IsStunt)
			{
				return;
			}
			if (Mathf.Abs(base.Controller.SlopeAngle) < 1f)
			{
				Core.Audio.EventOneShotPanned(hit.HitSoundId, base.transform.position);
				this.Behaviour.Execution();
			}
		}

		public override EnemyFloorChecker EnemyFloorChecker()
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

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}
	}
}
