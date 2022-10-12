using System;
using UnityEngine;

namespace Framework.Util
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Instance
		{
			get
			{
				if (Singleton<T>.applicationIsQuitting && Application.isPlaying)
				{
					Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit. Won't create again - returning null.");
					return (T)((object)null);
				}
				object @lock = Singleton<T>._lock;
				T instance;
				lock (@lock)
				{
					if (Singleton<T>._instance == null)
					{
						Singleton<T>._instance = (T)((object)UnityEngine.Object.FindObjectOfType(typeof(T)));
						if (UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
						{
							Debug.LogError("[Singleton] Something went really wrong  - there should never be more than 1 singleton! Reopenning the scene might fix it.");
							return Singleton<T>._instance;
						}
						if (Singleton<T>._instance == null)
						{
							GameObject gameObject = new GameObject();
							Singleton<T>._instance = gameObject.AddComponent<T>();
							gameObject.name = "(singleton) " + typeof(T).ToString();
							if (Application.isPlaying)
							{
								UnityEngine.Object.DontDestroyOnLoad(gameObject);
							}
							Debug.Log("[Singleton] Created: " + Singleton<T>._instance.gameObject.name);
						}
						else
						{
							Debug.Log("[Singleton] Using instance already created: " + Singleton<T>._instance.gameObject.name);
						}
					}
					instance = Singleton<T>._instance;
				}
				return instance;
			}
		}

		public void OnApplicationQuit()
		{
			Singleton<T>.applicationIsQuitting = true;
		}

		private static T _instance;

		private static object _lock = new object();

		private static bool applicationIsQuitting = false;
	}
}
