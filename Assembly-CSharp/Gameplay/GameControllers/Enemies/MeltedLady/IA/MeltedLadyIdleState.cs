using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.IA
{
	public class MeltedLadyIdleState : State
	{
		private protected FloatingLady MeltedLady { protected get; private set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.MeltedLady = machine.GetComponent<FloatingLady>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
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
