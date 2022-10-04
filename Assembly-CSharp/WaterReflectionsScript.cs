using System;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using UnityEngine;

public class WaterReflectionsScript : MonoBehaviour
{
	private void Awake()
	{
		this.mat = base.GetComponent<MeshRenderer>().material;
		LevelManager.OnGenericsElementsLoaded += this.OnGenericElementsLoaded;
	}

	private void OnGenericElementsLoaded()
	{
		LevelManager.OnGenericsElementsLoaded -= this.OnGenericElementsLoaded;
		CameraManager.Instance.TextureHolder.enabled = true;
	}

	private void Update()
	{
		Vector3 position = CameraManager.Instance.transform.position;
		base.transform.position = new Vector3(position.x, base.transform.position.y, base.transform.position.z);
		float num = Mathf.Abs(position.y - base.transform.position.y);
		float distanceFactor = Mathf.Lerp(this.baseDistanceFactor, this.maxDistanceFactor, num / this.maxcameraDistance);
		CameraManager.Instance.TextureHolder.distanceFactor = distanceFactor;
	}

	private Material mat;

	public float baseDistanceFactor = 0.2f;

	public float maxDistanceFactor = 1f;

	public float maxcameraDistance = 1f;
}
