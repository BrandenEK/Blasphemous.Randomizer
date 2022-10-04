using System;
using System.Collections;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Pietat.PietatAnimations
{
	public class PietatAnimations : MonoBehaviour
	{
		private void Awake()
		{
			this._pietat = base.GetComponent<Pietat>();
		}

		private void Start()
		{
			this._cameraPlayerOffset = Core.Logic.CameraManager.CameraPlayerOffset;
		}

		public void PietatIsAttacking()
		{
			if (this._pietat == null)
			{
				return;
			}
			if (!this._pietat.IsAttacking)
			{
				this._pietat.IsAttacking = true;
			}
		}

		public void CenterCamera()
		{
		}

		private IEnumerator CenterCameraCoroutine()
		{
			float currentCameraXOffset = this._cameraPlayerOffset.XOffset;
			while (currentCameraXOffset <= this.cameraXOffset)
			{
				currentCameraXOffset += 0.05f;
				this._cameraPlayerOffset.XOffset = currentCameraXOffset;
				yield return new WaitForSeconds(0.02f);
			}
			yield break;
		}

		private CameraPlayerOffset _cameraPlayerOffset;

		private Pietat _pietat;

		public float cameraXOffset = 5f;
	}
}
