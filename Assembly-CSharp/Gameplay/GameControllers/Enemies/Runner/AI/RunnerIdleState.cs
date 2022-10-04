using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.Runner.AI
{
	public class RunnerIdleState : State
	{
		protected Runner Runner { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Runner = machine.GetComponent<Runner>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Runner.Behaviour.StopMovement();
		}

		public override void Update()
		{
			base.Update();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}
	}
}
