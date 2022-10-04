using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.HomingTurret.Attack;
using Gameplay.GameControllers.Entities.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.HomingTurret.AI
{
	public class HomingTurretBehaviour : EnemyBehaviour
	{
		private HomingTurret homingTurret { get; set; }

		public HomingTurretAttack TurretAttack { get; set; }

		private StateMachine StateMachine { get; set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.StateMachine = base.GetComponent<StateMachine>();
			this.TurretAttack = this.StateMachine.GetComponentInChildren<HomingTurretAttack>();
		}

		public override void OnStart()
		{
			base.OnStart();
			this.homingTurret = (HomingTurret)this.Entity;
			this.SwitchToState(HomingTurretBehaviour.TurretStates.Idle);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (this.CurrentState)
			{
			case HomingTurretBehaviour.TurretStates.Idle:
				this.Idle();
				break;
			case HomingTurretBehaviour.TurretStates.Attack:
				this.Attack();
				break;
			case HomingTurretBehaviour.TurretStates.Dead:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public override void Idle()
		{
			if (this.CanSeeTarget)
			{
				this.currentDetectionTime += Time.deltaTime;
				if (this.currentDetectionTime >= this.DetectionTime)
				{
					this.SwitchToState(HomingTurretBehaviour.TurretStates.Attack);
				}
			}
			else
			{
				this.currentDetectionTime = 0f;
			}
			if (this.Entity.Status.Dead)
			{
				this.SwitchToState(HomingTurretBehaviour.TurretStates.Dead);
			}
		}

		public void ChargeAttack()
		{
			this.homingTurret.AnimationInyector.ChargeAttack();
		}

		public void ReleaseAttack()
		{
			this.homingTurret.AnimationInyector.ReleaseAttack();
			this.hitStunCounter = 0;
		}

		public override void Attack()
		{
			this.currentDetectionTime = 0f;
			if (!this.CanSeeTarget)
			{
				this.SwitchToState(HomingTurretBehaviour.TurretStates.Idle);
			}
			if (this.Entity.Status.Dead)
			{
				this.SwitchToState(HomingTurretBehaviour.TurretStates.Dead);
			}
		}

		public void Dead()
		{
			this.homingTurret.AnimationInyector.Death();
		}

		public void Spawn()
		{
			this.homingTurret.SetOrientation((this.GetOrientationToTarget() != 1) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
			this.homingTurret.AnimationInyector.Spawn();
		}

		private int GetOrientationToTarget()
		{
			if (!Core.Logic.Penitent)
			{
				return 0;
			}
			return (int)Mathf.Sign((Core.Logic.Penitent.transform.position - base.transform.position).x);
		}

		private bool CanSeeTarget
		{
			get
			{
				return Core.Logic.Penitent && this.homingTurret.Vision.CanSeeTarget(Core.Logic.Penitent.transform, "Penitent", false);
			}
		}

		private void SwitchToState(HomingTurretBehaviour.TurretStates targetState)
		{
			this.CurrentState = targetState;
			if (targetState != HomingTurretBehaviour.TurretStates.Idle)
			{
				if (targetState != HomingTurretBehaviour.TurretStates.Attack)
				{
					if (targetState == HomingTurretBehaviour.TurretStates.Dead)
					{
						this.StateMachine.SwitchState<HomingTurretDeadState>();
					}
				}
				else
				{
					this.StateMachine.SwitchState<HomingTurretAttackState>();
				}
			}
			else
			{
				this.StateMachine.SwitchState<HomingTurretIdleState>();
			}
		}

		public override void Damage()
		{
			if (this.hitStunCounter < this.maxHitsInHitstun)
			{
				this.hitStunCounter++;
				this.homingTurret.AnimationInyector.Damage();
			}
		}

		public override void Wander()
		{
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void StopMovement()
		{
		}

		[FoldoutGroup("Attack Settings", 0)]
		public float DetectionTime;

		private float currentDetectionTime;

		[FoldoutGroup("Attack Settings", 0)]
		public float ReadyAttackTime;

		[FoldoutGroup("Attack Settings", 0)]
		public float AttackCooldown;

		private HomingTurretBehaviour.TurretStates CurrentState;

		private int hitStunCounter;

		public int maxHitsInHitstun = 3;

		private enum TurretStates
		{
			Idle,
			Attack,
			Dead
		}
	}
}
