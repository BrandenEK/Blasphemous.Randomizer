using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

public class Pendant : MonoBehaviour
{
	private void Start()
	{
		this.rigidBody = base.GetComponent<Rigidbody2D>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		this.rigidBody.AddTorque((Core.Logic.Penitent.Status.Orientation != EntityOrientation.Left) ? this.force : (-this.force));
	}

	private Rigidbody2D rigidBody;

	private float force = 10f;
}
