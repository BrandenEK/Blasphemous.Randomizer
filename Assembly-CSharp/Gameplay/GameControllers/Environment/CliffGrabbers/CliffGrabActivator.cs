using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.CliffGrabbers
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class CliffGrabActivator : MonoBehaviour
	{
		private Vector2 RightBoundary { get; set; }

		private Vector2 LeftBoundary { get; set; }

		private BoxCollider2D BoxCollider { get; set; }

		private void Awake()
		{
			this.BoxCollider = base.GetComponentInChildren<BoxCollider2D>();
			this.RightBoundary = new Vector2(this.BoxCollider.bounds.max.x + this.OffsetDetection, this.BoxCollider.bounds.min.y);
			this.LeftBoundary = new Vector2(this.BoxCollider.bounds.min.x - this.OffsetDetection, this.BoxCollider.bounds.min.y);
		}

		private void Update()
		{
			if (!this._player)
			{
				this._player = Core.Logic.Penitent;
			}
			if (!this._player || this._player.IsGrabbingCliffLede)
			{
				return;
			}
			bool flag = this.CheckRayCastHitsPlatform();
			this.BoxCollider.enabled = !flag;
		}

		private bool CheckRayCastHitsPlatform()
		{
			RaycastHit2D hit = Physics2D.Raycast(this.RightBoundary, -Vector2.up, this.VerticalRayDistance, this.NearPlatform);
			RaycastHit2D hit2 = Physics2D.Raycast(this.LeftBoundary, -Vector2.up, this.VerticalRayDistance, this.NearPlatform);
			return hit || hit2;
		}

		public LayerMask NearPlatform;

		public float OffsetDetection = 0.5f;

		public float VerticalRayDistance = 4.25f;

		private Penitent _player;
	}
}
