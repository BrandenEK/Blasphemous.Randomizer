using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.Legionary.AI
{
	public class LegionaryWanderState : State
	{
		private protected Legionary Legionary { protected get; private set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Legionary = machine.GetComponent<Legionary>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
		}

		public override void Update()
		{
			base.Update();
			this.Legionary.Behaviour.Wander();
			if (this.Legionary.Behaviour.CanSeeTarget)
			{
				this.Legionary.StateMachine.SwitchState<LegionaryAttackState>();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}
	}
}
