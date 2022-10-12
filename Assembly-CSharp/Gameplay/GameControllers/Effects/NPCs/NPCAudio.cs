using System;
using FMOD.Studio;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.NPCs
{
	public class NPCAudio : MonoBehaviour
	{
		private void OnDisable()
		{
			if (this.stopOnDisable)
			{
				this.StopEvent();
			}
		}

		private void PlayEventOneShot(string eventKey)
		{
			Core.Audio.PlayOneShot(eventKey, base.transform.position);
		}

		private void PlayEvent(string eventKey)
		{
			if (this.eventInstance.isValid())
			{
				this.StopEvent();
			}
			this.eventInstance = default(EventInstance);
			Core.Audio.PlayEventNoCatalog(ref this.eventInstance, eventKey, base.transform.position);
		}

		private void StopEvent()
		{
			if (!this.eventInstance.isValid())
			{
				return;
			}
			this.eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.eventInstance.release();
			this.eventInstance = default(EventInstance);
		}

		public bool stopOnDisable;

		private EventInstance eventInstance;
	}
}
