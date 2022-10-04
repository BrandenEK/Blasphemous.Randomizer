using System;
using Gameplay.GameControllers.Entities.Guardian.AI;
using Gameplay.GameControllers.Entities.Guardian.Animation;
using Gameplay.GameControllers.Entities.Guardian.Audio;

namespace Gameplay.GameControllers.Entities.Guardian
{
	public class GuardianPrayer : Entity
	{
		public GuardianPrayerBehaviour Behaviour { get; private set; }

		public GuardianPrayerAnimationHandler AnimationHandler { get; private set; }

		public GuardianPrayerAudio Audio { get; private set; }

		public GuardianPrayerAttack Attack { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Audio = base.GetComponentInChildren<GuardianPrayerAudio>();
			this.Attack = base.GetComponentInChildren<GuardianPrayerAttack>();
			this.Behaviour = base.GetComponentInChildren<GuardianPrayerBehaviour>();
			this.AnimationHandler = base.GetComponentInChildren<GuardianPrayerAnimationHandler>();
			this.AnimationHandler.GuardianPrayer = this;
		}

		private bool turning;
	}
}
