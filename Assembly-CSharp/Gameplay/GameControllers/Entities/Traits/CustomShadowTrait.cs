using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Traits
{
	public class CustomShadowTrait : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.GenerateShadow();
		}

		private void GenerateShadow()
		{
			GameObject gameObject = new GameObject("Shadow");
			this._spriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
			this.results = new RaycastHit2D[1];
		}

		protected override void OnLateUpdate()
		{
			base.OnLateUpdate();
			if (this.showShadow)
			{
				this._spriteRenderer.enabled = true;
				Vector2 vector = base.transform.position + this.castPoint;
				int num = Physics2D.LinecastNonAlloc(vector, vector + Vector2.down * this.raycastDistance, this.results, this.groundLayerMask);
				if (num > 0)
				{
					Debug.DrawLine(vector, vector + Vector2.down * this.raycastDistance, Color.green);
					Vector2 point = this.results[0].point;
					this._spriteRenderer.transform.position = point + this.shadowOffset;
					this._spriteRenderer.transform.up = this.results[0].normal;
				}
				else
				{
					this._spriteRenderer.enabled = false;
					Debug.DrawLine(vector, vector + Vector2.down * this.raycastDistance, Color.red);
				}
				return;
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.black;
			Vector2 vector = base.transform.position + this.castPoint;
			Vector2 vector2 = vector + Vector2.down * this.raycastDistance;
			Gizmos.DrawWireSphere(vector, 0.2f);
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawWireCube(vector2, Vector2.one);
		}

		public bool showShadow = true;

		public LayerMask groundLayerMask;

		public Vector2 castPoint;

		public Vector2 shadowOffset;

		public float raycastDistance;

		private SpriteRenderer _spriteRenderer;

		private RaycastHit2D[] results;
	}
}
