using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.PontiffHusk;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Environment.MovingPlatforms;
using Sirenix.OdinInspector;
using Tools.Level.Layout;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PontiffHuskiesCombatTrigger : MonoBehaviour
{
	private void Awake()
	{
		foreach (GameObject gameObject in this.MovingPlatformGameobjects)
		{
			StraightMovingPlatform component = gameObject.GetComponent<StraightMovingPlatform>();
			if (component)
			{
				this.movingPlatformSs.Add(component);
			}
			else
			{
				WaypointsMovingPlatform component2 = gameObject.GetComponent<WaypointsMovingPlatform>();
				if (component2)
				{
					this.movingPlatformWs.Add(component2);
				}
				else
				{
					Debug.LogError("Gameobject: " + gameObject.name + " doesn't have a platform!");
				}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.penitentEnteredTrigger)
		{
			return;
		}
		this.penitentEnteredTrigger = this.CheckTriggerMask(collision);
		if (this.penitentEnteredTrigger)
		{
			base.StartCoroutine(this.CombatSequence());
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (this.penitentEnteredTrigger)
		{
			return;
		}
		this.penitentEnteredTrigger = this.CheckTriggerMask(collision);
		if (this.penitentEnteredTrigger)
		{
			base.StartCoroutine(this.CombatSequence());
		}
	}

	private bool CheckTriggerMask(Collider2D collision)
	{
		return (this.TriggerMask.value & 1 << collision.gameObject.layer) > 0;
	}

	[BoxGroup("Friendly Reminder", true, false, 0)]
	[InfoBox("This Button only serves debugging purposes.", 1, null)]
	[Button(0)]
	public void ResetState()
	{
		Debug.LogError("ResetState!");
		if (Application.isPlaying)
		{
			this.spawnedEnemies.ForEach(delegate(PontiffHuskRanged x)
			{
				x.Behaviour.ResetState();
			});
			this.spawnedEnemies.Clear();
			this.movingPlatformSs.ForEach(delegate(StraightMovingPlatform x)
			{
				x.Reset();
			});
			this.movingPlatformWs.ForEach(delegate(WaypointsMovingPlatform x)
			{
				x.ResetPlatform();
			});
			this.penitentEnteredTrigger = false;
			this.allEnemiesDied = false;
			this.InCombat = false;
		}
	}

	private IEnumerator CombatSequence()
	{
		this.InCombat = true;
		foreach (EnemySpawnPoint curSpawnPoint in this.ShuffleEnemiesArray())
		{
			if (!(curSpawnPoint == null))
			{
				yield return new WaitForSeconds(this.GetSpawnPointDelayInstantiation(curSpawnPoint));
				if (curSpawnPoint.SpawnedEnemy)
				{
					if (curSpawnPoint.SpawnOnArena)
					{
						curSpawnPoint.SpawnEnemyOnArena();
						Core.Audio.PlaySfx("event:/Key Event/HordeAppear", 0f);
					}
					else if (!curSpawnPoint.SpawnEnabledEnemy)
					{
						curSpawnPoint.SpawnedEnemy.gameObject.SetActive(true);
					}
					this.spawnedEnemies.Add(curSpawnPoint.SpawnedEnemy as PontiffHuskRanged);
				}
			}
		}
		this.SetEnemiesLife();
		this.SuscribeToEnemiesDeaths();
		yield return new WaitUntil(() => this.allEnemiesDied);
		this.movingPlatformSs.ForEach(delegate(StraightMovingPlatform x)
		{
			x.Use();
		});
		this.movingPlatformWs.ForEach(delegate(WaypointsMovingPlatform x)
		{
			x.Use();
		});
		yield return new WaitForSeconds(1f);
		this.InCombat = false;
		yield break;
	}

	private EnemySpawnPoint[] ShuffleEnemiesArray()
	{
		EnemySpawnPoint[] enemies = this.Enemies;
		Random rnd = new Random();
		return (from x in enemies
		orderby rnd.Next()
		select x).ToArray<EnemySpawnPoint>();
	}

	private void SetEnemiesLife()
	{
		foreach (PontiffHuskRanged pontiffHuskRanged in this.spawnedEnemies)
		{
			pontiffHuskRanged.Status.Dead = false;
			pontiffHuskRanged.Stats.Life.Current = pontiffHuskRanged.Stats.LifeBase;
		}
	}

	private void SuscribeToEnemiesDeaths()
	{
		foreach (PontiffHuskRanged pontiffHuskRanged in this.spawnedEnemies)
		{
			pontiffHuskRanged.OnEntityDeath += this.CheckForAliveEnemies;
		}
	}

	private void CheckForAliveEnemies(Entity lastDeadEnemy)
	{
		lastDeadEnemy.OnEntityDeath -= this.CheckForAliveEnemies;
		PontiffHuskRanged item = lastDeadEnemy as PontiffHuskRanged;
		this.spawnedEnemies.Remove(item);
		this.allEnemiesDied = (this.spawnedEnemies.Count == 0);
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

	[BoxGroup("Camera Settings", true, false, 0)]
	public bool RemovesCameraInfluenceDuringCombat;

	[BoxGroup("Camera Settings", true, false, 0)]
	[HideInInspector]
	public bool InCombat;

	[BoxGroup("Enemies Spawns Settings", true, false, 0)]
	public EnemySpawnPoint[] Enemies;

	[BoxGroup("Moving Platforms To Use", true, false, 0)]
	public List<GameObject> MovingPlatformGameobjects = new List<GameObject>();

	[BoxGroup("Penitent Trigger Mask", true, false, 0)]
	public LayerMask TriggerMask;

	public const string AppearFx = "event:/Key Event/HordeAppear";

	private List<StraightMovingPlatform> movingPlatformSs = new List<StraightMovingPlatform>();

	private List<WaypointsMovingPlatform> movingPlatformWs = new List<WaypointsMovingPlatform>();

	private bool penitentEnteredTrigger;

	private bool allEnemiesDied;

	private List<PontiffHuskRanged> spawnedEnemies = new List<PontiffHuskRanged>();
}
