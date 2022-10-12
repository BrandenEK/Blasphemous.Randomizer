using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Maikel.SteeringBehaviors;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.TrinityMinion.AI
{
	public class TrinityMinionBehaviour : EnemyBehaviour
	{
		public override void OnAwake()
		{
			base.OnAwake();
		}

		public override void OnStart()
		{
			base.OnStart();
			this.TrinityMinion = (TrinityMinion)this.Entity;
			float num = UnityEngine.Random.Range(-this.randomMaxSpeedRange, this.randomMaxSpeedRange);
			this.agent.maxSpeed += num;
			this.agent.maxForce += num;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.UpdateTarget();
		}

		private void LateUpdate()
		{
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this.Entity.Status.Dead)
			{
				return;
			}
			if (this.Entity.transform.position.x >= targetPos.x + 1f)
			{
				if (this.Entity.Status.Orientation != EntityOrientation.Left)
				{
					if (this.OnTurning != null)
					{
						this.OnTurning();
					}
					this.Entity.SetOrientation(EntityOrientation.Left, true, false);
					this.TrinityMinion.Collider.offset = new Vector2(-this.TrinityMinion.Collider.offset.x, this.TrinityMinion.Collider.offset.y);
				}
			}
			else if (this.Entity.transform.position.x < targetPos.x - 1f && this.Entity.Status.Orientation != EntityOrientation.Right)
			{
				if (this.OnTurning != null)
				{
					this.OnTurning();
				}
				this.Entity.SetOrientation(EntityOrientation.Right, true, false);
				this.TrinityMinion.Collider.offset = new Vector2(-this.TrinityMinion.Collider.offset.x, this.TrinityMinion.Collider.offset.y);
			}
		}

		public void Death()
		{
			this.agent.enabled = false;
			this.TrinityMinion.AnimatorInyector.Death();
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
			throw new NotImplementedException();
		}

		public override void Damage()
		{
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public void SetTarget(Transform t)
		{
			this._currentTarget = t;
		}

		private void UpdateTarget()
		{
			if (this._currentTarget != null)
			{
				this.seekBehavior.target = this._currentTarget.position + this.chaseOffset;
				this.LookAtTarget(this._currentTarget.position);
			}
		}

		private TrinityMinion TrinityMinion;

		private Transform _currentTarget;

		public Seek seekBehavior;

		public AutonomousAgent agent;

		public Vector2 chaseOffset;

		public float randomMaxSpeedRange = 2f;
	}
}
