using System;
using UnityEngine;

namespace Framework.Util
{
	public class PixelPivotMovement : MonoBehaviour
	{
		private void LateUpdate()
		{
			if (this.target == null)
			{
				this.RepositionAllChilds();
			}
			else
			{
				this.RepositionTarget();
			}
		}

		private void RepositionAllChilds()
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				GOUtil.PixelPerfectPosition(child.gameObject);
			}
		}

		private void RepositionTarget()
		{
			GOUtil.PixelPerfectPosition(base.transform.gameObject);
		}

		[SerializeField]
		private Transform target;
	}
}
