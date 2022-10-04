using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.JarThrower.AI
{
	public class JarThrowerAttackState : State
	{
		protected JarThrower JarThrower { get; set; }

		protected float CurrentAttackCoolDown { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.JarThrower = machine.GetComponent<JarThrower>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.JarThrower.AnimatorInjector.Walk(false);
			this.ResetCoolDown();
		}

		public override void Update()
		{
			base.Update();
			this.CurrentAttackCoolDown -= Time.deltaTime;
			if (this.JarThrower.Behaviour.TargetSeen || this.JarThrower.IsAttacking)
			{
				Vector3 position = this.JarThrower.Target.transform.position;
				float num = Vector2.Distance(this.JarThrower.transform.position, position);
				if (this.JarThrower.Behaviour.CanWalk)
				{
					if (num >= this.JarThrower.Behaviour.ThrowingDistance)
					{
						this.JarThrower.Behaviour.Chase(this.JarThrower.Target.transform);
					}
					else
					{
						this.JarThrower.Behaviour.StopMovement();
						this.Attack();
					}
				}
				else
				{
					this.JarThrower.Behaviour.StopMovement();
					this.JarThrower.Behaviour.LookAtTarget(position);
					this.JarThrower.StateMachine.SwitchState<JarThrowerChaseState>();
				}
			}
			else
			{
				this.JarThrower.StateMachine.SwitchState<JarThrowerChaseState>();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private bool CanAttack
		{
			get
			{
				return this.CurrentAttackCoolDown <= 0f;
			}
		}

		private void Attack()
		{
			if (!this.CanAttack)
			{
				return;
			}
			this.JarThrower.Behaviour.Attack();
			this.ResetCoolDown();
		}

		private void ResetCoolDown()
		{
			if (this.CurrentAttackCoolDown < this.JarThrower.Behaviour.AttackCooldown)
			{
				this.CurrentAttackCoolDown = this.JarThrower.Behaviour.AttackCooldown;
			}
		}
	}
}
