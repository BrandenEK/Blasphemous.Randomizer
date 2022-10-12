using System;
using System.Collections.Generic;
using Framework.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.GameControllers.Effects.NPCs.BloodDecals
{
	public class PermaBloodSpawner : MonoBehaviour
	{
		private void Awake()
		{
			this.permaBloodDic = this.getPermaBloodDictionary(this.permaBloods);
		}

		private void Start()
		{
			this.spawnPermaBloodInCurrentScene();
		}

		public void spawnPermaBloodInCurrentScene()
		{
			if (this.permaBloodDic.Count > 0)
			{
				this.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
				List<PermaBloodStorage.PermaBloodMemento> permaBloodSceneList = Core.Logic.PermaBloodStore.GetPermaBloodSceneList(this.currentSceneIndex);
				if (permaBloodSceneList != null)
				{
					for (int i = 0; i < permaBloodSceneList.Count; i++)
					{
						GameObject original = this.permaBloodDic[permaBloodSceneList[i].type];
						Vector2 position = permaBloodSceneList[i].position;
						Quaternion rotation = permaBloodSceneList[i].rotation;
						UnityEngine.Object.Instantiate<GameObject>(original, position, rotation);
					}
				}
			}
		}

		protected Dictionary<PermaBlood.PermaBloodType, GameObject> getPermaBloodDictionary(PermaBlood[] permaBloods)
		{
			Dictionary<PermaBlood.PermaBloodType, GameObject> dictionary = new Dictionary<PermaBlood.PermaBloodType, GameObject>();
			if (permaBloods.Length > 0)
			{
				byte b = 0;
				while ((int)b < permaBloods.Length)
				{
					dictionary.Add(permaBloods[(int)b].permaBloodType, permaBloods[(int)b].gameObject);
					b += 1;
				}
			}
			return dictionary;
		}

		public PermaBlood[] permaBloods;

		protected Dictionary<PermaBlood.PermaBloodType, GameObject> permaBloodDic;

		private int currentSceneIndex;
	}
}
