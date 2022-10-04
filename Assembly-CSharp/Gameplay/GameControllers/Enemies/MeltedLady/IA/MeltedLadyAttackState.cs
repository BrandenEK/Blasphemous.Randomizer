using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.IA
{
	public class MeltedLadyAttackState : State
	{
		private protected FloatingLady MeltedLady { protected get; private set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.MeltedLady = machine.GetComponent<FloatingLady>();
			this.MeltedLady.Behaviour.Teleport();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
		}

		public override void Update()
		{
			base.Update();
			if (!this.MeltedLady.Behaviour.CanTeleport)
			{
				return;
			}
			this.MeltedLady.Behaviour.TeleportCooldownLapse += Time.deltaTime;
			if (this.MeltedLady.Behaviour.TeleportCooldownLapse < this.MeltedLady.Behaviour.TeleportCooldown)
			{
				return;
			}
			this.MeltedLady.Behaviour.Teleport();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.MeltedLady.Behaviour.TeleportCooldownLapse = 0f;
		}
	}
}
