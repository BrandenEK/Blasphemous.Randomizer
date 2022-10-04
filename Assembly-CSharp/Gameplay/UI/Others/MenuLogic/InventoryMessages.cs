using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class InventoryMessages : MonoBehaviour
	{
		private void Start()
		{
			IEnumerator enumerator = Enum.GetValues(typeof(InventoryMessages.Messages)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					InventoryMessages.Messages key = (InventoryMessages.Messages)obj;
					this.cacheObjs[key] = base.transform.Find(key.ToString()).gameObject;
					this.cacheObjs[key].SetActive(false);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.ShowMessage(InventoryMessages.Messages.Msg_Default);
		}

		public void ShowMessage(InventoryMessages.Messages message)
		{
			this.cacheObjs[this.currentMessage].SetActive(false);
			this.currentMessage = message;
			this.cacheObjs[this.currentMessage].SetActive(true);
		}

		private InventoryMessages.Messages currentMessage;

		private Dictionary<InventoryMessages.Messages, GameObject> cacheObjs = new Dictionary<InventoryMessages.Messages, GameObject>();

		public enum Messages
		{
			Msg_Default,
			Msg_Relics1,
			Msg_Relics2,
			Msg_Rosary1,
			Msg_Rosary2,
			Msg_QuestItem,
			Msg_Prayer1,
			Msg_Prayer2,
			Msg_Prayer3
		}
	}
}
