using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.CommonAttacks
{
	public class BossEnemySpawn : EnemyAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (this.spawnFX != null)
			{
				PoolManager.Instance.CreatePool(this.spawnFX, 1);
			}
			this.spawnedEnemies = new List<GameObject>();
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
		}

		public void DestroyAllSpawned()
		{
			int count = this.spawnedEnemies.Count;
			for (int i = 0; i < count; i++)
			{
				this.spawnedEnemies[i].GetComponent<Enemy>().Kill();
			}
		}

		public Enemy Spawn(Vector2 origin, Vector2 dir, float spawnDelay = 0f, Action callback = null)
		{
			if (this.spawnedEnemies.Count >= this.maxEnemies)
			{
				return null;
			}
			if (this.spawnFX != null)
			{
				PoolManager.Instance.ReuseObject(this.spawnFX, origin, Quaternion.identity, false, 1);
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this.enemyToSpawn, origin, Quaternion.identity);
			this.spawnedEnemies.Add(gameObject);
			Enemy component = gameObject.GetComponent<Enemy>();
			component.SetOrientation((dir.x <= 0f) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
			component.OnEntityDeath += this.OnEnemyDeath;
			gameObject.SetActive(false);
			base.StartCoroutine(this.DelayedActivation(gameObject, spawnDelay, callback));
			return component;
		}

		private IEnumerator DelayedActivation(GameObject go, float seconds, Action callback)
		{
			yield return new WaitForSeconds(seconds);
			go.SetActive(true);
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		private void OnEnemyDeath(Entity e)
		{
			e.OnEntityDeath -= this.OnEnemyDeath;
			this.spawnedEnemies.Remove(e.gameObject);
		}

		private void DrawDebugCross(Vector2 point, Color c, float seconds)
		{
			float num = 0.6f;
			Debug.DrawLine(point - Vector2.up * num, point + Vector2.up * num, c, seconds);
			Debug.DrawLine(point - Vector2.right * num, point + Vector2.right * num, c, seconds);
		}

		private Coroutine _currentCoroutine;

		[FoldoutGroup("Spawn settings", 0)]
		public GameObject enemyToSpawn;

		public int maxEnemies = 3;

		public List<GameObject> spawnedEnemies;

		public GameObject spawnFX;
	}
}
