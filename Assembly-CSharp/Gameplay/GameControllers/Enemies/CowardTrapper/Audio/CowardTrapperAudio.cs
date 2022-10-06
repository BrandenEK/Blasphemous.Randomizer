using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CowardTrapper.Audio
{
	public class CowardTrapperAudio : EntityAudio
	{
		public void PlayIdle()
		{
			this.StopIdle();
			base.PlayEvent(ref this._idleEventInstance, "CowardTrapperIdle", true);
		}

		public void StopIdle()
		{
			base.StopEvent(ref this._idleEventInstance);
		}

		public void PlayRun()
		{
			base.PlayOneShotEvent("CowardTrapperRun", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("CowardTrapperDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void SetIdleParam(float value)
		{
			this.SetMoveParam(this._idleEventInstance, value);
		}

		public void SetMoveParam(EventInstance eventInstance, float value)
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

		private const string IdleEventKey = "CowardTrapperIdle";

		private const string RunEventKey = "CowardTrapperRun";

		private const string DeathEventKey = "CowardTrapperDeath";

		private const string MoveParameterKey = "Moves";

		private EventInstance _idleEventInstance;
	}
}
