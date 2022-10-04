using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.AI
{
	public class NewFlagellantIdleState : State
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
			this.NewFlagellant.NewFlagellantBehaviour.StopMovement();
			this._counter = UnityEngine.Random.Range(1f, 2f);
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		public override void Update()
		{
			base.Update();
			this.NewFlagellant.AnimatorInyector.StopAttack();
			if (this.NewFlagellant.NewFlagellantBehaviour.CanSeeTarget() && this.NewFlagellant.NewFlagellantBehaviour.CanReachPlayer())
			{
				this.NewFlagellant.NewFlagellantBehaviour.ResetRememberTime();
				this.NewFlagellant.NewFlagellantBehaviour.LookAtTarget(this.NewFlagellant.Target.transform.position);
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantChaseState>();
			}
			this._counter -= Time.deltaTime;
			if (this._counter <= 0f)
			{
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantPatrolState>();
			}
			this.NewFlagellant.NewFlagellantBehaviour.CheckFall();
		}

		private const float minIdleTime = 1f;

		private const float maxIdleTime = 2f;

		private float _counter;
	}
}
