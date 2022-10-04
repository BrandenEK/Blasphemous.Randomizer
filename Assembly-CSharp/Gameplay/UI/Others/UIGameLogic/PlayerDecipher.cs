using System;
using Framework.Inventory;
using UnityEngine;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class PlayerDecipher : MonoBehaviour
	{
		public void SetDechiperData(Prayer prayer)
		{
			int decipherMax = prayer.decipherMax;
			int currentDecipher = prayer.CurrentDecipher;
			for (int i = 0; i < 6; i++)
			{
				Transform transform = base.transform.Find("Segment" + i.ToString());
				bool flag = i < decipherMax;
				transform.gameObject.SetActive(flag);
				if (flag)
				{
					bool flag2 = i < currentDecipher;
					transform.Find("fill").gameObject.SetActive(flag2);
					transform.Find("empty").gameObject.SetActive(!flag2);
				}
			}
		}

		public const int DECHIPER_BARS_UI = 6;
	}
}
