using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.ViciousDasher.AI
{
	public class ViciousDasherIdleState : State
	{
		public ViciousDasher ViciousDasher { get; private set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.ViciousDasher = machine.GetComponent<ViciousDasher>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.ViciousDasher.MotionLerper.StopLerping();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		public override void Update()
		{
			base.Update();
			this.ViciousDasher.AnimatorInjector.StopAttack();
			if (!this.ViciousDasher.IsTargetVisible || this.ViciousDasher.ViciousDasherBehaviour.CanBeExecuted)
			{
				return;
			}
			this.ViciousDasher.ViciousDasherBehaviour.LookAtTarget(this.ViciousDasher.Target.transform.position);
			this.ViciousDasher.StateMachine.SwitchState<ViciousDasherAttackState>();
		}
	}
}
