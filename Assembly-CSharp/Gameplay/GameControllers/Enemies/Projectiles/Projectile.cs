using System;
using System.Diagnostics;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class Projectile : Entity
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Projectile> OnLifeEndedEvent;

		public void ResetTTL()
		{
			this._currentTTL = this.timeToLive;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.UpdateOrientation();
			base.transform.Translate(this.velocity * Time.deltaTime, 0);
			this._currentTTL -= Time.deltaTime;
			if (this._currentTTL < 0f)
			{
				this.OnLifeEnded();
			}
		}

		protected void UpdateOrientation()
		{
			if (this.useOrientation)
			{
				EntityOrientation orientation = (this.velocity.x <= 0f) ? EntityOrientation.Left : EntityOrientation.Right;
				this.SetOrientation(orientation, this.flipRenderer, false);
			}
		}

		protected void OnLifeEnded()
		{
			if (this.OnLifeEndedEvent != null)
			{
				this.OnLifeEndedEvent(this);
			}
		}

		public Entity owner;

		public Vector2 velocity;

		public bool flipRenderer;

		public bool useOrientation;

		public float timeToLive = 10f;

		protected float _currentTTL;
	}
}
