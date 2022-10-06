using System;
using System.Collections.Generic;
using Framework.Pooling;
using UnityEngine;

namespace Framework.Managers
{
	public class PoolManager : MonoBehaviour
	{
		public static PoolManager Instance
		{
			get
			{
				if (PoolManager._instance == null)
				{
					PoolManager._instance = Object.FindObjectOfType<PoolManager>();
				}
				return PoolManager._instance;
			}
		}

		public void CreatePool(GameObject prefab, int poolSize)
		{
			int instanceID = prefab.GetInstanceID();
			if (!this.poolDictionary.ContainsKey(instanceID))
			{
				this.poolDictionary.Add(instanceID, new Queue<PoolManager.ObjectInstance>());
				GameObject gameObject = new GameObject(prefab.name + " pool");
				gameObject.transform.parent = base.transform;
				for (int i = 0; i < poolSize; i++)
				{
					PoolManager.ObjectInstance objectInstance = new PoolManager.ObjectInstance(Object.Instantiate<GameObject>(prefab));
					this.poolDictionary[instanceID].Enqueue(objectInstance);
					objectInstance.SetParent(gameObject.transform);
				}
			}
			else
			{
				GameObject gameObject2 = GameObject.Find(prefab.name + " pool");
				if (gameObject2 == null)
				{
					return;
				}
				for (int j = 0; j < poolSize; j++)
				{
					PoolManager.ObjectInstance objectInstance2 = new PoolManager.ObjectInstance(Object.Instantiate<GameObject>(prefab));
					this.poolDictionary[instanceID].Enqueue(objectInstance2);
					objectInstance2.SetParent(gameObject2.transform);
				}
			}
		}

		public PoolManager.ObjectInstance ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation, bool createPoolIfNeeded = false, int poolSize = 1)
		{
			int instanceID = prefab.GetInstanceID();
			PoolManager.ObjectInstance objectInstance = null;
			if (this.poolDictionary.ContainsKey(instanceID))
			{
				objectInstance = this.poolDictionary[instanceID].Dequeue();
				this.poolDictionary[instanceID].Enqueue(objectInstance);
				objectInstance.Reuse(position, rotation);
			}
			else if (createPoolIfNeeded)
			{
				this.CreatePool(prefab, poolSize);
				objectInstance = this.ReuseObject(prefab, position, rotation, false, 1);
			}
			return objectInstance;
		}

		private static PoolManager _instance;

		private readonly Dictionary<int, Queue<PoolManager.ObjectInstance>> poolDictionary = new Dictionary<int, Queue<PoolManager.ObjectInstance>>();

		public class ObjectInstance
		{
			public ObjectInstance(GameObject objectInstance)
			{
				this.gameObject = objectInstance;
				this.transform = this.gameObject.transform;
				this.gameObject.SetActive(false);
				if (this.gameObject.GetComponent<PoolObject>())
				{
					this.hasPoolObjectComponent = true;
					this.poolObjectScripts = this.gameObject.GetComponents<PoolObject>();
				}
			}

			public GameObject GameObject
			{
				get
				{
					return this.gameObject;
				}
			}

			public void Reuse(Vector3 position, Quaternion rotation)
			{
				if (!this.gameObject)
				{
					return;
				}
				this.gameObject.SetActive(true);
				this.transform.position = position;
				this.transform.rotation = rotation;
				if (this.hasPoolObjectComponent)
				{
					foreach (PoolObject poolObject in this.poolObjectScripts)
					{
						poolObject.OnObjectReuse();
					}
				}
			}

			public void SetParent(Transform parent)
			{
				this.transform.parent = parent;
			}

			private readonly GameObject gameObject;

			private readonly bool hasPoolObjectComponent;

			private readonly PoolObject[] poolObjectScripts;

			private readonly Transform transform;
		}
	}
}
