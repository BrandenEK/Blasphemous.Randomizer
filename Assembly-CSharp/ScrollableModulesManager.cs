using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gameplay.GameControllers.Camera;
using UnityEngine;

public class ScrollableModulesManager : MonoBehaviour
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<float> OnUpdateHeight;

	public void UpdateTotalHeight(float totalHeight)
	{
		if (totalHeight - this._lastHeight > this.moduleHeight)
		{
			this.CycleNewModule();
			this._lastHeight = totalHeight;
		}
		if (totalHeight - this._lastBg > this.backgroundHeight)
		{
			this.CycleNewBG();
			this._lastBg = totalHeight;
		}
	}

	public void ActivateDeathCollider()
	{
		this.deathTrapCollider.SetActive(true);
	}

	private void LateUpdate()
	{
		if (!this.scrollActive)
		{
			return;
		}
		float num = Time.deltaTime * this.speed;
		this._totalHeight += num;
		foreach (GameObject gameObject in this.scrollableItems)
		{
			gameObject.transform.position += Vector3.up * num;
		}
		this.camNumBound.BottomBoundary += num;
		this.camNumBound.TopBoundary += num;
		this.camNumBound.SetBoundaries();
		if (this.OnUpdateHeight != null)
		{
			this.OnUpdateHeight(num);
		}
		this.UpdateTotalHeight(this._totalHeight);
	}

	private void CycleNewModule()
	{
		GameObject gameObject = this.PopModule();
		gameObject.transform.position = this.modules[this.modules.Count - 1].transform.position + Vector3.up * this.moduleHeight;
		this.PushModule(gameObject);
	}

	private void CycleNewBG()
	{
		GameObject gameObject = this.PopBG();
		gameObject.transform.position = this.backgrounds[0].transform.position + Vector3.up * this.backgroundHeight;
		this.PushBG(gameObject);
	}

	private GameObject PopBG()
	{
		GameObject gameObject = this.backgrounds[0];
		this.backgrounds.Remove(gameObject);
		return gameObject;
	}

	private void PushBG(GameObject go)
	{
		this.backgrounds.Add(go);
	}

	private GameObject PopModule()
	{
		GameObject gameObject = this.modules[0];
		this.modules.Remove(gameObject);
		return gameObject;
	}

	private void PushModule(GameObject go)
	{
		this.modules.Add(go);
	}

	public float speed = 1f;

	public float moduleHeight = 10f;

	public float backgroundHeight = 10f;

	public List<GameObject> backgrounds;

	public List<GameObject> modules;

	private float _totalHeight;

	public float _lastHeight;

	public float _lastBg;

	public CameraNumericBoundaries camNumBound;

	public List<GameObject> scrollableItems;

	public bool scrollActive;

	public GameObject deathTrapCollider;
}
