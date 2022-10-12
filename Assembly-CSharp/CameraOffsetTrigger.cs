using System;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class CameraOffsetTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (ProCamera2D.Instance != null && (this.triggerMask.value & 1 << collision.gameObject.layer) > 0)
		{
			this.influenceOn = true;
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (this.influenceOn)
		{
			return;
		}
		if (ProCamera2D.Instance != null && (this.triggerMask.value & 1 << collision.gameObject.layer) > 0)
		{
			this.influenceOn = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (ProCamera2D.Instance != null && (this.triggerMask.value & 1 << collision.gameObject.layer) > 0)
		{
			this.influenceOn = false;
		}
	}

	private void Update()
	{
		if (this.influenceOn)
		{
			ProCamera2D.Instance.ApplyInfluence(this.offset);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = this.gizmoColor;
		Vector3 size = new Vector3(20f, 11f, 0f);
		Gizmos.DrawWireCube(base.transform.position + this.offset, size);
		Gizmos.DrawIcon(base.transform.position + new Vector3(0f, 2f, 0f), "Blasphemous/TPO_singleImage.png", true);
	}

	public Vector2 offset;

	public LayerMask triggerMask;

	private bool influenceOn;

	public Color gizmoColor = Color.cyan;
}
