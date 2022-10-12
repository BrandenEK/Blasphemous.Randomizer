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
			int nameID = Shader.PropertyToID("_NoiseOffset");
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			component.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat(nameID, UnityEngine.Random.value);
			component.SetPropertyBlock(materialPropertyBlock);
		}
	}
}
