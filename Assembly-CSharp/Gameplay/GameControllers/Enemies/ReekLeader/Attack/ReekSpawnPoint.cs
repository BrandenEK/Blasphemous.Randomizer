using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ReekLeader.Attack
{
	public class ReekSpawnPoint : SpawnPoint
	{
		public int SpawnedEntityId { get; set; }

		private void OnDrawGizmos()
		{
			Gizmos.DrawIcon(base.transform.position, "Blasphemous/ReekSpawnReference.png", true);
		}
	}
}
