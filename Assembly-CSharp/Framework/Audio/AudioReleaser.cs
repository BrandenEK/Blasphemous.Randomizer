using System;
using FMODUnity;
using UnityEngine;

namespace Framework.Audio
{
	public class AudioReleaser : MonoBehaviour
	{
		private void Start()
		{
			this._eventEmitters = Object.FindObjectsOfType<StudioEventEmitter>();
		}

		private void OnDestroy()
		{
			if (this._eventEmitters == null)
			{
				return;
			}
			for (int i = 0; i < this._eventEmitters.Length; i++)
			{
				this._eventEmitters[i].Stop();
			}
		}

		private StudioEventEmitter[] _eventEmitters;
	}
}
