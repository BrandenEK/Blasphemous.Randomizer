using System;
using Gameplay.GameControllers.Bosses.Snake;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.AlliedCherub.AI
{
	public class AlliedCherubAttackState : State
	{
		private AlliedCherub AlliedCherub { get; set; }

		private Entity Target { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.AlliedCherub = machine.GetComponent<AlliedCherub>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Target = this.AlliedCherub.Behaviour.Target;
		}

		private void ChaseEnemy()
		{
			if (this.Target == null)
			{
				this.AlliedCherub.Behaviour.OnTargetLost();
				return;
			}
			Vector2 v = this.lastTargetPos;
			float chasingEnemyElongation = this.AlliedCherub.Behaviour.ChasingEnemyElongation;
			float chasingEnemySpeed = this.AlliedCherub.Behaviour.ChasingEnemySpeed;
			DamageArea damageArea = this.GetDamageArea(this.Target);
			if (damageArea != null)
			{
				v = damageArea.DamageAreaCollider.bounds.center;
			}
			this.AlliedCherub.Behaviour.ChaseEntity(v, chasingEnemyElongation, chasingEnemySpeed);
		}

		public override void LateUpdate()
		{
			base.LateUpdate();
			this.AccquireTarget();
			if (!this.AlliedCherub.Behaviour.IsShooting() && this.AlliedCherub.Behaviour.CanAttack() && this.AlliedCherub.Behaviour.IsInAttackRange(this.lastTargetPos))
			{
				DamageArea damageArea = this.GetDamageArea(this.Target);
				if (damageArea != null)
				{
					this.AlliedCherub.Behaviour.ShootRailgun(damageArea.DamageAreaCollider);
				}
			}
			else if (!this.AlliedCherub.Behaviour.IsShooting())
			{
				this.ChaseEnemy();
			}
		}

		private DamageArea GetDamageArea(Entity target)
		{
			if (target is Snake)
			{
				return (target as Snake).GetActiveDamageArea();
			}
			return this.Target.GetComponent<Entity>().EntityDamageArea;
		}

		private void AccquireTarget()
		{
			if (this.Target != null)
			{
				this.lastTargetPos = this.Target.transform.position;
				DamageArea damageArea = this.GetDamageArea(this.Target);
				if (damageArea != null)
				{
					this.lastTargetPos = damageArea.DamageAreaCollider.bounds.center;
				}
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.Target = null;
		}

		public Vector2 lastTargetPos;
	}
}
