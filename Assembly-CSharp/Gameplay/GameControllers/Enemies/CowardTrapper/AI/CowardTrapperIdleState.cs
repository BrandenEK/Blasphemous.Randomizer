using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.CowardTrapper.AI
{
	public class CowardTrapperIdleState : State
	{
		protected CowardTrapper CowardTrapper { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.CowardTrapper = machine.GetComponent<CowardTrapper>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.CowardTrapper.Behaviour.LookAtTarget(this.CowardTrapper.Target.transform.position);
		}

		public override void Update()
		{
			base.Update();
			this.CowardTrapper.Behaviour.StopMovement();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}
	}
}
