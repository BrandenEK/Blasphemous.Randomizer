using System;
using System.Collections;
using UnityEngine;

namespace Framework.Managers
{
	public class ShockwaveManager : MonoBehaviour
	{
		private void Start()
		{
			this.shockWaveMaterial.SetFloat("_Radius", -0.2f);
		}

		public void Shockwave(Vector3 worldpos, float maxSeconds, float minScreenRadius, float maxScreenRadius)
		{
			this.SetCenter(worldpos);
			base.StopAllCoroutines();
			base.StartCoroutine(this.ShockWaveEffect(maxSeconds, minScreenRadius, maxScreenRadius));
		}

		private void SetCenter(Vector3 worldpos)
		{
			Camera gameCamera = Core.Logic.CameraManager.ProCamera2D.GameCamera;
			this.lastWorldPos = worldpos;
			Vector2 vector = gameCamera.WorldToViewportPoint(worldpos);
			this.shockWaveMaterial.SetFloat("_CenterX", vector.x);
			this.shockWaveMaterial.SetFloat("_CenterY", vector.y);
		}

		private IEnumerator ShockWaveEffect(float seconds, float minRadius, float radius)
		{
			float counter = 0f;
			while (counter < seconds)
			{
				counter += Time.deltaTime;
				float v = this.curve.Evaluate(counter / seconds);
				float waveRadius = Mathf.Lerp(minRadius, radius, v);
				this.shockWaveMaterial.SetFloat("_Radius", waveRadius);
				this.SetCenter(this.lastWorldPos);
				yield return null;
			}
			this.shockWaveMaterial.SetFloat("_Radius", -0.2f);
			yield break;
		}

		public float maxRadius = 1f;

		public AnimationCurve curve;

		public Material shockWaveMaterial;

		private Vector3 lastWorldPos;
	}
}
