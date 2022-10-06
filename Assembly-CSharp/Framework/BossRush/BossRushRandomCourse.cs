using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.BossRush
{
	[Serializable]
	public class BossRushRandomCourse : BossRushCourse
	{
		public override List<string> GetScenes()
		{
			if (this.RandomizeNextScenesList)
			{
				this.RandomizeNextScenesList = false;
				this.RandomizeScenesList();
			}
			return this.RandomizedScenes;
		}

		public override List<string> GetFontRechargingScenes()
		{
			List<string> list = new List<string>();
			if (this.RandomizedScenes.Count > 0)
			{
				foreach (int index in this.FontRechargingScenesIndexes)
				{
					list.Add(this.RandomizedScenes[index]);
				}
			}
			return list;
		}

		[Button(0)]
		private void RandomizeScenesList()
		{
			this.RandomizedScenes.Clear();
			this.RandomizedScenes.AddRange(this.ScenesPool);
			this.RandomizedScenes = this.Shuffle(this.RandomizedScenes);
		}

		private List<string> Shuffle(List<string> scenes)
		{
			for (int i = 0; i < scenes.Count; i++)
			{
				if (!this.FixedScenesIndexes.Contains(i))
				{
					string value = scenes[i];
					int num = Random.Range(i, scenes.Count);
					if (!this.FixedScenesIndexes.Contains(num))
					{
						scenes[i] = scenes[num];
						scenes[num] = value;
					}
				}
			}
			return scenes;
		}

		[InfoBox("Put here all the scenes of the course, fixed in place or not.", 1, null)]
		public List<string> ScenesPool = new List<string>();

		[InfoBox("The indexes of these scenes should consider all the scenes in total, and these indexes refer to the scenes pool.", 1, null)]
		public List<int> FixedScenesIndexes = new List<int>();

		[InfoBox("The indexes of these scenes should consider all the scenes in total. These indexes will refer to the randomized result.", 1, null)]
		public List<int> FontRechargingScenesIndexes = new List<int>();

		[HideInInspector]
		public bool RandomizeNextScenesList;

		private List<string> RandomizedScenes = new List<string>();
	}
}
