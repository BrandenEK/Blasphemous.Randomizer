using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class HomingBonfireAudio : EntityAudio
	{
		public void PlayBrazierBroken()
		{
			this.PlaySimpleOneShot(this.Brazier_Broken);
		}

		public void PlayBrazierIgnitionPhase2()
		{
			this.PlaySimpleOneShot(this.Brazier_Ignition2);
		}

		public void PlaySimpleOneShot(string key)
		{
			if (!string.IsNullOrEmpty(key))
			{
				Core.Audio.PlayOneShot(key, default(Vector3));
			}
		}

		[SerializeField]
		[EventRef]
		private string Brazier_Ignition2;

		[SerializeField]
		[EventRef]
		private string Brazier_Broken;
	}
}
