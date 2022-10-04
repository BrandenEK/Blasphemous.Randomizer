using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	public class TestRicochetRay : MonoBehaviour
	{
		private void Start()
		{
			this.results = new RaycastHit2D[1];
		}

		public void Update()
		{
			if (!Core.ready || Core.Logic.Penitent == null)
			{
				return;
			}
			Vector2[] array = new Vector2[this.numBounces];
			Vector2[] array2 = new Vector2[this.numBounces];
			Vector2[] array3 = new Vector2[this.numBounces];
			array[0] = base.transform.position;
			array2[0] = Core.Logic.Penitent.transform.position - base.transform.position;
			for (int i = 0; i < this.numBounces; i++)
			{
				if (!this.ThrowRay(array[i], array2[i]))
				{
					break;
				}
				array3[i] = this.results[0].point;
				GizmoExtensions.DrawDebugCross(array3[i], Color.green, 0.5f);
				if (i + 1 < this.numBounces)
				{
					array2[i + 1] = this.CalculateBounceDirection(array3[i] - array[i], this.results[0]);
					array[i + 1] = array3[i] + array2[i + 1] * 0.01f;
				}
			}
		}

		private Vector2 CalculateBounceDirection(Vector2 direction, RaycastHit2D hit)
		{
			return Vector3.Reflect(direction, hit.normal).normalized;
		}

		private bool ThrowRay(Vector2 startPoint, Vector2 direction)
		{
			bool result = false;
			if (Physics2D.RaycastNonAlloc(startPoint, direction, this.results, 100f, this.mask) > 0)
			{
				Debug.DrawRay(startPoint, direction.normalized * this.results[0].distance, Color.red);
				result = true;
			}
			else
			{
				Debug.DrawRay(startPoint, direction.normalized * 100f, Color.yellow);
			}
			return result;
		}

		public int numBounces = 10;

		public LayerMask mask;

		private RaycastHit2D[] results;
	}
}
