using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	public class CliffLedeRootTarget : MonoBehaviour
	{
		private void OnDrawGizmosSelected()
		{
			Color color = new Color(0.21f, 0.8f, 0.2f, 1f);
			Gizmos.color = color;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
	}
}
