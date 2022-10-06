using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.AI
{
	public class NewFlagellantPatrolState : State
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
			this.NewFlagellant.MotionLerper.StopLerping();
			this._counter = Random.Range(4f, 7f);
		}

		public override void OnStateExit()
		{
			this.NewFlagellant.AnimatorInyector.Walk(false);
			base.OnStateExit();
		}

		public override void Update()
		{
			base.Update();
			this.NewFlagellant.NewFlagellantBehaviour.Patrol();
			if (this.NewFlagellant.NewFlagellantBehaviour.CanSeeTarget())
			{
				if (this.NewFlagellant.NewFlagellantBehaviour.CanReachPlayer())
				{
					this.NewFlagellant.NewFlagellantBehaviour.ResetRememberTime();
					this.NewFlagellant.NewFlagellantBehaviour.LookAtTarget(this.NewFlagellant.Target.transform.position);
					this.NewFlagellant.StateMachine.SwitchState<NewFlagellantChaseState>();
				}
				else
				{
					this.NewFlagellant.NewFlagellantBehaviour.ResetRememberTime();
					this.NewFlagellant.NewFlagellantBehaviour.LookAtTarget(this.NewFlagellant.Target.transform.position);
					this.NewFlagellant.StateMachine.SwitchState<NewFlagellantIdleState>();
				}
			}
			this._counter -= Time.deltaTime;
			if (this._counter <= 0f)
			{
				this.NewFlagellant.StateMachine.SwitchState<NewFlagellantIdleState>();
			}
		}

		private const float minPatrolTime = 4f;

		private const float maxPatrolTime = 7f;

		private float _counter;
	}
}
