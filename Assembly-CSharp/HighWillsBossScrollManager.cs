using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Environment.MovingPlatforms;
using UnityEngine;

public class HighWillsBossScrollManager : MonoBehaviour
{
	private void Start()
	{
		foreach (GameObject gameObject in this.Modules)
		{
			this._modulesPositions.Add(gameObject.transform.position);
		}
		foreach (GameObject gameObject2 in this.ScrollableItems)
		{
			this._scrollableItemsPositions.Add(gameObject2.transform.position);
		}
	}

	public void Reset()
	{
		Debug.LogError("Reset");
		this._timeUntilNextModule = this.TimeToCycleNewModule;
		this._scrollActive = false;
		this.ResetAllPlatforms();
		for (int i = 0; i < this.Modules.Count; i++)
		{
			this.Modules[i].transform.position = this._modulesPositions[i];
		}
		for (int j = 0; j < this.ScrollableItems.Count; j++)
		{
			this.ScrollableItems[j].transform.position = this._scrollableItemsPositions[j];
		}
		this.BossCamNumBound.LeftBoundary = this._startingCamBoundaries.x;
		this.BossCamNumBound.RightBoundary = this._startingCamBoundaries.y;
		this.NormalCamNumBound.SetBoundaries();
		this._nextModuleIndex = 0;
	}

	public void Stop()
	{
		Debug.LogError("Stop");
		this._modulesActive = false;
		this.StopAllPlatforms();
	}

	public void ActivateModules()
	{
		Debug.LogError("ActivateModules");
		this._modulesActive = true;
		this._timeUntilNextModule = this.TimeToCycleNewModule;
		this.Modules.ForEach(delegate(GameObject x)
		{
			x.SetActive(true);
		});
		this.UseStartingPlatforms();
		this.LevelScrollManager.Stop();
	}

	public void SetBossCamBounds()
	{
		Debug.LogError("SetBossCamBounds");
		this._startingCamBoundaries = new Vector2(this.BossCamNumBound.LeftBoundary, this.BossCamNumBound.RightBoundary);
		this.BossCamNumBound.SetBoundaries();
	}

	public void ActivateScroll()
	{
		Debug.LogError("ActivateScroll");
		this._scrollActive = true;
	}

	private void LateUpdate()
	{
		if (!this._modulesActive)
		{
			return;
		}
		this._timeUntilNextModule -= Time.deltaTime;
		if (!this._scrollActive)
		{
			return;
		}
		float num = Time.deltaTime * this.Speed;
		foreach (GameObject gameObject in this.ScrollableItems)
		{
			gameObject.transform.position += Vector3.right * num;
		}
		this.BossCamNumBound.LeftBoundary += num;
		this.BossCamNumBound.RightBoundary += num;
		this.BossCamNumBound.SetBoundaries();
		this.CheckToCycleNewModule();
	}

	private void CheckToCycleNewModule()
	{
		Debug.LogError("CheckToCycleNewModule");
		if (this._timeUntilNextModule <= 0f)
		{
			this._timeUntilNextModule = this.TimeToCycleNewModule;
			this.CycleNewModule();
		}
	}

	private void CycleNewModule()
	{
		Debug.LogError("CycleNewModule");
		GameObject gameObject = this.Modules[this._nextModuleIndex];
		WaypointsMovingPlatform[] componentsInChildren = gameObject.GetComponentsInChildren<WaypointsMovingPlatform>();
		foreach (WaypointsMovingPlatform waypointsMovingPlatform in componentsInChildren)
		{
			waypointsMovingPlatform.ResetPlatform();
		}
		gameObject.transform.position += Vector3.right * this.ModuleWidth * (float)this.Modules.Count;
		foreach (WaypointsMovingPlatform waypointsMovingPlatform2 in componentsInChildren)
		{
			waypointsMovingPlatform2.Use();
		}
		this._nextModuleIndex++;
		if (this._nextModuleIndex == this.Modules.Count)
		{
			this._nextModuleIndex = 0;
		}
	}

	private void ResetAllPlatforms()
	{
		Debug.LogError("ResetAllPlatforms");
		foreach (GameObject gameObject in this.Modules)
		{
			WaypointsMovingPlatform[] componentsInChildren = gameObject.GetComponentsInChildren<WaypointsMovingPlatform>();
			foreach (WaypointsMovingPlatform waypointsMovingPlatform in componentsInChildren)
			{
				waypointsMovingPlatform.ResetPlatform();
			}
		}
	}

	private void StopAllPlatforms()
	{
		Debug.LogError("StopAllPlatforms");
		foreach (GameObject gameObject in this.Modules)
		{
			WaypointsMovingPlatform[] componentsInChildren = gameObject.GetComponentsInChildren<WaypointsMovingPlatform>();
			foreach (WaypointsMovingPlatform waypointsMovingPlatform in componentsInChildren)
			{
				waypointsMovingPlatform.Use();
			}
		}
	}

	private void UseStartingPlatforms()
	{
		Debug.LogError("UseStartingPlatforms");
		GameObject gameObject = this.Modules[this.Modules.Count - 1];
		WaypointsMovingPlatform[] componentsInChildren = gameObject.GetComponentsInChildren<WaypointsMovingPlatform>();
		foreach (WaypointsMovingPlatform waypointsMovingPlatform in componentsInChildren)
		{
			waypointsMovingPlatform.Use();
		}
	}

	public HighWillsLevelScrollManager LevelScrollManager;

	public List<GameObject> Modules;

	public CameraNumericBoundaries NormalCamNumBound;

	public CameraNumericBoundaries BossCamNumBound;

	public List<GameObject> ScrollableItems;

	public float Speed = 1f;

	public float ModuleWidth = 31f;

	public float TimeToCycleNewModule = 14f;

	private bool _modulesActive;

	private bool _scrollActive;

	private List<Vector2> _modulesPositions = new List<Vector2>();

	private List<Vector2> _scrollableItemsPositions = new List<Vector2>();

	private Vector2 _startingCamBoundaries;

	private int _nextModuleIndex;

	private float _timeUntilNextModule;
}
