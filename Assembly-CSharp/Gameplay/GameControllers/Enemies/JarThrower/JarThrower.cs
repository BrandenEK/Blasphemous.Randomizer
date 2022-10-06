using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.JarThrower.AI;
using Gameplay.GameControllers.Enemies.JarThrower.Animator;
using Gameplay.GameControllers.Enemies.JarThrower.Audio;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.JarThrower
{
	public class JarThrower : Enemy, IDamageable
	{
		public EnemyDamageArea DamageArea { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public NPCInputs Inputs { get; private set; }

		public StateMachine StateMachine { get; private set; }

		public JarThrowerAnimator AnimatorInjector { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public BossJumpAttack JumpAttack { get; private set; }

		public BossStraightProjectileAttack JarAttack { get; private set; }

		public JarThrowerBehaviour Behaviour { get; private set; }

		public JarThrowerAudio Audio { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponentInChildren<JarThrowerBehaviour>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.StateMachine = base.GetComponentInChildren<StateMachine>();
			this.AnimatorInjector = base.GetComponentInChildren<JarThrowerAnimator>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.JumpAttack = base.GetComponentInChildren<BossJumpAttack>();
			this.JarAttack = base.GetComponentInChildren<BossStraightProjectileAttack>();
			this.Audio = base.GetComponentInChildren<JarThrowerAudio>();
			this.Inputs = base.GetComponent<NPCInputs>();
			this.Behaviour.enabled = false;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			this.Behaviour.enabled = true;
			base.Target = penitent.gameObject;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Status.CastShadow = true;
		}

		private void Jump()
		{
			this.JumpAttack.Use(base.transform, Core.Logic.Penitent.transform.position);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsGrounded = base.Controller.IsGrounded;
			if (base.IsFalling)
			{
				base.IsFalling = !this.Status.IsGrounded;
			}
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
		}

		public void Damage(Hit hit)
		{
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				return;
			}
			this.SleepTimeByHit(hit);
			this.ColorFlash.TriggerColorFlash();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.CompareTag("SpikeTrap"))
			{
				return;
			}
			this.Kill();
			this.AnimatorInjector.EntityAnimator.Play("Death");
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
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

		public bool IsRunLanding;
	}
}
