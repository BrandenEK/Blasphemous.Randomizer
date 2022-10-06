using System;
using UnityEngine;

namespace VisualEffects.Scripts
{
	[RequireComponent(typeof(SpriteRenderer))]
	[ExecuteInEditMode]
	public class FakeLightRandomNoiseOffset : MonoBehaviour
	{
		private void OnEnable()
		{
			SpriteRenderer component = base.GetComponent<SpriteRenderer>();
			int num = Shader.PropertyToID("_NoiseOffset");
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			component.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat(num, Random.value);
			component.SetPropertyBlock(materialPropertyBlock);
		}
	}
}
