using System;
using Gameplay.GameControllers.Environment;
using UnityEngine;

public class ClimbDeactivatorCollider : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Climbing"))
		{
			CliffLede component = collision.gameObject.GetComponent<CliffLede>();
			if (component != null)
			{
				component.isClimbable = false;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Climbing"))
		{
			CliffLede component = collision.gameObject.GetComponent<CliffLede>();
			if (component != null)
			{
				component.isClimbable = true;
			}
		}
	}
}
