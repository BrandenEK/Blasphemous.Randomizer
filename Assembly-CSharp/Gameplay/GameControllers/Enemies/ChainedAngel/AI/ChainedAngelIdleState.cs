using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.ChainedAngel.AI
{
	public class ChainedAngelIdleState : State
	{
		private ChainedAngel ChainedAngel { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.ChainedAngel = machine.GetComponent<ChainedAngel>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.ChainedAngel.FloatingMotion.IsFloating = true;
		}

		public override void Update()
		{
			base.Update();
			this.ChainedAngel.Behaviour.LookAtTarget(this.ChainedAngel.Target.transform.position);
			if (this.ChainedAngel.Behaviour.CanSeeTarget)
			{
				this.ChainedAngel.StateMachine.SwitchState<ChainedAngelAttackState>();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}
	}
}
