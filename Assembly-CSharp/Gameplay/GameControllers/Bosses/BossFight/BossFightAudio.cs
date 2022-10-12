using System;
using System.Diagnostics;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BossFight
{
	public class BossFightAudio : MonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnBossMusicStarts;

		public EventInstance GetCurrentMusicInstance()
		{
			return this._bossMusicInstance;
		}

		private void Start()
		{
			this._bossMusicInstance = this.GetEventInstanceByKey(this.BossTrackId);
		}

		[Button(ButtonSizes.Small)]
		public void PlayBossTrack()
		{
			if (this._bossMusicInstance.isValid())
			{
				if (this.OnBossMusicStarts != null)
				{
					this.OnBossMusicStarts();
				}
				this._bossMusicInstance.start();
			}
		}

		public void StopBossTrack()
		{
			if (this._bossMusicInstance.isValid())
			{
				this._bossMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this._bossMusicInstance.release();
			}
		}

		public void SetBossTrackState(float paramValue)
		{
			if (!this._bossMusicInstance.isValid())
			{
				return;
			}
			ParameterInstance parameterInstance;
			this._bossMusicInstance.getParameter("Intensity", out parameterInstance);
			float value = Mathf.Clamp(paramValue, 0f, 100f);
			if (parameterInstance.isValid())
			{
				parameterInstance.setValue(value);
			}
		}

		public void SetBossTrackParam(string paramName, float paramValue)
		{
			if (!this._bossMusicInstance.isValid())
			{
				return;
			}
			ParameterInstance parameterInstance;
			this._bossMusicInstance.getParameter(paramName, out parameterInstance);
			if (parameterInstance.isValid())
			{
				parameterInstance.setValue(paramValue);
			}
		}

		public ParameterInstance GetBossTrackParam(string paramName)
		{
			ParameterInstance result = default(ParameterInstance);
			if (this._bossMusicInstance.isValid())
			{
				this._bossMusicInstance.getParameter(paramName, out result);
			}
			return result;
		}

		public void SetBossEndingMusic(float paramValue)
		{
			if (!this._bossMusicInstance.isValid())
			{
				return;
			}
			ParameterInstance parameterInstance;
			this._bossMusicInstance.getParameter("Ending", out parameterInstance);
			if (parameterInstance.isValid())
			{
				parameterInstance.setValue(Mathf.Clamp01(paramValue));
			}
		}

		private EventInstance GetEventInstanceByKey(string eventInstanceId)
		{
			EventInstance result = default(EventInstance);
			if (!string.IsNullOrEmpty(eventInstanceId))
			{
				result = Core.Audio.CreateEvent(eventInstanceId, default(Vector3));
			}
			return result;
		}

		private void OnDestroy()
		{
			this.StopBossTrack();
		}

		[EventRef]
		public string BossTrackId;

		private EventInstance _bossMusicInstance;
	}
}
