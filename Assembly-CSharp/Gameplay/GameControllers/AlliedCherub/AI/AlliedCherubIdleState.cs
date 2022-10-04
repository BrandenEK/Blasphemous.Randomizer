using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.AlliedCherub.AI
{
	public class AlliedCherubIdleState : State
	{
		private AlliedCherub AlliedCherub { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.AlliedCherub = machine.GetComponent<AlliedCherub>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.AlliedCherub.FloatingMotion.IsFloating = true;
		}

		public override void Update()
		{
			base.Update();
		}

		public override void LateUpdate()
		{
			base.LateUpdate();
			this.AlliedCherub.Behaviour.ChasingAlly();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.AlliedCherub.FloatingMotion.IsFloating = false;
		}
	}
}
