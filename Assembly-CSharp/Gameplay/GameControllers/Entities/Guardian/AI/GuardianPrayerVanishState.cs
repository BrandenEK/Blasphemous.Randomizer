using System;
using Gameplay.GameControllers.Entities.Guardian.Animation;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Entities.Guardian.AI
{
	public class GuardianPrayerVanishState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._behaviour = this.Machine.GetComponent<GuardianPrayerBehaviour>();
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
			GuardianPrayer guardian = this._behaviour.Guardian;
			guardian.AnimationHandler.SetAnimatorTrigger(GuardianPrayerAnimationHandler.VanishTrigger);
			this._behaviour.Guardian.Audio.PlayVanish();
		}

		private GuardianPrayerBehaviour _behaviour;
	}
}
