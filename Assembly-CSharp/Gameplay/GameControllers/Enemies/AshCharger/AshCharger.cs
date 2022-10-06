using System;
using CreativeSpore.SmartColliders;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.AshCharger.AI;
using Gameplay.GameControllers.Enemies.AshCharger.Animator;
using Gameplay.GameControllers.Enemies.AshCharger.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.AshCharger
{
	public class AshCharger : Enemy, IDamageable
	{
		public NPCInputs Inputs { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public StateMachine StateMachine { get; private set; }

		public AshChargerAnimatorInyector AnimatorInyector { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public AshChargerBehaviour Behaviour { get; private set; }

		public AshChargerAudio Audio { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Audio = base.GetComponentInChildren<AshChargerAudio>();
			this.Inputs = base.GetComponentInChildren<NPCInputs>();
			base.Controller = base.GetComponentInChildren<PlatformCharacterController>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.Behaviour = base.GetComponentInChildren<AshChargerBehaviour>();
			this.StateMachine = base.GetComponentInChildren<StateMachine>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.AnimatorInyector = base.GetComponentInChildren<AshChargerAnimatorInyector>();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsGrounded = base.Controller.IsGrounded;
			if (base.Landing)
			{
				return;
			}
			base.Landing = true;
			this.SetPositionAtStart();
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
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

		public void Damage(Hit hit)
		{
			if (this.GuardHit(hit))
			{
				this.Audio.PlayGuardHit();
			}
			else
			{
				this.DamageArea.TakeDamage(hit, false);
				if (this.Status.Dead)
				{
					this.AnimatorInyector.Death();
					return;
				}
				this.ColorFlash.TriggerColorFlash();
			}
			this.SleepTimeByHit(hit);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}
	}
}
