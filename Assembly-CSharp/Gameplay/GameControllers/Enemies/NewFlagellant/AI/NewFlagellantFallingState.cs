using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.AI
{
	public class NewFlagellantFallingState : State
	{
		public NewFlagellant NewFlagellant { get; private set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.NewFlagellant = machine.GetComponent<NewFlagellant>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.NewFlagellant.AnimatorInyector.Falling();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.NewFlagellant.AnimatorInyector.Landing();
		}

		public override void Update()
		{
			base.Update();
			if (this.NewFlagellant.MotionChecker.HitsFloor)
			{
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantIdleState>();
			}
		}
	}
}
