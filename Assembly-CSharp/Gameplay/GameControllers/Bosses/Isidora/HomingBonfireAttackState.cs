using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class HomingBonfireAttackState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Behaviour = machine.GetComponent<HomingBonfireBehaviour>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.currentCooldown = this.Behaviour.AttackCooldown;
		}

		public override void Update()
		{
			base.Update();
			this.currentCooldown -= Time.deltaTime;
			if (this.currentCooldown < 0f)
			{
				this.currentCooldown = this.Behaviour.AttackCooldown;
				this.Behaviour.FireProjectile();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private HomingBonfireBehaviour Behaviour;

		private float currentCooldown;
	}
}
