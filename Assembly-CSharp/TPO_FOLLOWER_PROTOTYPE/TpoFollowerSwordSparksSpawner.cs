using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Sparks;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace TPO_FOLLOWER_PROTOTYPE
{
	public class TpoFollowerSwordSparksSpawner : MonoBehaviour
	{
		private void Awake()
		{
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			SwordSparkSpawner componentInChildren = penitent.GetComponentInChildren<SwordSparkSpawner>();
			this.SetDemakeSparks(componentInChildren);
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		private void SetDemakeSparks(SwordSparkSpawner sparkSpawner)
		{
			if (!sparkSpawner)
			{
				return;
			}
			sparkSpawner.SwordSparks = this.swordSparks;
		}

		[SerializeField]
		private SwordSpark[] swordSparks;
	}
}
