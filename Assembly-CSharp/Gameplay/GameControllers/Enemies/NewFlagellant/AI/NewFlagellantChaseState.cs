using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.AI
{
	public class NewFlagellantChaseState : State
	{
		public NewFlagellant NewFlagellant { get; private set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.NewFlagellant = machine.GetComponent<NewFlagellant>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.NewFlagellant.AnimatorInyector.Run(true);
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.NewFlagellant.AnimatorInyector.Run(false);
		}

		public override void Update()
		{
			base.Update();
			if (!this.NewFlagellant.NewFlagellantBehaviour.CanAttack())
			{
				this.NewFlagellant.AnimatorInyector.Run(false);
				return;
			}
			if (this.NewFlagellant.DistanceToTarget > 10f || !this.NewFlagellant.NewFlagellantBehaviour.StillRemembersPlayer())
			{
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantPatrolState>();
				return;
			}
			this.NewFlagellant.NewFlagellantBehaviour.LookAtTarget(this.NewFlagellant.Target.transform.position);
			this.NewFlagellant.NewFlagellantBehaviour.Chase();
			if (this.NewFlagellant.NewFlagellantBehaviour.IsTargetInsideAttackRange())
			{
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantAttackState>();
			}
			this.NewFlagellant.NewFlagellantBehaviour.CheckFall();
		}
	}
}
