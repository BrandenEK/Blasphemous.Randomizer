using System;
using Gameplay.GameControllers.Entities.MiriamPortal.Animation;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Entities.MiriamPortal.AI
{
	public class MiriamPortalPrayerVanishState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._behaviour = this.Machine.GetComponent<MiriamPortalPrayerBehaviour>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Vanish();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private void Vanish()
		{
			this._behaviour.VanishFlag = false;
			this._behaviour.IdleFlag = false;
			MiriamPortalPrayer miriamPortal = this._behaviour.MiriamPortal;
			miriamPortal.AnimationHandler.SetAnimatorTrigger(MiriamPortalPrayerAnimationHandler.VanishTrigger);
			this._behaviour.MiriamPortal.Audio.PlayVanish();
		}

		private MiriamPortalPrayerBehaviour _behaviour;
	}
}
