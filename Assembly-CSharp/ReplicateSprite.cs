using System;
using System.Collections.Generic;
using UnityEngine;

public class ReplicateSprite : MonoBehaviour
{
	private void LateUpdate()
	{
		foreach (SpriteRenderer spriteRenderer in this.childRenderers)
		{
			spriteRenderer.sprite = this.spriteRenderer.sprite;
		}
	}

	public SpriteRenderer spriteRenderer;

	public List<SpriteRenderer> childRenderers;
}
