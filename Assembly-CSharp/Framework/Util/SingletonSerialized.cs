using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Util
{
	public class SingletonSerialized<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
	{
		public static T Instance
		{
			get
			{
				if (SingletonSerialized<T>.applicationIsQuitting)
				{
					Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit. Won't create again - returning null.");
					return (T)((object)null);
				}
				object @lock = SingletonSerialized<T>._lock;
				T instance;
				lock (@lock)
				{
					if (SingletonSerialized<T>._instance == null)
					{
						SingletonSerialized<T>._instance = (T)((object)UnityEngine.Object.FindObjectOfType(typeof(T)));
						if (UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
						{
							Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopenning the scene might fix it.");
							return SingletonSerialized<T>._instance;
						}
						if (SingletonSerialized<T>._instance == null)
						{
							GameObject gameObject = new GameObject();
							SingletonSerialized<T>._instance = gameObject.AddComponent<T>();
							gameObject.name = "(singleton) " + typeof(T).ToString();
							UnityEngine.Object.DontDestroyOnLoad(gameObject);
						}
						else
						{
							Debug.Log("[Singleton] Using instance already created: " + SingletonSerialized<T>._instance.gameObject.name);
						}
					}
					instance = SingletonSerialized<T>._instance;
				}
				return instance;
			}
		}

		public void OnApplicationQuit()
		{
			SingletonSerialized<T>.applicationIsQuitting = true;
		}

		private static T _instance;

		private static object _lock = new object();

		private static bool applicationIsQuitting = false;
	}
}
