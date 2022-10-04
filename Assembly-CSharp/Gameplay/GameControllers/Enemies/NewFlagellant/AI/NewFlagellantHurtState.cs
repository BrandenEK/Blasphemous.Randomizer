using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.AI
{
	public class NewFlagellantHurtState : State
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
			this.NewFlagellant.NewFlagellantBehaviour.StopMovement();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		public override void Update()
		{
			base.Update();
			this.NewFlagellant.NewFlagellantBehaviour.UpdateHurtTime();
			if (this.NewFlagellant.NewFlagellantBehaviour._currentHurtTime > this.hurtRecoveryTime)
			{
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantIdleState>();
			}
			this.NewFlagellant.NewFlagellantBehaviour.CheckFall();
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private float hurtRecoveryTime = 1.2f;
	}
}
