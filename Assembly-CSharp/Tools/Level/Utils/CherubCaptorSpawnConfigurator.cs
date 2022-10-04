using System;
using BezierSplines;
using Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy;
using Gameplay.GameControllers.Entities;
using Tools.Level.Layout;
using UnityEngine;

namespace Tools.Level.Utils
{
	public class CherubCaptorSpawnConfigurator : EnemySpawnConfigurator
	{
		protected override void OnAwake()
		{
			base.OnAwake();
		}

		public void DisableCherubSpawn()
		{
			Debug.Log(string.Format("<color=red>CHERUB OF ID:{0} already destroyed. Won't spawn.</color>", this.cherubPersistentObject.cherubId));
			EnemySpawnPoint component = base.GetComponent<EnemySpawnPoint>();
			component.EnemySpawnDisabled = true;
		}

		public void DestroySpawnedCherub()
		{
			EnemySpawnPoint component = base.GetComponent<EnemySpawnPoint>();
			if (component.HasEnemySpawned)
			{
				UnityEngine.Object.Destroy(this.cherub.gameObject);
			}
		}

		protected override void OnSpawn(Enemy e)
		{
			base.OnSpawn(e);
			Debug.Log(string.Format("<color=red>SPAWNING CHERUB OF ID:{0} </color>", this.cherubPersistentObject.cherubId));
			(e as PatrollingFlyingEnemy).SetConfig(this.path, this.curve, this.secondsToCompletePatrol);
			e.OnDeath += this.OnCherubDeath;
			this.cherub = e;
		}

		private void OnCherubDeath()
		{
			Debug.Log(string.Format("<color=red>CHERUB OF ID:{0} DIED</color>", this.cherubPersistentObject.cherubId));
			this.cherubPersistentObject.OnCherubKilled();
		}

		public BezierSpline path;

		public AnimationCurve curve;

		public CherubCaptorPersistentObject cherubPersistentObject;

		public float secondsToCompletePatrol = 2f;

		private Enemy cherub;
	}
}
