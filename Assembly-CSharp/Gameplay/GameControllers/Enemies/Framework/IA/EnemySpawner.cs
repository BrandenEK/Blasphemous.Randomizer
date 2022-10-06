using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Framework.FrameworkCore;
using Framework.Managers;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class EnemySpawner
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnEnemiesRespawn;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnConsumedSpanwPoint;

		public void SpawnEnemiesOnLoad()
		{
			Log.Trace("Spawn", "Spawning enemies on level.", null);
			EnemySpawnPoint[] array = Object.FindObjectsOfType<EnemySpawnPoint>();
			foreach (EnemySpawnPoint enemySpawnPoint in array)
			{
				enemySpawnPoint.CreateEnemy();
			}
		}

		public void RespawnDeadEnemies()
		{
			Log.Trace("Spawn", "Respawning dead enemies on level.", null);
			EnemySpawnPoint[] array = Object.FindObjectsOfType<EnemySpawnPoint>();
			this.consumedSpawns.Clear();
			foreach (EnemySpawnPoint enemySpawnPoint in array)
			{
				enemySpawnPoint.CreateEnemy();
			}
			if (this.OnEnemiesRespawn != null)
			{
				this.OnEnemiesRespawn();
			}
		}

		public void Reset()
		{
			this.consumedSpawns.Clear();
		}

		public void AddConsumedSpawner(EnemySpawnPoint spawnPoint)
		{
			if (spawnPoint == null)
			{
				return;
			}
			if (!this.consumedSpawns.Contains(spawnPoint.transform.position))
			{
				this.consumedSpawns.Add(spawnPoint.transform.position);
				if (this.OnConsumedSpanwPoint != null)
				{
					this.OnConsumedSpanwPoint();
				}
			}
		}

		public bool IsSpawnerConsumed(string spawnPointName)
		{
			EnemySpawnPoint[] source = Object.FindObjectsOfType<EnemySpawnPoint>();
			return this.IsSpawnerConsumed(source.First((EnemySpawnPoint p) => p.gameObject.name == spawnPointName));
		}

		public bool IsSpawnerConsumed(EnemySpawnPoint spawnPoint)
		{
			return spawnPoint != null && this.consumedSpawns.Contains(spawnPoint.transform.position);
		}

		public bool AreAllSpawnersConsumed()
		{
			EnemySpawnPoint[] source = Object.FindObjectsOfType<EnemySpawnPoint>();
			return source.All((EnemySpawnPoint p) => this.IsSpawnerConsumed(p));
		}

		public bool IsAnySpawnerLeft()
		{
			EnemySpawnPoint[] source = Object.FindObjectsOfType<EnemySpawnPoint>();
			return source.Any((EnemySpawnPoint p) => !this.IsSpawnerConsumed(p));
		}

		private EnemySpawnPoint[] _enemySpawnPoints;

		private readonly List<Vector3> consumedSpawns = new List<Vector3>();
	}
}
