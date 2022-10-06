using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.WalkingTomb.AI;
using Gameplay.GameControllers.Enemies.WalkingTomb.Animator;
using Gameplay.GameControllers.Enemies.WalkingTomb.Attack;
using Gameplay.GameControllers.Enemies.WalkingTomb.Audio;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WalkingTomb
{
	public class WalkingTomb : Enemy, IDamageable
	{
		public NPCInputs Inputs { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public StateMachine StateMachine { get; private set; }

		public WalkingTombAttack Attack { get; private set; }

		public WalkingTombAnimatorInjector AnimatorInjector { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public WalkingTombBehaviour Behaviour { get; private set; }

		public WalkingTombAudio Audio { get; private set; }

		public EntityExecution EntExecution { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Attack = base.GetComponentInChildren<WalkingTombAttack>();
			this.Audio = base.GetComponentInChildren<WalkingTombAudio>();
			this.Inputs = base.GetComponentInChildren<NPCInputs>();
			base.Controller = base.GetComponentInChildren<PlatformCharacterController>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.Behaviour = base.GetComponentInChildren<WalkingTombBehaviour>();
			this.StateMachine = base.GetComponentInChildren<StateMachine>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.AnimatorInjector = base.GetComponentInChildren<WalkingTombAnimatorInjector>();
			this.EntExecution = base.GetComponentInChildren<EntityExecution>();
			this.Behaviour.enabled = false;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			base.Target = penitent.gameObject;
			this.Behaviour.enabled = true;
			this.Status.CastShadow = true;
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
				if (this.Execution(hit))
				{
					this.GetStun(hit);
					this.SleepTimeByHit(hit);
					return;
				}
				this.DamageArea.TakeDamage(hit, false);
				if (this.Status.Dead)
				{
					this.AnimatorInjector.Death();
					return;
				}
				this.ColorFlash.TriggerColorFlash();
			}
			this.SleepTimeByHit(hit);
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

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}
	}
}
