using System;
using Gameplay.GameControllers.Camera;
using UnityEngine;

public class PontiffHuskLevelScrollManager : MonoBehaviour
{
	public void Reset()
	{
		if (this._stopped)
		{
			return;
		}
		this._scrollActive = false;
		this.ScrollCamNumBound.LeftBoundary = this._startingCamBoundaries.x;
		this.ScrollCamNumBound.RightBoundary = this._startingCamBoundaries.y;
		this.PrevCamNumBound.SetBoundaries();
	}

	public void Stop()
	{
		this._stopped = true;
		this._scrollActive = false;
		this.ScrollCamNumBound.LeftBoundary = this._startingCamBoundaries.x;
		this.ScrollCamNumBound.RightBoundary = this._startingCamBoundaries.y;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Penitent"))
		{
			if (this._scrollActive)
			{
				return;
			}
			this.Activate();
		}
	}

	private void Activate()
	{
		this._startingCamBoundaries = new Vector2(this.ScrollCamNumBound.LeftBoundary, this.ScrollCamNumBound.RightBoundary);
		this._scrollActive = true;
	}

	private void LateUpdate()
	{
		if (!this._scrollActive)
		{
			return;
		}
		float num = Time.deltaTime * this.Speed;
		this.ScrollCamNumBound.LeftBoundary += num;
		this.ScrollCamNumBound.RightBoundary += num;
		this.ScrollCamNumBound.SetBoundaries();
	}

	public BoxCollider2D Collider;

	public CameraNumericBoundaries PrevCamNumBound;

	public CameraNumericBoundaries ScrollCamNumBound;

	public float Speed = 1f;

	private bool _stopped;

	private bool _scrollActive;

	private Vector2 _startingCamBoundaries;
}
