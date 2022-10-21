using System;
using BezierSplines;
using Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Tools.Level.Utils
{
	public class FlyingPatrollingEnemySpawnConfigurator : EnemySpawnConfigurator
	{
		protected override void OnSpawn(Enemy e)
		{
			base.OnSpawn(e);
			if (e.GetType() == typeof(PatrollingFlyingEnemy))
			{
				((PatrollingFlyingEnemy)e).SetConfig(this.path, this.curve, this.secondsToCompletePatrol);
			}
		}

		public BezierSpline path;

		public AnimationCurve curve;

		public float secondsToCompletePatrol = 2f;
	}
}
