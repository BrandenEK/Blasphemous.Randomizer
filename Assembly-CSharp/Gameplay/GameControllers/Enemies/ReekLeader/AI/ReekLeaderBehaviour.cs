using System;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.ReekLeader.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ReekLeader.AI
{
	public class ReekLeaderBehaviour : EnemyBehaviour
	{
		public ReekLeader ReekLeader { get; private set; }

		public ReekSpawner ReekSpawner { get; private set; }

		public bool IsDefensiveMode { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this.ReekLeader = (ReekLeader)this.Entity;
			this.ReekSpawner = base.GetComponentInChildren<ReekSpawner>();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.ReekLeader.Status.Dead)
			{
				return;
			}
			this._currentSummoningLapse += Time.deltaTime;
			float num = Vector2.Distance(base.transform.position, this.ReekLeader.Target.transform.position);
			if (num <= this.DefensiveDistance && !this.IsDefensiveMode)
			{
				this.IsDefensiveMode = true;
			}
			if (this._currentSummoningLapse >= this.SummoningLapse)
			{
				this._currentSummoningLapse = 0f;
				this.SummonReek();
			}
		}

		public override void Idle()
		{
		}

		public override void Wander()
		{
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void Attack()
		{
		}

		public void SummonReek()
		{
			int summonedReekAmount = this.ReekSpawner.SummonedReekAmount;
			if (this.IsDefensiveMode)
			{
				if (summonedReekAmount <= this.DefensiveModeMaxReekSummoned)
				{
					ReekSpawnPoint spawnPoint = this.GetSpawnPoint();
					if (spawnPoint == null)
					{
						return;
					}
					spawnPoint.SpawnedEntityId = this.ReekSpawner.InstanceReek(spawnPoint.transform.position).GetInstanceID();
					this.ReekLeader.AnimatorInyector.Spawn();
					this.ReekLeader.Audio.StopIdle();
				}
				else
				{
					this._currentSummoningLapse = this.SummoningLapse - 1f;
				}
			}
			else if (summonedReekAmount < this.OffensiveModeMaxReekSummoned)
			{
				ReekSpawnPoint spawnPoint2 = this.GetSpawnPoint();
				if (spawnPoint2 == null)
				{
					return;
				}
				spawnPoint2.SpawnedEntityId = this.ReekSpawner.InstanceReek(spawnPoint2.transform.position).GetInstanceID();
				this.ReekLeader.AnimatorInyector.Spawn();
				this.ReekLeader.Audio.StopIdle();
			}
			else
			{
				this._currentSummoningLapse = this.SummoningLapse - 1f;
			}
		}

		private ReekSpawnPoint GetSpawnPoint()
		{
			return (!this.IsDefensiveMode) ? this.ReekSpawner.GetPlayerClosestReekSpawnPoint() : this.ReekSpawner.GetNearestReekSpawnPoint();
		}

		public override void Damage()
		{
			if (this.ReekLeader == null)
			{
				return;
			}
			this.ReekLeader.AnimatorInyector.Hurt();
			this.ReekLeader.Audio.StopIdle();
			this.ReekLeader.Audio.StopCall();
		}

		public void Death()
		{
			if (this.ReekLeader == null)
			{
				return;
			}
			this.ReekLeader.AnimatorInyector.Death();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		[FoldoutGroup("Attack Settings", true, 0)]
		public float SpawningDistanceToPlayer = 4f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float DefensiveDistance = 4f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float SummoningLapse = 5f;

		private float _currentSummoningLapse;

		[FoldoutGroup("Attack Settings", true, 0)]
		public int DefensiveModeMaxReekSummoned = 1;

		[FoldoutGroup("Attack Settings", true, 0)]
		public int OffensiveModeMaxReekSummoned = 3;
	}
}
