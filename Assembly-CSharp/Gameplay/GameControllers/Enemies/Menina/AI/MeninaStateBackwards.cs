using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Menina.AI
{
	public class MeninaStateBackwards : State
	{
		protected Menina Menina { get; set; }

		protected MeninaBehaviour Behaviour { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			if (this.Menina == null)
			{
				this.Menina = machine.GetComponent<Menina>();
			}
			if (this.Behaviour == null)
			{
				this.Behaviour = this.Menina.GetComponent<MeninaBehaviour>();
			}
		}

		public override void Update()
		{
			base.Update();
			if (!this.Behaviour.IsAwake)
			{
				return;
			}
			this.CheckIfPlayerIsClose();
			this._currentAwaitBeforeBackwards += Time.deltaTime;
			float num = Vector2.Distance(this.Menina.StartPosition, this.Menina.transform.position);
			if (this._currentAwaitBeforeBackwards >= this.Behaviour.AwaitBeforeBackward && num > 1f)
			{
				this.Behaviour.StepBackwards();
			}
			else
			{
				this.Behaviour.StopMovement();
			}
		}

		private void CheckIfPlayerIsClose()
		{
			if (this.Behaviour.PlayerSeen)
			{
				this.Menina.StateMachine.SwitchState<MeninaStateAttack>();
			}
			else if (this.Behaviour.PlayerHeard)
			{
				this.Menina.StateMachine.SwitchState<MeninaStateChase>();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this._currentAwaitBeforeBackwards = 0f;
		}

		private float _currentAwaitBeforeBackwards;
	}
}
