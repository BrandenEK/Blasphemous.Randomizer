using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;
using Tools.Level.Layout;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.PlayMaker.Action
{
	[ActionCategory(ActionCategory.Array)]
	[HutongGames.PlayMaker.Tooltip("Spawns a group of enemies set in their spawn points")]
	public class SpawnEnemies : FsmStateAction
	{
		public override void Reset()
		{
			this.EnemySpawners = null;
		}

		public override void OnEnter()
		{
			this.CastEnemySpawners();
			base.Finish();
		}

		private void CastEnemySpawners()
		{
			List<Enemy> list = SpawnEnemies.FindObjectsOfTypeAll<Enemy>();
			foreach (object obj in this.EnemySpawners.Values)
			{
				GameObject gameObject = obj as GameObject;
				if (gameObject)
				{
					EnemySpawnPoint component = gameObject.GetComponent<EnemySpawnPoint>();
					if (component && !component.EnemySpawnDisabled)
					{
						bool flag = false;
						foreach (Enemy enemy in list)
						{
							if (enemy.SpawningId == component.gameObject.GetHashCode())
							{
								enemy.gameObject.SetActive(true);
								enemy.Stats.Life.Current = enemy.Stats.Life.Base;
								enemy.Status.Dead = false;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							component.CreateEnemy();
						}
						component.Consumed = false;
					}
				}
			}
		}

		public static List<T> FindObjectsOfTypeAll<T>()
		{
			List<T> list = new List<T>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.isLoaded)
				{
					foreach (GameObject gameObject in sceneAt.GetRootGameObjects())
					{
						list.AddRange(gameObject.GetComponentsInChildren<T>(true));
					}
				}
			}
			return list;
		}

		[HutongGames.PlayMaker.Tooltip("The set of enemy spawners.")]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray EnemySpawners;
	}
}
