using System;
using UnityEngine;

public class SimpleTimedDestruction : MonoBehaviour
{
	private void Update()
	{
		this.TTL -= Time.deltaTime;
		if (this.TTL < 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public float TTL = 1f;
}
