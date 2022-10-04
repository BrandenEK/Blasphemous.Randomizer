using System;
using UnityEngine;

namespace Gameplay.GameControllers.Camera
{
	[ExecuteInEditMode]
	public class CRT : MonoBehaviour
	{
		private void Start()
		{
			Debug.Log(Shader.Find("Custom/CRT"));
			this.material = new Material(Shader.Find("Custom/CRT"));
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.material.SetTexture("_MainTex", source);
			Graphics.Blit(source, destination, this.material);
		}

		public Material material;
	}
}
