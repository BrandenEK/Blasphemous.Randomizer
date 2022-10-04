using System;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Entities.MiriamPortal.AI;
using Gameplay.GameControllers.Entities.MiriamPortal.Animation;
using Gameplay.GameControllers.Entities.MiriamPortal.Audio;

namespace Gameplay.GameControllers.Entities.MiriamPortal
{
	public class MiriamPortalPrayer : Entity
	{
		public MiriamPortalPrayerBehaviour Behaviour { get; private set; }

		public MiriamPortalPrayerAnimationHandler AnimationHandler { get; private set; }

		public MiriamPortalPrayerAudio Audio { get; private set; }

		public MiriamPortalPrayerAttack Attack { get; private set; }

		public GhostTrailGenerator GhostTrail { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Audio = base.GetComponentInChildren<MiriamPortalPrayerAudio>();
			this.Attack = base.GetComponentInChildren<MiriamPortalPrayerAttack>();
			this.Behaviour = base.GetComponentInChildren<MiriamPortalPrayerBehaviour>();
			this.AnimationHandler = base.GetComponentInChildren<MiriamPortalPrayerAnimationHandler>();
			this.AnimationHandler.MiriamPortalPrayer = this;
			this.GhostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
		}

		private bool turning;
	}
}
