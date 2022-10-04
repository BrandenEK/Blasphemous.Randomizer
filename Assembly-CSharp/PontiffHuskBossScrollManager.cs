using System;
using System.Collections.Generic;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Environment.MovingPlatforms;
using Tools.Level.Interactables;
using UnityEngine;

public class PontiffHuskBossScrollManager : MonoBehaviour
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
		this.NormalCamNumBound.SetBoundaries();
		if (this._scrollActive)
		{
			this._scrollActive = false;
			this._timeUntilNextModule = this.TimeToCycleFirstModuleAfterReset;
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
			this._nextModuleIndex = 0;
		}
	}

	public void Stop()
	{
		this._modulesActive = false;
		this.DisableAllPlatforms();
	}

	public void ActivateModules()
	{
		this._modulesActive = true;
		this.EnableAllPlatforms();
		this._timeUntilNextModule = this.TimeToCycleFirstModuleAfterReset;
		GameObject gameObject = this.Modules[this.Modules.Count - 1];
		gameObject.SetActive(true);
		this.Modules[this._nextModuleIndex].SetActive(true);
		this.UseStartingPlatforms();
		this.LevelScrollManager.Stop();
	}

	public void SetBossCamBounds()
	{
		this._startingCamBoundaries = new Vector2(this.BossCamNumBound.LeftBoundary, this.BossCamNumBound.RightBoundary);
		this.BossCamNumBound.SetBoundaries();
	}

	public void SetExecutionCamBounds()
	{
		Vector3 position = Core.Logic.Penitent.GetPosition();
		float num = this.BossCamNumBound.RightBoundary - this.BossCamNumBound.LeftBoundary;
		float num2 = 1f;
		FakeExecution fakeExecution = UnityEngine.Object.FindObjectOfType<FakeExecution>();
		float num3 = -0.75f;
		DOTween.To(() => this.BossCamNumBound.LeftBoundary, delegate(float x)
		{
			this.BossCamNumBound.LeftBoundary = x;
		}, position.x - num / 2f + num3, num2);
		DOTween.To(() => this.BossCamNumBound.RightBoundary, delegate(float x)
		{
			this.BossCamNumBound.RightBoundary = x;
		}, position.x + num / 2f + num3, num2);
		DOTween.To(() => this.BossCamNumBound.BottomBoundary, delegate(float x)
		{
			this.BossCamNumBound.BottomBoundary = x;
		}, position.y - 1f, num2);
		int num4 = 100;
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(num2 / (float)num4);
		sequence.OnStepComplete(delegate
		{
			this.BossCamNumBound.SetBoundaries();
		});
		sequence.SetLoops(num4);
		sequence.OnComplete(delegate
		{
			this.BossCamNumBound.UseRightBoundary = false;
		});
		sequence.Play<Sequence>();
	}

	public void SetHWCamBounds()
	{
		Vector3 position = Core.Logic.Penitent.GetPosition();
		float num = this.BossCamNumBound.RightBoundary - this.BossCamNumBound.LeftBoundary;
		float num2 = 1f;
		DOTween.To(() => this.BossCamNumBound.LeftBoundary, delegate(float x)
		{
			this.BossCamNumBound.LeftBoundary = x;
		}, position.x - 2f, num2);
		DOTween.To(() => this.BossCamNumBound.RightBoundary, delegate(float x)
		{
			this.BossCamNumBound.RightBoundary = x;
		}, position.x - 2f + num, num2);
		int num3 = 100;
		Sequence sequence = DOTween.Sequence();
		sequence.AppendInterval(num2 / (float)num3);
		sequence.OnStepComplete(delegate
		{
			this.BossCamNumBound.SetBoundaries();
		});
		sequence.SetLoops(num3);
		sequence.Play<Sequence>();
	}

	public void ActivateScroll()
	{
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
		if (this._timeUntilNextModule <= 0f)
		{
			this._timeUntilNextModule = this.TimeToCycleNewModule;
			this.CycleNewModule();
		}
	}

	private void CycleNewModule()
	{
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
		this.Modules[this._nextModuleIndex].SetActive(true);
	}

	private void ResetAllPlatforms()
	{
		foreach (GameObject gameObject in this.Modules)
		{
			WaypointsMovingPlatform[] componentsInChildren = gameObject.GetComponentsInChildren<WaypointsMovingPlatform>();
			foreach (WaypointsMovingPlatform waypointsMovingPlatform in componentsInChildren)
			{
				waypointsMovingPlatform.ResetPlatform();
			}
			gameObject.SetActive(false);
		}
	}

	private void EnableAllPlatforms()
	{
		this.ToggleAllPlatforms(true);
	}

	private void DisableAllPlatforms()
	{
		this.ToggleAllPlatforms(false);
	}

	private void ToggleAllPlatforms(bool active)
	{
		foreach (GameObject gameObject in this.Modules)
		{
			WaypointsMovingPlatform[] componentsInChildren = gameObject.GetComponentsInChildren<WaypointsMovingPlatform>();
			foreach (WaypointsMovingPlatform waypointsMovingPlatform in componentsInChildren)
			{
				waypointsMovingPlatform.enabled = active;
			}
		}
	}

	private void UseStartingPlatforms()
	{
		GameObject gameObject = this.Modules[this.Modules.Count - 1];
		WaypointsMovingPlatform[] componentsInChildren = gameObject.GetComponentsInChildren<WaypointsMovingPlatform>();
		foreach (WaypointsMovingPlatform waypointsMovingPlatform in componentsInChildren)
		{
			waypointsMovingPlatform.Use();
		}
	}

	public PontiffHuskLevelScrollManager LevelScrollManager;

	public List<GameObject> Modules;

	public CameraNumericBoundaries NormalCamNumBound;

	public CameraNumericBoundaries BossCamNumBound;

	public List<GameObject> ScrollableItems;

	public float Speed = 1f;

	public float ModuleWidth = 29.6f;

	public float TimeToCycleFirstModuleAfterReset = 20f;

	public float TimeToCycleNewModule = 29.6f;

	private bool _modulesActive;

	private bool _scrollActive;

	private List<Vector2> _modulesPositions = new List<Vector2>();

	private List<Vector2> _scrollableItemsPositions = new List<Vector2>();

	private Vector2 _startingCamBoundaries;

	private int _nextModuleIndex;

	private float _timeUntilNextModule;
}
