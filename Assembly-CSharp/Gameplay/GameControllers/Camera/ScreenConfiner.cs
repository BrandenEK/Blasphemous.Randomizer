using System;
using UnityEngine;

namespace Gameplay.GameControllers.Camera
{
	public class ScreenConfiner : MonoBehaviour
	{
		public void EnableBoundary()
		{
			Debug.Log("enable");
			if (this.levelLeftBoundary)
			{
				this.levelLeftBoundary.gameObject.GetComponent<Collider2D>().enabled = true;
			}
		}

		public void DisableBoundary()
		{
			if (this.levelLeftBoundary)
			{
				this.levelLeftBoundary.gameObject.GetComponent<Collider2D>().enabled = false;
			}
		}

		private void Start()
		{
			Vector2 vector;
			vector..ctor(this.cameraNumericBoundaries.LeftBoundary, this.cameraNumericBoundaries.TopBoundary / 2f);
			this.levelLeftBoundary = Object.Instantiate<GameObject>(this.levelLeftBoundaryPrefab, vector, Quaternion.identity);
			this.levelLeftBoundary.transform.SetParent(base.transform, true);
			this.DisableBoundary();
		}

		private void LateUpdate()
		{
			Vector2 vector;
			vector..ctor(this.cameraNumericBoundaries.LeftBoundary, this.cameraNumericBoundaries.TopBoundary / 2f);
			this.levelLeftBoundary.transform.position = vector;
		}

		[SerializeField]
		private CameraNumericBoundaries cameraNumericBoundaries;

		[SerializeField]
		private GameObject levelLeftBoundaryPrefab;

		private GameObject levelLeftBoundary;
	}
}
