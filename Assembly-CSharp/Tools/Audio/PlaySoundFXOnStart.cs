using System;
using FMODUnity;
using Framework.Managers;
using UnityEngine;

namespace Tools.Audio
{
	public class PlaySoundFXOnStart : MonoBehaviour
	{
		private void Start()
		{
			Core.Audio.PlaySfx(this.eventId, 0f);
		}

		[EventRef]
		public string eventId;
	}
}
