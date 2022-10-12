using System;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.Audio
{
	public class InkLadyAudio : MeltedLadyAudio
	{
		public void PlayBeamCharge(ref EventInstance beamFireInstance)
		{
			base.StopEvent(ref beamFireInstance);
			Core.Audio.PlayEventWithCatalog(ref beamFireInstance, "InkLadyBeamFire", default(Vector3));
		}

		public void PlayBeamFire(ref EventInstance beamFireInstance)
		{
			this.SetParam(beamFireInstance, "Attack", 1f);
		}

		public void StopBeamFire(ref EventInstance beamFireInstance)
		{
			this.SetParam(beamFireInstance, "End", 1f);
		}

		private void SetParam(EventInstance eventInstance, string paramKey, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter(paramKey, out parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		public new void Hurt()
		{
			base.PlayOneShotEvent("InkLadyHit", EntityAudio.FxSoundCategory.Damage);
		}

		public new void Appearing()
		{
			base.PlayOneShotEvent("InkLadyAppearing", EntityAudio.FxSoundCategory.Motion);
		}

		public new void Disappearing()
		{
			base.PlayOneShotEvent("InkLadyDisappearing", EntityAudio.FxSoundCategory.Motion);
		}

		public new void PlayDeath()
		{
			base.PlayOneShotEvent("InkLadyDeath", EntityAudio.FxSoundCategory.Damage);
		}

		private const string BeamFireEventKey = "InkLadyBeamFire";

		private const string AttackParamKey = "Attack";

		private const string EndParamKey = "End";

		private const string AttackEventKey = "MeltedLadyAttack";

		private const string AppearingEventKey = "InkLadyAppearing";

		private const string DisappearingEventKey = "InkLadyDisappearing";

		private const string DeathEventKey = "InkLadyDeath";

		private const string MeltedLadyDamage = "InkLadyHit";
	}
}
