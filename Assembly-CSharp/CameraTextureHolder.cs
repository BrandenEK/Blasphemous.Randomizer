using System;
using UnityEngine;

public class CameraTextureHolder : MonoBehaviour
{
	private void OnEnable()
	{
		if (this._renderTexture && !this._renderTexture.IsCreated())
		{
			this._renderTexture.Create();
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		int num = Mathf.RoundToInt((float)src.height * this.distanceFactor);
		int num2 = Mathf.RoundToInt((float)src.height * (1f - this.distanceFactor));
		if (this._renderTexture)
		{
			Graphics.CopyTexture(src, 0, 0, 0, num, src.width, num2, this._renderTexture, 0, 0, 0, 0);
		}
		Graphics.Blit(src, dest);
	}

	public RenderTexture _renderTexture;

	public float distanceFactor = 0.5f;
}
