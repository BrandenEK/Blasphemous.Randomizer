using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using UnityEngine;

public class PenitentCancelEffect : MonoBehaviour
{
	private void Start()
	{
		if (this.cancelVFX != null)
		{
			PoolManager.Instance.CreatePool(this.cancelVFX, 3);
		}
	}

	public void PlayCancelEffect()
	{
		PoolManager.Instance.ReuseObject(this.cancelVFX, this.GetSpawnPosition(), base.transform.rotation, false, 1);
		if (this.shaderEffects)
		{
			this.shaderEffects.TriggerColorFlash();
		}
	}

	private Vector2 GetSpawnPosition()
	{
		Vector2 offset = this.Offset;
		offset.x = ((Core.Logic.Penitent.Status.Orientation != EntityOrientation.Left) ? this.Offset.x : (-this.Offset.x));
		return base.transform.position + offset;
	}

	public GameObject cancelVFX;

	public MasterShaderEffects shaderEffects;

	public Vector2 Offset;
}
