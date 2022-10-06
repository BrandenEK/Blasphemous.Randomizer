using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.GameControllers.Effects.NPCs.BloodDecals
{
	public class BloodDecalPumper : MonoBehaviour
	{
		private void Awake()
		{
			this.bloodDecalsQuantity = this.bloodDecals.Length;
			this.feedBloodDecalKeys(this.bloodDecalsQuantity);
			this.lastBloodDecalRandomKey = Random.Range(0, this.bloodDecals.Length);
			this.permaBloodSpawnPoint = base.GetComponentInChildren<SpawnPoint>();
			this.entity = base.GetComponentInParent<Entity>();
		}

		private void Update()
		{
			if (this.hasPermaBlood)
			{
				this.setPermaBloodPosition();
			}
		}

		public BloodDecal GetBloodDecal(Vector3 position)
		{
			return null;
		}

		protected void storeBloodDecal(int bloodDecalKey, BloodDecal bloodDecal)
		{
			if (!this.bloodDecalsStore.ContainsKey(bloodDecalKey))
			{
				this.bloodDecalsStore.Add(bloodDecalKey, bloodDecal);
			}
		}

		public void DisposeBloodDecal(BloodDecal bloodDecal)
		{
			if (bloodDecal != null)
			{
				bloodDecal.gameObject.SetActive(false);
			}
		}

		public void DrainBloodDecalPool()
		{
			if (this.bloodDecalsStore.Count > 0)
			{
				this.bloodDecalsStore.Clear();
			}
		}

		protected int getBloodDecalRandomKey(int bloodDecalsCollectionRange)
		{
			int num = 0;
			if (bloodDecalsCollectionRange > 0)
			{
				num = Random.Range(num, bloodDecalsCollectionRange);
			}
			return num;
		}

		protected int getNewBloodDecalKey()
		{
			if (this.bloodDecalsKeys.Count > 0)
			{
				return this.retrieveFirstRoundKey();
			}
			int num = 0;
			if (this.bloodDecalsQuantity > 1)
			{
				do
				{
					num = this.getBloodDecalRandomKey(this.bloodDecalsQuantity);
				}
				while (this.lastBloodDecalRandomKey == num);
			}
			return num;
		}

		private void feedBloodDecalKeys(int _bloodDecalsQuantity)
		{
			byte b = 0;
			while ((int)b < _bloodDecalsQuantity)
			{
				this.bloodDecalsKeys.Add((int)b);
				b += 1;
			}
		}

		protected int retrieveFirstRoundKey()
		{
			int index = Random.Range(0, this.bloodDecalsKeys.Count);
			int num = this.bloodDecalsKeys[index];
			this.bloodDecalsKeys.Remove(num);
			return num;
		}

		protected GameObject instanceBloodDecal(BloodDecal bloodDecalPrefab, Vector3 pos, Quaternion rotation)
		{
			return Object.Instantiate<GameObject>(bloodDecalPrefab.gameObject, pos, rotation);
		}

		public SpawnPoint PermaBloodSpawnPoint
		{
			get
			{
				return this.permaBloodSpawnPoint;
			}
		}

		public void setPermaBloodPosition()
		{
			Vector3 localPosition = this.permaBloodSpawnPoint.transform.localPosition;
			localPosition.x = Mathf.Abs(localPosition.x);
			if (this.entity.Status.Orientation == EntityOrientation.Left)
			{
				localPosition.x *= -1f;
			}
			this.permaBloodSpawnPoint.transform.localPosition = localPosition;
		}

		protected void addPermaBloodToSceneStore(PermaBlood permaBlood)
		{
			Vector2 position = this.permaBloodSpawnPoint.transform.position;
			Quaternion rotation = this.permaBloodSpawnPoint.transform.rotation;
			PermaBloodStorage.PermaBloodMemento memento = new PermaBloodStorage.PermaBloodMemento(permaBlood.permaBloodType, position, rotation);
			int buildIndex = SceneManager.GetActiveScene().buildIndex;
			Core.Logic.PermaBloodStore.AddPermaBloodToSceneList(buildIndex, memento);
		}

		public BloodDecal[] bloodDecals;

		protected Dictionary<int, BloodDecal> bloodDecalsStore = new Dictionary<int, BloodDecal>();

		protected int bloodDecalsQuantity;

		protected Entity entity;

		protected int bloodDecalRandomKey;

		protected int lastBloodDecalRandomKey;

		public bool hasPermaBlood;

		protected SpawnPoint permaBloodSpawnPoint;

		private List<int> bloodDecalsKeys = new List<int>();
	}
}
