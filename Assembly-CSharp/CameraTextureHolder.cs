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
		int srcY = Mathf.RoundToInt((float)src.height * this.distanceFactor);
		int srcHeight = Mathf.RoundToInt((float)src.height * (1f - this.distanceFactor));
		if (this._renderTexture)
		{
			Graphics.CopyTexture(src, 0, 0, 0, srcY, src.width, srcHeight, this._renderTexture, 0, 0, 0, 0);
		}
		Graphics.Blit(src, dest);
	}

	public RenderTexture _renderTexture;

	public float distanceFactor = 0.5f;
}
