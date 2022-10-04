using System;
using UnityEngine;

[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour
{
	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, this.material);
	}

	public Material material;
}
