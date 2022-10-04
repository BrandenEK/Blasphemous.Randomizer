using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.WallEnemy.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WallEnemy.AI
{
	public class WallEnemyBehaviour : EnemyBehaviour
	{
		public CollisionSensor CollisionSensor { get; private set; }

		public EnemyAttack WallEnemyAttack { get; protected set; }

		public override void OnStart()
		{
			base.OnStart();
			this.CollisionSensor = this.Entity.GetComponentInChildren<CollisionSensor>();
			this.CollisionSensor.SensorTriggerEnter += this.EnemyOnRange;
			this.CollisionSensor.SensorTriggerExit += this.EnemyOutOfRange;
			this.WallEnemyAttack = this.Entity.GetComponentInChildren<WallEnemyAttack>();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Entity.Status.Dead)
			{
				return;
			}
			this._currentAttackLapse += Time.deltaTime;
			if (this.Entity.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
			{
				this._currentAttackLapse = 0f;
			}
			if (base.PlayerSeen)
			{
				if (this._currentAttackLapse < this.AttackCooldown)
				{
					return;
				}
				this._currentAttackLapse = 0f;
				this.Attack();
			}
		}

		private void OnDestroy()
		{
			this.CollisionSensor.SensorTriggerEnter -= this.EnemyOnRange;
			this.CollisionSensor.SensorTriggerExit -= this.EnemyOutOfRange;
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			this.Entity.Animator.SetTrigger("ATTACK");
		}

		public override void Damage()
		{
			if (this.Entity.Status.Dead)
			{
				this.Entity.Animator.SetTrigger("DEATH");
			}
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		private void EnemyOnRange(Collider2D objectcollider)
		{
			if (!base.PlayerSeen)
			{
				base.PlayerSeen = true;
			}
		}

		private void EnemyOutOfRange(Collider2D objectcollider)
		{
			if (base.PlayerSeen)
			{
				base.PlayerSeen = !base.PlayerSeen;
			}
		}

		private float _currentAttackLapse;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackCooldown = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackDelay = 2f;
	}
}
