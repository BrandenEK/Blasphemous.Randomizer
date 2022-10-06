using System;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomSprite : MonoBehaviour
{
	public void OnEnable()
	{
		this.sprRenderer.sprite = this.sprites[Random.Range(0, this.sprites.Count)];
	}

	public List<Sprite> sprites;

	public SpriteRenderer sprRenderer;
}
