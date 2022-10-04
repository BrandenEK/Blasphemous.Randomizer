using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.WalkingTomb.AI
{
	public class WalkingTombWalkState : State
	{
		protected WalkingTomb WalkingTomb { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.WalkingTomb = machine.GetComponent<WalkingTomb>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
		}

		public override void Update()
		{
			base.Update();
			this.WalkingTomb.Behaviour.Wander();
			if (this.WalkingTomb.Behaviour.TargetIsDead)
			{
				return;
			}
			if (this.WalkingTomb.Behaviour.TargetIsInFront && this.WalkingTomb.Behaviour.TargetIsOnAttackDistance)
			{
				this.WalkingTomb.StateMachine.SwitchState<WalkingTombAttackState>();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}
	}
}
