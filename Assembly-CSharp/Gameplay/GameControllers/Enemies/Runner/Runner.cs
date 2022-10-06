using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.Runner.AI;
using Gameplay.GameControllers.Enemies.Runner.Animator;
using Gameplay.GameControllers.Enemies.Runner.Attack;
using Gameplay.GameControllers.Enemies.Runner.Audio;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Runner
{
	public class Runner : Enemy, IDamageable
	{
		public RunnerAttack Attack { get; private set; }

		public RunnerBehaviour Behaviour { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public RunnerAudio Audio { get; private set; }

		public RunnerAnimatorInjector AnimatorInjector { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public StateMachine StateMachine { get; private set; }

		public NPCInputs Input { get; private set; }

		public ContactDamage ContactDamage { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public EntityExecution EntExecution { get; private set; }

		private void OnEnable()
		{
			base.Landing = false;
			if (this.DamageArea)
			{
				this.DamageArea.DamageAreaCollider.enabled = true;
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Attack = base.GetComponentInChildren<RunnerAttack>();
			base.Controller = base.GetComponentInChildren<PlatformCharacterController>();
			this.Behaviour = base.GetComponentInChildren<RunnerBehaviour>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.Audio = base.GetComponentInChildren<RunnerAudio>();
			this.AnimatorInjector = base.GetComponentInChildren<RunnerAnimatorInjector>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.StateMachine = base.GetComponentInChildren<StateMachine>();
			this.ContactDamage = base.GetComponentInChildren<ContactDamage>();
			this.Input = base.GetComponent<NPCInputs>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
			this.EntExecution = base.GetComponentInChildren<EntityExecution>();
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			base.Target = penitent.gameObject;
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		protected override void OnStart()
		{
			base.OnStart();
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
			if (this.Execution(hit))
			{
				this.GetStun(hit);
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
