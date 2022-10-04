using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.AshCharger.AI
{
	public class AshChargerAppearState : State
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
			this._counter += Time.deltaTime;
			if (this._counter > this.appearSeconds)
			{
				this.AshCharger.StateMachine.SwitchState<AshChargeWaitingState>();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private float appearSeconds = 0.5f;

		private float _counter;
	}
}
