using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Gizmos
{
	public class PenitentSpawnPoint : SpawnPoint
	{
		public GameObject Instance()
		{
			GameObject result = null;
			if (this.PenitentPrefab != null)
			{
				result = UnityEngine.Object.Instantiate<GameObject>(this.PenitentPrefab, base.transform.position, Quaternion.identity);
			}
			else
			{
				Debug.LogError("The prefab variable cannot be null");
			}
			return result;
		}

		public GameObject PenitentPrefab;
	}
}
