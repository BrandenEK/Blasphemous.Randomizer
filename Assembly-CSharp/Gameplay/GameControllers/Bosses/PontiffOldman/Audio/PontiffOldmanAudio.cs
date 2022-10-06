using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffOldman.Audio
{
	public class PontiffOldmanAudio : EntityAudio
	{
		public void PlayDeath_AUDIO()
		{
			this.StopAll();
			base.PlayOneShotEvent("PontiffOldmanDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void StopAll()
		{
			base.StopEvent(ref this._castEventInstance);
			this.StopWind_AUDIO();
		}

		public void PlayTeleportIn_AUDIO()
		{
			base.PlayOneShotEvent("PontiffOldmanTeleportIn", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayTeleportOut_AUDIO()
		{
			base.PlayOneShotEvent("PontiffOldmanTeleportOut", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayStartCast_AUDIO()
		{
			base.PlayOneShotEvent("PontiffOldmanCast", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayStartCastLoop_AUDIO()
		{
			base.StopEvent(ref this._castEventInstance);
			base.PlayEvent(ref this._castEventInstance, "PontiffOldmanCastLoop", true);
		}

		public void PlayStopCastLoop_AUDIO()
		{
			this.SetParam(this._castEventInstance, "End", 1f);
		}

		public void PlayWind_AUDIO()
		{
			base.StopEvent(ref this._windEventInstance);
			base.PlayEvent(ref this._windEventInstance, "PontiffOldmanWindLoop", true);
		}

		public void StopWind_AUDIO()
		{
			this.SetParam(this._windEventInstance, "End", 1f);
		}

		public void SetParam(EventInstance eventInstance, string paramKey, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter(paramKey, ref parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private void OnDestroy()
		{
			base.StopEvent(ref this._windEventInstance);
			base.StopEvent(ref this._castEventInstance);
		}

		private const string PontiffOldman_DEATH = "PontiffOldmanDeath";

		private const string PontiffOldman_TELEPORT_IN = "PontiffOldmanTeleportIn";

		private const string PontiffOldman_TELEPORT_OUT = "PontiffOldmanTeleportOut";

		private const string PontiffOldman_CAST = "PontiffOldmanCast";

		private const string PontiffOldman_CAST_LOOP = "PontiffOldmanCastLoop";

		private const string PontiffOldman_WIND = "PontiffOldmanWindLoop";

		private const string EndParamKey = "End";

		private EventInstance _castEventInstance;

		private EventInstance _windEventInstance;
	}
}
