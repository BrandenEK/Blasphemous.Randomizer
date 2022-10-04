using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

public class FreeFallSensor : MonoBehaviour
{
	private void Awake()
	{
		this.controller = base.GetComponent<PlatformCharacterController>();
	}

	private void Start()
	{
		Penitent penitent = Core.Logic.Penitent;
		penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(delegate()
		{
			base.enabled = false;
		}));
	}

	private void FixedUpdate()
	{
		if (this.controller.InstantVelocity.y > this.minVerticalFallSpeed)
		{
			this.fallingTime = 0f;
			return;
		}
		this.fallingTime += Time.fixedDeltaTime;
		if (this.fallingTime > this.maxFallingTime)
		{
			base.enabled = false;
			this.KillPlayer();
		}
	}

	private void KillPlayer()
	{
		Penitent penitent = Core.Logic.Penitent;
		if (penitent == null)
		{
			return;
		}
		if (penitent.Stats.Life.Current > 0f)
		{
			penitent.Stats.Life.Current = 0f;
		}
		penitent.KillInstanteneously();
		if (this.controller)
		{
			this.controller.enabled = false;
		}
		Core.Logic.CameraManager.ProCamera2D.FollowVertical = false;
	}

	public float maxFallingTime = 5f;

	public float minVerticalFallSpeed = -1f;

	private PlatformCharacterController controller;

	private float fallingTime;
}
