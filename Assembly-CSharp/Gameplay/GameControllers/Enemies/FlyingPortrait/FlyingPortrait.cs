using System;
using Gameplay.GameControllers.Combat;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.FlyingPortrait.AI;
using Gameplay.GameControllers.Enemies.FlyingPortrait.Animator;
using Gameplay.GameControllers.Enemies.FlyingPortrait.Attack;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.FlyingPortrait
{
	public class FlyingPortrait : Enemy, IDamageable
	{
		public StateMachine StateMachine { get; private set; }

		public FlyingPortraitBehaviour Behaviour { get; private set; }

		public FlyingPortraitAnimator AnimatorInjector { get; private set; }

		public FlyingPortraitAttack Attack { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public DamageArea DamageArea { get; private set; }

		public new EntityExecution Execution { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.StateMachine = base.GetComponent<StateMachine>();
			this.Behaviour = base.GetComponentInChildren<FlyingPortraitBehaviour>();
			this.AnimatorInjector = base.GetComponentInChildren<FlyingPortraitAnimator>();
			this.Attack = base.GetComponentInChildren<FlyingPortraitAttack>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.DamageArea = base.GetComponentInChildren<DamageArea>();
			this.Execution = base.GetComponentInChildren<EntityExecution>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.DamageArea.DamageAreaCollider.enabled = false;
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
			if (this.Behaviour.GotParry)
			{
				this.Behaviour.GotParry = false;
			}
			this.ColorFlash.TriggerColorFlash();
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
			this.Behaviour.Execution();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}
	}
}
