using System;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.Legionary.AI;
using Gameplay.GameControllers.Enemies.Legionary.Animator;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Legionary
{
	public class Legionary : Enemy, IDamageable
	{
		public LegionaryBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public NPCInputs Inputs { get; private set; }

		public StateMachine StateMachine { get; private set; }

		public MotionLerper MotionLerper { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public bool CanTakeDamage { get; set; }

		public LegionaryAnimator AnimatorInjector { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponentInChildren<PlatformCharacterController>();
			this.Behaviour = base.GetComponentInChildren<LegionaryBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.Inputs = base.GetComponentInChildren<NPCInputs>();
			this.StateMachine = base.GetComponentInChildren<StateMachine>();
			this.MotionLerper = base.GetComponentInChildren<MotionLerper>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.Behaviour.enabled = false;
			this.AnimatorInjector = base.GetComponentInChildren<LegionaryAnimator>();
			SpawnManager.OnPlayerSpawn += this.OnSpawn;
			EsdrasMeleeAttack spinAttack = this.SpinAttack;
			spinAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Combine(spinAttack.OnAttackGuarded, new Core.SimpleEvent(this.OnAttackGuarded));
		}

		private void OnSpawn(Penitent penitent)
		{
			SpawnManager.OnPlayerSpawn -= this.OnSpawn;
			base.Target = penitent.gameObject;
			this.Behaviour.enabled = true;
			this.Status.CastShadow = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsGrounded = base.Controller.IsGrounded;
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
			if (this.GuardHit(hit) || !this.CanTakeDamage)
			{
				return;
			}
			this.SleepTimeByHit(hit);
			this.Behaviour.Damage();
			this.DamageArea.TakeDamage(hit, false);
			this.ColorFlash.TriggerColorFlash();
			if (this.Behaviour.GotParry)
			{
				this.CanTakeDamage = false;
				this.Behaviour.GotParry = false;
			}
			if (this.Status.Dead)
			{
				this.AnimatorInjector.Death();
			}
		}

		private void OnAttackGuarded()
		{
			this.MotionLerper.StopLerping();
			this.MotionLerper.distanceToMove = 1f;
			this.MotionLerper.TimeTakenDuringLerp = 1f;
			Vector3 dir = (this.Status.Orientation != EntityOrientation.Right) ? (-base.transform.right) : base.transform.right;
			this.MotionLerper.StartLerping(dir);
		}

		public override void Parry()
		{
			base.Parry();
			this.Behaviour.GotParry = true;
			this.Behaviour.Parry();
			base.IsGuarding = false;
			this.CanTakeDamage = true;
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			EsdrasMeleeAttack spinAttack = this.SpinAttack;
			spinAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Remove(spinAttack.OnAttackGuarded, new Core.SimpleEvent(this.OnAttackGuarded));
		}

		[FoldoutGroup("Attacks", 0)]
		public EsdrasMeleeAttack LightAttack;

		[FoldoutGroup("Attacks", 0)]
		public EsdrasMeleeAttack SpinAttack;

		[FoldoutGroup("Attacks", 0)]
		public BossAreaSummonAttack LightningSummonAttack;
	}
}
