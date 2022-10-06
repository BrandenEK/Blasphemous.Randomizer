using System;
using System.Collections.Generic;
using FMOD.Studio;
using Gameplay.GameControllers.Enemies.Stoners.Rock;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Stoners.Audio
{
	public class StonersRockAudio : EntityAudio
	{
		protected override void OnWake()
		{
			base.OnWake();
			this.EventInstances = new List<EventInstance>();
		}

		public void BrokenRock()
		{
			if (this._brokenRockEventInstance.isValid())
			{
				return;
			}
			this._brokenRockEventInstance = base.AudioManager.CreateCatalogEvent("StonersBrokenRock", default(Vector3));
			this._brokenRockEventInstance.setCallback(EntityAudio.SetPanning(this._brokenRockEventInstance, base.transform.position), 1);
			this._brokenRockEventInstance.start();
			this._brokenRockEventInstance.release();
		}

		public const string StonersBrokenRockEventKey = "StonersBrokenRock";

		private EventInstance _brokenRockEventInstance;

		private StonersRock _stonersRock;
	}
}
