using System;
using System.Collections.Generic;
using System.Linq;
using Framework.EditorScripts.EnemiesBalance;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.ExplodingEnemy;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ReekLeader.Attack
{
	public class ReekSpawner : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.SetSpawnPoints();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		public GameObject InstanceReek(Vector3 position)
		{
			if (!this.Reek)
			{
				return null;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this.Reek, position, Quaternion.identity);
			ExplodingEnemy componentInChildren = gameObject.GetComponentInChildren<ExplodingEnemy>();
			ReekSpawner.SetStatsByGameMode(componentInChildren);
			componentInChildren.IsSummoned = true;
			componentInChildren.ReekLeader = (ReekLeader)base.EntityOwner;
			this._activedReeks.Add(gameObject);
			return gameObject;
		}

		public void DisposeReek(GameObject reekGo)
		{
			ExplodingEnemy componentInChildren = reekGo.GetComponentInChildren<ExplodingEnemy>();
			if (!componentInChildren)
			{
				return;
			}
			this._activedReeks.Remove(reekGo);
			componentInChildren.Destroy();
		}

		private static void SetStatsByGameMode(Enemy enemy)
		{
			EnemyStatsImporter enemyStatsImporter = Core.Logic.CurrentLevelConfig.EnemyStatsImporter;
			if (enemyStatsImporter != null)
			{
				enemyStatsImporter.SetEnemyStats(enemy);
			}
		}

		public int SummonedReekAmount
		{
			get
			{
				return this._activedReeks.Count((GameObject x) => x.activeSelf);
			}
		}

		private void SetSpawnPoints()
		{
			ReekSpawnPoint[] array = Object.FindObjectsOfType<ReekSpawnPoint>();
			foreach (ReekSpawnPoint item in array)
			{
				this.ReekSpawnPoints.Add(item);
			}
		}

		public ReekSpawnPoint GetNearestReekSpawnPoint()
		{
			Vector3 position = base.transform.position;
			return this.GetNearestReekSpawnPoint(position);
		}

		public ReekSpawnPoint GetPlayerClosestReekSpawnPoint()
		{
			Vector3 position = Core.Logic.Penitent.transform.position;
			return this.GetNearestReekSpawnPoint(position);
		}

		private ReekSpawnPoint GetNearestReekSpawnPoint(Vector3 position)
		{
			ReekSpawnPoint result = null;
			float num = float.PositiveInfinity;
			foreach (ReekSpawnPoint reekSpawnPoint in this.ReekSpawnPoints)
			{
				float num2 = Vector3.Distance(reekSpawnPoint.transform.position, position);
				if (num2 < num && reekSpawnPoint.SpawnedEntityId == 0)
				{
					result = reekSpawnPoint;
					num = num2;
				}
			}
			return result;
		}

		public void ResetSpawnPoint(int id)
		{
			ReekSpawnPoint reekSpawnPoint = this.ReekSpawnPoints.First((ReekSpawnPoint x) => x.SpawnedEntityId == id);
			if (reekSpawnPoint != null)
			{
				reekSpawnPoint.SpawnedEntityId = 0;
			}
		}

		public GameObject Reek;

		public List<ReekSpawnPoint> ReekSpawnPoints = new List<ReekSpawnPoint>();

		private List<GameObject> _activedReeks = new List<GameObject>();
	}
}
