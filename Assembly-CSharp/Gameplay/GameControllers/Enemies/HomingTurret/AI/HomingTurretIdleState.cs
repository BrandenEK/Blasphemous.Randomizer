using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.HomingTurret.AI
{
	public class HomingTurretIdleState : State
	{
		private HomingTurretBehaviour Behaviour { get; set; }

		private HomingTurret HomingTurret { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.HomingTurret = machine.GetComponent<HomingTurret>();
			this.Behaviour = (HomingTurretBehaviour)this.HomingTurret.EnemyBehaviour;
		}
	}
}
