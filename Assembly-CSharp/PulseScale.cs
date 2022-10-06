using System;
using UnityEngine;

public class PulseScale : MonoBehaviour
{
	private void Awake()
	{
		this.initialScale = base.transform.localScale;
	}

	private void Update()
	{
		float num = (1f + Mathf.Sin(Time.time * this.speed)) / 2f;
		base.transform.localScale = Vector2.Lerp(this.initialScale * this.minScale, this.initialScale * this.maxScale, num);
	}

	public float minScale = 1f;

	public float maxScale = 3f;

	public float speed;

	private Vector2 initialScale;
}
