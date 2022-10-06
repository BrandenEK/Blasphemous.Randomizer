using System;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Runner.Audio
{
	public class RunnerAudio : EntityAudio
	{
		public void PlayDeath()
		{
			this.StopScream();
			base.PlayOneShotEvent("RunnerDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayRun()
		{
			base.PlayOneShotEvent("RunnerRun", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySlide()
		{
			base.PlayOneShotEvent("RunnerSlide", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayScream()
		{
			this.StopScream();
			Core.Audio.PlayEventNoCatalog(ref this._screamEventInstance, this.ScreamEventKey, default(Vector3));
		}

		public void StopScream()
		{
			base.StopEvent(ref this._screamEventInstance);
		}

		public void SetScreamParam(float value)
		{
			this.SetMoveParam(this._screamEventInstance, value);
		}

		private void SetMoveParam(EventInstance eventInstance, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Moves", ref parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private const string DeathEventKey = "RunnerDeath";

		private const string RunEventKey = "RunnerRun";

		private const string SlideEventKey = "RunnerSlide";

		[EventRef]
		[SerializeField]
		private string ScreamEventKey;

		private const string MoveParameterKey = "Moves";

		private EventInstance _screamEventInstance;
	}
}
