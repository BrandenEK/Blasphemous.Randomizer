using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Guardian.Animation;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Entities.Guardian.AI
{
	public class GuardianPrayerIdleState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._behaviour = this.Machine.GetComponent<GuardianPrayerBehaviour>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Awaiting();
		}

		public override void Update()
		{
			base.Update();
			if (!Core.Logic.Penitent.Status.IsGrounded)
			{
				return;
			}
			GuardianPrayer guardian = this._behaviour.Guardian;
			guardian.AnimationHandler.Appearing();
			guardian.Behaviour.SetInitialOrientation();
			guardian.transform.position = this._behaviour.Master.transform.position;
			this._behaviour.SetState(GuardianPrayerBehaviour.GuardianState.Follow);
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private void Awaiting()
		{
			this._behaviour.IdleFlag = false;
			GuardianPrayer guardian = this._behaviour.Guardian;
			guardian.AnimationHandler.SetAnimatorTrigger(GuardianPrayerAnimationHandler.AwaitingTrigger);
		}

		private GuardianPrayerBehaviour _behaviour;
	}
}
