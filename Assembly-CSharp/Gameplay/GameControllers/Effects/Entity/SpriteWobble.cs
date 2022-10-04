using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Entity
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteWobble : Ability
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			if (this.SpriteRenderer == null)
			{
				Debug.LogError("This component needs a sprite renderer component");
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._currentTimeWobbling -= Time.deltaTime;
			if (this._currentTimeWobbling > 0f)
			{
				this.Wobbling();
			}
			else if (base.Casting)
			{
				base.StopCast();
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			this._currentTimeWobbling = this.TimeWobbling;
			this._startPosition = new Vector3(base.transform.position.x, base.transform.position.y);
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			base.transform.position = this._startPosition;
		}

		private void Wobbling()
		{
			Vector3 position = base.transform.position;
			position.x += Mathf.Sin(Time.time * this.Speed) * Time.deltaTime * this.Amplitude;
			base.transform.position = position;
		}

		[SerializeField]
		protected SpriteRenderer SpriteRenderer;

		public float Amplitude = 1.75f;

		public float Speed = 40f;

		public float TimeWobbling;

		private float _currentTimeWobbling;

		private Vector3 _startPosition;
	}
}
