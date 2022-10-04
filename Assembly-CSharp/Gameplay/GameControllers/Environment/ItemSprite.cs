using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	public class ItemSprite : MonoBehaviour
	{
		private void Start()
		{
			this.rb = base.GetComponentInParent<Rigidbody2D>();
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
			this.touched = false;
			this.spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
			this.trigger = this.rb.GetComponent<Collider2D>();
			this.trigger.enabled = false;
		}

		private void OnCollisionStay2D(Collision2D col)
		{
			if (col.gameObject.layer == LayerMask.NameToLayer("Floor") || col.gameObject.layer == LayerMask.NameToLayer("OneWayDown"))
			{
				this.touched = true;
			}
		}

		private void Update()
		{
			if (this.show && this.spriteRenderer.color.a < 1f)
			{
				this.spriteRenderer.color = Color.Lerp(this.spriteRenderer.color, new Color(1f, 1f, 1f, 1f), Time.deltaTime * 2f);
			}
			else if (!this.trigger.enabled)
			{
				this.trigger.enabled = true;
			}
			if (this.touched && this.rb.velocity.x == 0f && this.rb.velocity.y == 0f)
			{
				this.show = true;
			}
		}

		public void Hide()
		{
			this.touched = false;
		}

		private Collider2D trigger;

		private SpriteRenderer spriteRenderer;

		private bool touched;

		private bool show;

		private Rigidbody2D rb;
	}
}
