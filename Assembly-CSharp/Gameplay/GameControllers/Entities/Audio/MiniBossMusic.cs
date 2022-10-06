using System;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Audio
{
	public class MiniBossMusic : MonoBehaviour
	{
		public void PlayEvent()
		{
			this.StopEvent();
			if (this._eventInstance.isValid())
			{
				return;
			}
			this._eventInstance = Core.Audio.CreateEvent(this.SoundEvent, default(Vector3));
			this._eventInstance.start();
		}

		public void StopEvent()
		{
			if (!this._eventInstance.isValid())
			{
				return;
			}
			this._eventInstance.stop(0);
			this._eventInstance.release();
			this._eventInstance = default(EventInstance);
		}

		public void SetParameter(float paramValue)
		{
			if (string.IsNullOrEmpty(this.ParamName) || !this._eventInstance.isValid())
			{
				return;
			}
			paramValue = Mathf.Clamp01(paramValue);
			try
			{
				ParameterInstance parameterInstance;
				this._eventInstance.getParameter(this.ParamName, ref parameterInstance);
				if (parameterInstance.isValid())
				{
					parameterInstance.setValue(paramValue);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + ex.StackTrace);
				throw;
			}
		}

		private void OnDestroy()
		{
			this.StopEvent();
		}

		[EventRef]
		public string SoundEvent;

		private string ParamName = "Ending";

		private EventInstance _eventInstance;
	}
}
