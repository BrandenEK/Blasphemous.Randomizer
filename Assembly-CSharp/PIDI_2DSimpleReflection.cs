using System;
using UnityEngine;

[ExecuteInEditMode]
public class PIDI_2DSimpleReflection : MonoBehaviour
{
	private void OnWillRenderObject()
	{
		if (base.GetComponent<SpriteRenderer>().sharedMaterial.HasProperty("_SurfaceLevel"))
		{
			base.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat("_SurfaceLevel", this.SurfaceLevel);
		}
	}

	[Range(-5f, 5f)]
	public float SurfaceLevel;
}
