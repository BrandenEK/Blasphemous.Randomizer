using System;
using Framework.Managers;
using UnityEngine;

public class LookAtPenitent : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Core.Logic.Penitent != null)
		{
			this.target = Core.Logic.Penitent.transform;
			Vector3 vector = this.target.position - base.transform.parent.position;
			float d = this.radius;
			float num = Vector2.Distance(this.target.position, base.transform.parent.position);
			if (num < this.maxDistance)
			{
				d = Mathf.Lerp(0f, this.radius, num / this.maxDistance);
			}
			base.transform.position = Vector3.Lerp(base.transform.position, base.transform.parent.position + vector.normalized * d, 0.2f);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.parent.position, this.radius);
	}

	private Transform target;

	public float radius;

	public float maxDistance;
}
