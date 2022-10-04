using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class HomingBonfireIdleState : State
	{
		private HomingBonfireBehaviour Behaviour { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Behaviour = machine.GetComponent<HomingBonfireBehaviour>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Behaviour.DeactivateHalfCharged();
			this.Behaviour.DeactivateFullCharged();
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
