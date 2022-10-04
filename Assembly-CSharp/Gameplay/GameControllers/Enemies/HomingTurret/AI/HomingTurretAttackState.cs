using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.HomingTurret.AI
{
	public class HomingTurretAttackState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Behaviour = machine.GetComponent<HomingTurretBehaviour>();
			this.Behaviour.Spawn();
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
			if (this.currentCooldown > 0f)
			{
				return;
			}
			if (this.currentReadyAttackTime == 0f)
			{
				this.Behaviour.ChargeAttack();
			}
			this.currentReadyAttackTime += Time.deltaTime;
			if (this.currentReadyAttackTime >= this.Behaviour.ReadyAttackTime)
			{
				this.currentReadyAttackTime = 0f;
				this.currentCooldown = this.Behaviour.AttackCooldown;
				this.Behaviour.ReleaseAttack();
			}
		}

		private HomingTurretBehaviour Behaviour;

		private float currentReadyAttackTime;

		private float currentCooldown;
	}
}
