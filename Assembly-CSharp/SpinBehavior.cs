using System;
using UnityEngine;

public class SpinBehavior : MonoBehaviour
{
	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, 0f, this.angularSpeed * Time.deltaTime));
	}

	public float angularSpeed;
}
