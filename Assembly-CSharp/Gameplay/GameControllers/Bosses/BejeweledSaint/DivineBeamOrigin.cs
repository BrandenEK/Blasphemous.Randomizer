using System;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class DivineBeamOrigin : MonoBehaviour
	{
		public Vector2 OriginPosition { get; private set; }

		public BoxCollider2D Collider2D { get; private set; }

		private void Start()
		{
			this.Collider2D = base.GetComponent<BoxCollider2D>();
			this.OriginPosition = new Vector2(this.Collider2D.bounds.center.x, this.Collider2D.bounds.min.y);
		}
	}
}
