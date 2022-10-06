using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Enemies.ChasingHead;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.HeadThrower.Animator;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.HeadThrower.AI
{
	public class HeadThrowerBehaviour : EnemyBehaviour
	{
		public HeadThrowerAnimatorInyector AnimatorInyector { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this._headThrower = (HeadThrower)this.Entity;
			this.AnimatorInyector = this._headThrower.GetComponentInChildren<HeadThrowerAnimatorInyector>();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this._headThrower.Target == null)
			{
				return;
			}
			float num = Vector2.Distance(this._headThrower.transform.position, this._headThrower.Target.transform.position);
			this._currentSpawningLapse += Time.deltaTime;
			if (num > this.MinDistanceBeforeSpawn)
			{
				return;
			}
			if (this._currentSpawningLapse >= this.SpawningInterval && (float)this.SpawnedHeadAmount < this.MaxHeadSpawned)
			{
				this._currentSpawningLapse = 0f;
				this.SpawnHead();
			}
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			this._currentSpawningLapse = 0f;
			if (this._headThrower.Status.Dead)
			{
				this.AnimatorInyector.Death();
			}
			else
			{
				this.AnimatorInyector.Damage();
			}
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public void SpawnHead()
		{
			if (this.HeadPrefab == null)
			{
				return;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this.HeadPrefab, this.SpawningRootPoint.transform.position, Quaternion.identity);
			if (gameObject != null)
			{
				this.AddSpawnedHeadToList(gameObject);
				ChasingHead component = gameObject.GetComponent<ChasingHead>();
				component.SetTarget(this._headThrower.Target);
				component.OwnHeadThrower = this._headThrower;
			}
		}

		public int SpawnedHeadAmount
		{
			get
			{
				return this._spawnedHeadList.Count;
			}
		}

		public void RemoveSpawnedHeadFromList(GameObject spawnedHead)
		{
			if (!this._spawnedHeadList.Contains(spawnedHead))
			{
				return;
			}
			this._spawnedHeadList.Remove(spawnedHead);
		}

		public void AddSpawnedHeadToList(GameObject spawnedHead)
		{
			if (this._spawnedHeadList.Contains(spawnedHead))
			{
				return;
			}
			this._spawnedHeadList.Add(spawnedHead);
		}

		private readonly List<GameObject> _spawnedHeadList = new List<GameObject>();

		private float _currentSpawningLapse;

		private float _distanceToTarget;

		private HeadThrower _headThrower;

		[FoldoutGroup("Spawning Settings", true, 0)]
		public GameObject HeadPrefab;

		[FoldoutGroup("Spawning Settings", true, 0)]
		public float MaxHeadSpawned = 3f;

		[FoldoutGroup("Spawning Settings", true, 0)]
		public float MinDistanceBeforeSpawn = 16f;

		[FoldoutGroup("Spawning Settings", true, 0)]
		public float SpawningInterval = 3f;

		[FoldoutGroup("Spawning Settings", true, 0)]
		public EnemyRootPoint SpawningRootPoint;
	}
}
