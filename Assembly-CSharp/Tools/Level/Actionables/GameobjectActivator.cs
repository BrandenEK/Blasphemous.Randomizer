using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using Tools.Gameplay;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class GameobjectActivator : PersistentObject, IActionable
	{
		private void Awake()
		{
		}

		private void SetActiveState(bool b)
		{
			this.gameobjectsActivated = b;
			this.ActivateObjects(this.target, this.gameobjectsActivated);
		}

		private void ActivateObjects(GameObject[] gameObjects, bool active)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				if (!(gameObject == null))
				{
					gameObject.SetActive(active);
				}
			}
		}

		public void Use()
		{
			this.SetActiveState(!this.gameobjectsActivated);
		}

		public bool Locked { get; set; }

		public override bool IsOpenOrActivated()
		{
			return this.gameobjectsActivated;
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			if (!this.persistState)
			{
				return null;
			}
			BasicPersistence basicPersistence = base.CreatePersistentData<BasicPersistence>();
			basicPersistence.triggered = this.gameobjectsActivated;
			return basicPersistence;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			if (!this.persistState)
			{
				return;
			}
			BasicPersistence basicPersistence = (BasicPersistence)data;
			this.gameobjectsActivated = basicPersistence.triggered;
			this.SetActiveState(!this.gameobjectsActivated);
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		protected GameObject[] target = new GameObject[0];

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool persistState;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool gameobjectsActivated;
	}
}
