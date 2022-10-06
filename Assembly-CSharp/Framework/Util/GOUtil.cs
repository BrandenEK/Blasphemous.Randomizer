using System;
using UnityEngine;

namespace Framework.Util
{
	public class GOUtil
	{
		public static Transform FindParent(GameObject gameobject, string name)
		{
			Transform transform = gameobject.transform;
			while (transform != null)
			{
				if (transform.name == name)
				{
					return transform;
				}
				transform = transform.parent;
			}
			return null;
		}

		public static void PixelPerfectPosition(GameObject go)
		{
			float num = Mathf.Floor(go.transform.position.z * 32f) / 32f;
			float num2 = Mathf.Floor(go.transform.position.z * 32f) / 32f;
			go.transform.position = new Vector3(num, num2, go.transform.position.z);
		}
	}
}
