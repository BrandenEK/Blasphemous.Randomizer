using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Guardian.Animation;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Entities.MiriamPortal.AI
{
	public class MiriamPortalPrayerIdleState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._behaviour = this.Machine.GetComponent<MiriamPortalPrayerBehaviour>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this._behaviour.ReappearFlag = false;
			this.Awaiting();
		}

		public override void Update()
		{
			base.Update();
			if (!Core.Logic.Penitent.Status.IsGrounded)
			{
				return;
			}
			this._behaviour.MiriamPortal.AnimationHandler.Appearing();
			this._behaviour.SetInitialOrientation();
			this._behaviour.SetState(MiriamPortalPrayerBehaviour.MiriamPortalState.Follow);
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private void Awaiting()
		{
			this._behaviour.IdleFlag = false;
			MiriamPortalPrayer miriamPortal = this._behaviour.MiriamPortal;
			miriamPortal.AnimationHandler.SetAnimatorTrigger(GuardianPrayerAnimationHandler.AwaitingTrigger);
		}

		private MiriamPortalPrayerBehaviour _behaviour;
	}
}
