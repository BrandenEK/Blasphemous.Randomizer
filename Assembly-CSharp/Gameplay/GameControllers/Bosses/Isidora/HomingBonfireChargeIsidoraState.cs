using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class HomingBonfireChargeIsidoraState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Behaviour = machine.GetComponent<HomingBonfireBehaviour>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.timeToMaxRate = this.Behaviour.TimeToMaxRate;
			this.chargeRate = this.Behaviour.ChargeRate;
			this.currentTime = 0f;
			this.currentCooldown = this.chargeRate.Evaluate(0f);
		}

		public override void Update()
		{
			base.Update();
			if (this.currentTime > this.timeToMaxRate)
			{
				return;
			}
			this.currentTime += Time.deltaTime;
			this.currentCooldown -= Time.deltaTime;
			if (this.currentCooldown < 0f)
			{
				this.currentCooldown = this.chargeRate.Evaluate(this.currentTime / this.timeToMaxRate);
				this.Behaviour.FireProjectile();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private HomingBonfireBehaviour Behaviour;

		private float timeToMaxRate;

		private AnimationCurve chargeRate;

		private float currentTime;

		private float currentCooldown;
	}
}
