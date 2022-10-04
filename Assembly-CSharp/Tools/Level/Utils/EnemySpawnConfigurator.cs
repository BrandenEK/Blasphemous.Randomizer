using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Tools.Level.Layout;
using UnityEngine;

namespace Tools.Level.Utils
{
	public class EnemySpawnConfigurator : MonoBehaviour
	{
		private void Awake()
		{
			this.spawnPoint = base.GetComponent<EnemySpawnPoint>();
			this.spawnPoint.OnEnemySpawned += this.OnEnemySpawn;
			this.OnAwake();
		}

		private void OnEnemySpawn(EnemySpawnPoint sp, Enemy e)
		{
			if (this.facingLeft)
			{
				e.SetOrientation(EntityOrientation.Left, true, false);
			}
			e.GetComponent<EnemyBehaviour>().ReadSpawnerConfig(this.configPackage);
			this.OnSpawn(e);
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnSpawn(Enemy e)
		{
		}

		protected void OnDestroy()
		{
			this.spawnPoint.OnEnemySpawned -= this.OnEnemySpawn;
		}

		private EnemySpawnPoint spawnPoint;

		public SpawnBehaviourConfig configPackage;

		public bool facingLeft;
	}
}
