using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.AshCharger.AI
{
	public class AshChargeWaitingState : State
	{
		protected AshCharger AshCharger { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.AshCharger = machine.GetComponent<AshCharger>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
		}

		public override void Update()
		{
			base.Update();
			this.AshCharger.Behaviour.Wander();
			if (this.AshCharger.Behaviour.TargetIsDead)
			{
				return;
			}
			if (this.AshCharger.Behaviour.TargetIsInFront)
			{
				this.AshCharger.StateMachine.SwitchState<AshChargerAttackState>();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}
	}
}
