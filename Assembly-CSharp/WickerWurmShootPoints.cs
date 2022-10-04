using System;
using Gameplay.GameControllers.Bosses.BlindBaby;
using UnityEngine;

public class WickerWurmShootPoints : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		WickerWurmBehaviour component = collision.GetComponent<WickerWurmBehaviour>();
		if (component != null)
		{
			component.OnShootPointTouched();
		}
	}
}
