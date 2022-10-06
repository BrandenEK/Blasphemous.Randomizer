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
			float num = this.radius;
			float num2 = Vector2.Distance(this.target.position, base.transform.parent.position);
			if (num2 < this.maxDistance)
			{
				num = Mathf.Lerp(0f, this.radius, num2 / this.maxDistance);
			}
			base.transform.position = Vector3.Lerp(base.transform.position, base.transform.parent.position + vector.normalized * num, 0.2f);
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
