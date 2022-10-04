using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.Persistence
{
	public class PersistentEnemy : PersistentObject
	{
		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			PersistentEnemy.PietyPersistenceData pietyPersistenceData = base.CreatePersistentData<PersistentEnemy.PietyPersistenceData>();
			pietyPersistenceData.IsAlive = (this.enemy.CurrentLife > 0f);
			pietyPersistenceData.Position = this.enemy.transform.localPosition;
			return pietyPersistenceData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			PersistentEnemy.PietyPersistenceData pietyPersistenceData = (PersistentEnemy.PietyPersistenceData)data;
			this.enemy.gameObject.SetActive(pietyPersistenceData.IsAlive);
			foreach (GameObject gameObject in this.objectsToEnableWhenDead)
			{
				if (!(gameObject == null))
				{
					gameObject.SetActive(!pietyPersistenceData.IsAlive);
				}
			}
			foreach (GameObject gameObject2 in this.objectsToDisbleWhenDead)
			{
				if (!(gameObject2 == null))
				{
					gameObject2.SetActive(pietyPersistenceData.IsAlive);
				}
			}
			if (this.SpriteDead)
			{
				this.SpriteDead.SetActive(!pietyPersistenceData.IsAlive);
				this.SpriteDead.transform.localPosition = pietyPersistenceData.Position;
			}
		}

		public Enemy enemy;

		public List<GameObject> objectsToDisbleWhenDead = new List<GameObject>();

		public List<GameObject> objectsToEnableWhenDead = new List<GameObject>();

		public GameObject SpriteDead;

		private class PietyPersistenceData : PersistentManager.PersistentData
		{
			public PietyPersistenceData(string id) : base(id)
			{
			}

			public bool IsAlive;

			public Vector3 Position;
		}
	}
}
