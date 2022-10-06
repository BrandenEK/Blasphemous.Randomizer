using System;
using System.Collections;
using System.Linq;
using Framework.Managers;
using HutongGames.PlayMaker;
using Tools.Level.Layout;
using UnityEngine;

namespace Tools.PlayMaker.Action
{
	public class EnableStandByEnemies : FsmStateAction
	{
		public override void OnEnter()
		{
			base.OnEnter();
			base.StartCoroutine(this.SetActiveStandByEnemies());
			base.Finish();
		}

		private IEnumerator SetActiveStandByEnemies()
		{
			object[] spawnPoints = this.EnemySpawnPoint.Values;
			Random rnd = new Random();
			object[] shuffleArray = (from x in spawnPoints
			orderby rnd.Next()
			select x).ToArray<object>();
			for (int i = 0; i < shuffleArray.Length; i++)
			{
				GameObject go = shuffleArray[i] as GameObject;
				if (!(go == null))
				{
					EnemySpawnPoint spawnPoint = go.GetComponentInChildren<EnemySpawnPoint>();
					yield return new WaitForSeconds(this.GetSpawnPointDelayInstantiation(spawnPoint));
					if (spawnPoint.SpawnedEnemy)
					{
						if (spawnPoint.SpawnOnArena)
						{
							spawnPoint.SpawnEnemyOnArena();
							Core.Audio.PlaySfx("event:/Key Event/HordeAppear", 0f);
						}
						else if (!spawnPoint.SpawnEnabledEnemy)
						{
							spawnPoint.SpawnedEnemy.gameObject.SetActive(true);
						}
					}
				}
			}
			yield break;
		}

		private float GetSpawnPointDelayInstantiation(EnemySpawnPoint spawnPoint)
		{
			float result = 0f;
			if (spawnPoint.SpawnEnabledEnemy || spawnPoint.SpawnOnArena)
			{
				result = Random.Range(0.1f, 0.5f);
			}
			return result;
		}

		[ArrayEditor(3, "", 0, 0, 65536)]
		public FsmArray EnemySpawnPoint;

		public const string AppearFx = "event:/Key Event/HordeAppear";
	}
}
