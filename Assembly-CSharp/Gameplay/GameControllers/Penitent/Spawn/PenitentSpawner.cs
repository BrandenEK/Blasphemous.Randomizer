using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Spawn
{
	public class PenitentSpawner
	{
		public GameObject SpawnPenitent(PenitentSpawnPoint penitentSpawnPoint)
		{
			GameObject gameObject = null;
			if (penitentSpawnPoint != null)
			{
				gameObject = penitentSpawnPoint.Instance();
			}
			if (gameObject != null && this.OnSpawned != null)
			{
				this.OnSpawned();
			}
			return gameObject;
		}

		public Core.SimpleEvent OnSpawned;
	}
}
