using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.JarThrower.AI
{
	public class JarThrowerChaseState : State
	{
		protected JarThrower JarThrower { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.JarThrower = machine.GetComponent<JarThrower>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.JarThrower.Behaviour.StopMovement();
			this.ResetChasingTime();
		}

		public override void Update()
		{
			base.Update();
			this.JarThrower.Behaviour.StopMovement();
			Vector3 position = this.JarThrower.Target.transform.position;
			float num = Vector2.Distance(this.JarThrower.transform.position, position);
			if (this.JarThrower.Behaviour.TargetSeen)
			{
				this.ResetChasingTime();
			}
			else
			{
				this.CurrentTimeChasing -= Time.deltaTime;
				if (this.CurrentTimeChasing <= 0f)
				{
					this.JarThrower.StateMachine.SwitchState<JarThrowerWanderState>();
				}
			}
			if (num <= this.JarThrower.Behaviour.AttackDistance)
			{
				this.JarThrower.StateMachine.SwitchState<JarThrowerAttackState>();
			}
			if (this.JarThrower.Behaviour.TargetSeen && this.JarThrower.Controller.IsGrounded)
			{
				this.JarThrower.Behaviour.Jump(position);
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private void ResetChasingTime()
		{
			if (this.CurrentTimeChasing < this.JarThrower.Behaviour.TimeChasing)
			{
				this.CurrentTimeChasing = this.JarThrower.Behaviour.TimeChasing;
			}
		}

		protected float CurrentTimeChasing;
	}
}
