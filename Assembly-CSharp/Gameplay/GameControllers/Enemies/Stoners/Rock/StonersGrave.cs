using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Stoners.Rock
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class StonersGrave : MonoBehaviour
	{
		private void Start()
		{
			this._spriteRenderer = base.GetComponent<SpriteRenderer>();
		}

		private void Update()
		{
			if (!this._spriteRenderer.isVisible)
			{
				this._timeInvisible += Time.deltaTime;
				if (this._timeInvisible >= 2f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			else
			{
				this._timeInvisible = 0f;
			}
		}

		private SpriteRenderer _spriteRenderer;

		private float _timeInvisible;
	}
}
