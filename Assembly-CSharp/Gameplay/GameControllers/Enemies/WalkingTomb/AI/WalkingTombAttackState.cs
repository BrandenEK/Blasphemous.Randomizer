using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.WalkingTomb.AI
{
	public class WalkingTombAttackState : State
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
			this.WalkingTomb.Behaviour.StopMovement();
			this.WalkingTomb.Behaviour.Attack();
		}

		public override void Update()
		{
			base.Update();
			bool attack = this.WalkingTomb.Behaviour.TargetIsInFront && this.WalkingTomb.Behaviour.TargetIsOnAttackDistance;
			this.WalkingTomb.AnimatorInjector.Attack(attack);
			if (!this.WalkingTomb.IsAttacking)
			{
				this.WalkingTomb.StateMachine.SwitchState<WalkingTombWalkState>();
			}
			else
			{
				this.WalkingTomb.Behaviour.StopMovement();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}
	}
}
