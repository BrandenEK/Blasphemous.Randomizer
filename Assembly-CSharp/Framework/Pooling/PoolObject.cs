using System;
using UnityEngine;

namespace Framework.Pooling
{
	public class PoolObject : MonoBehaviour
	{
		public virtual void OnObjectReuse()
		{
		}

		protected void Destroy()
		{
			if (base.gameObject != null)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
