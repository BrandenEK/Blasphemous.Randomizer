using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.JarThrower.AI
{
	public class JarThrowerWanderState : State
	{
		protected JarThrower JarThrower { get; set; }

		protected float CurrentHealingLapse { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.JarThrower = machine.GetComponent<JarThrower>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.CurrentHealingLapse = this.JarThrower.Behaviour.HealingLapse;
		}

		public override void Update()
		{
			base.Update();
			this.CurrentHealingLapse -= Time.deltaTime;
			if (this.CurrentHealingLapse <= 0f)
			{
				this.JarThrower.Behaviour.Healing();
				this.ResetHealingTime();
			}
			if (this.JarThrower.Behaviour.IsHealing)
			{
				this.JarThrower.Behaviour.StopMovement();
			}
			else
			{
				this.JarThrower.Behaviour.Wander();
			}
			if (this.JarThrower.Behaviour.TargetSeen && !this.JarThrower.Behaviour.IsHealing && !this.JarThrower.Behaviour.TargetIsDead)
			{
				this.JarThrower.StateMachine.SwitchState<JarThrowerChaseState>();
			}
		}

		private void ResetHealingTime()
		{
			if (this.CurrentHealingLapse < this.JarThrower.Behaviour.HealingLapse)
			{
				this.CurrentHealingLapse = this.JarThrower.Behaviour.HealingLapse;
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.JarThrower.Behaviour.IsHealing = false;
		}
	}
}
