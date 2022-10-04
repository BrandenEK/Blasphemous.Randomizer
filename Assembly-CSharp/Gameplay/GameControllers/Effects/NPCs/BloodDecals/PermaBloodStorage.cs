using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.NPCs.BloodDecals
{
	public class PermaBloodStorage
	{
		public PermaBloodStorage()
		{
			this.generalStore = new Dictionary<int, List<PermaBloodStorage.PermaBloodMemento>>();
		}

		public List<PermaBloodStorage.PermaBloodMemento> GetPermaBloodSceneList(int sceneIndex)
		{
			List<PermaBloodStorage.PermaBloodMemento> result;
			if (this.generalStore.TryGetValue(sceneIndex, out result))
			{
				return result;
			}
			return null;
		}

		public void AddPermaBloodToSceneList(int sceneIndex, PermaBloodStorage.PermaBloodMemento memento)
		{
			List<PermaBloodStorage.PermaBloodMemento> list = this.GetPermaBloodSceneList(sceneIndex);
			if (list != null)
			{
				list.Add(memento);
			}
			else
			{
				list = new List<PermaBloodStorage.PermaBloodMemento>();
				list.Add(memento);
				this.generalStore.Add(sceneIndex, list);
			}
		}

		public void ClearSceneStore(int sceneIndex)
		{
			List<PermaBloodStorage.PermaBloodMemento> permaBloodSceneList = this.GetPermaBloodSceneList(sceneIndex);
			if (permaBloodSceneList != null)
			{
				permaBloodSceneList.Clear();
			}
		}

		public void RemoveGeneralStore()
		{
			if (this.generalStore.Count > 0)
			{
				this.generalStore.Clear();
			}
		}

		protected Dictionary<int, List<PermaBloodStorage.PermaBloodMemento>> generalStore;

		public struct PermaBloodMemento
		{
			public PermaBloodMemento(PermaBlood.PermaBloodType _type, Vector2 _position, Quaternion _rotation)
			{
				this.type = _type;
				this.position = _position;
				this.rotation = _rotation;
			}

			public PermaBlood.PermaBloodType type;

			public Vector2 position;

			public Quaternion rotation;
		}
	}
}
