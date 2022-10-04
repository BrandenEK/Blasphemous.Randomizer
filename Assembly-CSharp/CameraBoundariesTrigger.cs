using System;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using UnityEngine;

public class CameraBoundariesTrigger : MonoBehaviour
{
	private void Awake()
	{
		LevelManager.OnLevelLoaded += this.OnLevelLoaded;
	}

	private void OnDestroy()
	{
		LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
	}

	private void OnLevelLoaded(Level oldLevel, Level newLevel)
	{
		this.levelLoaded = true;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (this.cameraBoundariesOnEnter == null || this.boundariesAlreadySet || !this.levelLoaded)
		{
			return;
		}
		if ((this.triggerMask.value & 1 << collision.gameObject.layer) > 0)
		{
			this.SwitchCameraBoundaries(this.cameraBoundariesOnEnter);
			this.boundariesAlreadySet = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		this.boundariesAlreadySet = false;
	}

	private void SwitchCameraBoundaries(CameraNumericBoundaries camBounds)
	{
		ProCamera2DNumericBoundaries component = Camera.main.GetComponent<ProCamera2DNumericBoundaries>();
		camBounds.SetBoundaries();
		component.ResetVerticalBoundedDuration();
		component.ResetHorizontalBoundedDuration();
	}

	public LayerMask triggerMask;

	public CameraNumericBoundaries cameraBoundariesOnEnter;

	private bool boundariesAlreadySet;

	private bool levelLoaded;
}
