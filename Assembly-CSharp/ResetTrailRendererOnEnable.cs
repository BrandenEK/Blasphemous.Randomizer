using System;
using UnityEngine;

public class ResetTrailRendererOnEnable : MonoBehaviour
{
	private void OnEnable()
	{
		this.Clean();
	}

	private void OnDisable()
	{
		this.Clean();
	}

	public void Clean()
	{
		TrailRenderer component = base.GetComponent<TrailRenderer>();
		if (component)
		{
			component.Clear();
		}
	}
}
