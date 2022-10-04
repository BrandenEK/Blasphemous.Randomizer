using System;
using Framework.Managers;
using Framework.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.FrameworkCore
{
	public class PersistentObject : MonoBehaviour, PersistentInterface
	{
		public virtual bool IsIgnoringPersistence()
		{
			return this.IgnorePersistence;
		}

		public virtual bool IsOpenOrActivated()
		{
			return true;
		}

		public string GetPersistenID()
		{
			UniqueId component = base.GetComponent<UniqueId>();
			if (!component)
			{
				return string.Empty;
			}
			return component.uniqueId;
		}

		public T CreatePersistentData<T>() where T : PersistentManager.PersistentData
		{
			T t = (T)((object)Activator.CreateInstance(typeof(T), new object[]
			{
				this.GetPersistenID()
			}));
			t.debugName = base.name;
			return t;
		}

		public virtual PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			return null;
		}

		public virtual void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
		}

		public int GetOrder()
		{
			return 0;
		}

		public void ResetPersistence()
		{
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		protected bool IgnorePersistence;
	}
}
