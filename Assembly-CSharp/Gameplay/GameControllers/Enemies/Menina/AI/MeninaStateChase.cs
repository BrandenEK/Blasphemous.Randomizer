using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Menina.AI
{
	public class MeninaStateChase : State
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

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Behaviour.CurrentChasingTime = 0f;
		}

		public override void Update()
		{
			base.Update();
			this.Behaviour.IsAwake = true;
			this.Behaviour.CurrentChasingTime += Time.deltaTime;
			this.CheckStateTransition();
			if (!this.Menina.MotionChecker.HitsFloor)
			{
				this.Behaviour.StopMovement();
				return;
			}
			if (this.Behaviour.CurrentChasingTime < this.ChasingTime)
			{
				return;
			}
			if (this.Menina.DistanceToTarget > this.MinDistanceChasing)
			{
				this.Behaviour.Chase(this.Menina.Target.transform);
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.Behaviour.StopMovement();
		}

		private void CheckStateTransition()
		{
			if (this.Behaviour.PlayerSeen)
			{
				this.Menina.StateMachine.SwitchState<MeninaStateAttack>();
			}
			else if (!this.Behaviour.PlayerHeard)
			{
				this.Menina.StateMachine.SwitchState<MeninaStateBackwards>();
			}
		}

		public float ChasingTime;

		public float MinDistanceChasing;
	}
}
