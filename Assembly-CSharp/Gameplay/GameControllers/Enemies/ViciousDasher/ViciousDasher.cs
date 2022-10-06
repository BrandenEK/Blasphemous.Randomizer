using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.ViciousDasher.AI;
using Gameplay.GameControllers.Enemies.ViciousDasher.Animator;
using Gameplay.GameControllers.Enemies.ViciousDasher.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using Plugins.GhostSprites2D.Scripts.GhostSprites;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ViciousDasher
{
	public class ViciousDasher : Enemy, IDamageable
	{
		public StateMachine StateMachine { get; private set; }

		public ViciousDasherBehaviour ViciousDasherBehaviour { get; private set; }

		public ViciousDasherAnimatorInyector AnimatorInjector { get; private set; }

		public MotionLerper MotionLerper { get; set; }

		public EnemyDamageArea DamageArea { get; set; }

		public ViciousDasherAttack Attack { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public bool IsTargetVisible { get; private set; }

		public EntityExecution EntExecution { get; private set; }

		public GhostSprites GhostSprites { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.StateMachine = base.GetComponent<StateMachine>();
			this.ViciousDasherBehaviour = base.GetComponent<ViciousDasherBehaviour>();
			this.AnimatorInjector = base.GetComponentInChildren<ViciousDasherAnimatorInyector>();
			this.MotionLerper = base.GetComponentInChildren<MotionLerper>();
			base.Controller = base.GetComponentInChildren<PlatformCharacterController>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Attack = base.GetComponentInChildren<ViciousDasherAttack>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.EntExecution = base.GetComponentInChildren<EntityExecution>();
			this.GhostSprites = base.GetComponentInChildren<GhostSprites>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Status.CastShadow = true;
			MotionLerper motionLerper = this.MotionLerper;
			motionLerper.OnLerpStart = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
			MotionLerper motionLerper2 = this.MotionLerper;
			motionLerper2.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper2.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsGrounded = base.Controller.IsGrounded;
			if (base.Target != null)
			{
				base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
				this.IsTargetVisible = this.VisionCone.CanSeeTarget(base.Target.transform, "Penitent", false);
			}
			else
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			if (base.Landing)
			{
				return;
			}
			base.Landing = true;
			this.SetPositionAtStart();
		}

		private void OnLerpStart()
		{
			this.GhostSprites.EnableGhostTrail = true;
			this.DamageArea.DamageAreaCollider.enabled = false;
		}

		private void OnLerpStop()
		{
			this.GhostSprites.EnableGhostTrail = false;
			this.DamageArea.DamageAreaCollider.enabled = true;
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
			if (this.MotionLerper.IsLerping)
			{
				return;
			}
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
			if (this.ViciousDasherBehaviour.GotParry)
			{
				this.ViciousDasherBehaviour.GotParry = false;
				this.ViciousDasherBehaviour.ResetParryRecover();
			}
			this.AnimatorInjector.IsParried(false);
			this.ColorFlash.TriggerColorFlash();
		}

		public override void Parry()
		{
			base.Parry();
			this.ViciousDasherBehaviour.Parry();
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
				this.AnimatorInjector.IsParried(false);
				this.ViciousDasherBehaviour.Execution();
			}
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
			return this.Attack;
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.MotionLerper.OnLerpStart != null)
			{
				MotionLerper motionLerper = this.MotionLerper;
				motionLerper.OnLerpStart = (Core.SimpleEvent)Delegate.Remove(motionLerper.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
			}
			if (this.MotionLerper.OnLerpStop != null)
			{
				MotionLerper motionLerper2 = this.MotionLerper;
				motionLerper2.OnLerpStop = (Core.SimpleEvent)Delegate.Remove(motionLerper2.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			}
		}
	}
}
