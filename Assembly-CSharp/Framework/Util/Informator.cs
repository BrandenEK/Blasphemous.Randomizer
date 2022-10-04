using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace Framework.Util
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class Informator : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.CompareTag("Penitent") || this.disabled)
			{
				return;
			}
			Analytics.CustomEvent("TRIGGER_ENTER", new Dictionary<string, object>
			{
				{
					"SCENE",
					SceneManager.GetActiveScene().name
				},
				{
					"SPAWN_POINT",
					base.gameObject.name
				}
			});
			this.disabled = true;
			Debug.Log("[Analytics] Trigger enter \"" + base.gameObject.name + "\" has been submited.");
		}

		[SerializeField]
		private bool disabled;
	}
}
