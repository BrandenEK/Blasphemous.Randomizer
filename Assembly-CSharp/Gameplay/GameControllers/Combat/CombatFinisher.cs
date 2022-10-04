using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Entities;
using Tools.Level;
using UnityEngine;

namespace Gameplay.GameControllers.Combat
{
	[RequireComponent(typeof(Collider2D))]
	public class CombatFinisher : MonoBehaviour
	{
		private void Start()
		{
			base.InvokeRepeating("Refresh", 0f, this.refreshRate);
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (col.CompareTag("NPC"))
			{
				Entity componentInParent = col.GetComponentInParent<Entity>();
				if (componentInParent != null && !this.enemies.Contains(componentInParent))
				{
					this.enemies.Add(componentInParent);
				}
			}
		}

		private void Refresh()
		{
			this.RemoveDeadEnemies();
			if (this.enemies.Count == 0)
			{
				this.FinishLogic();
				base.CancelInvoke();
			}
		}

		private void RemoveDeadEnemies()
		{
			for (int i = 0; i < this.enemies.Count; i++)
			{
				if (this.enemies != null && this.enemies[i].Dead)
				{
					this.enemies.Remove(this.enemies[i]);
				}
			}
		}

		private void FinishLogic()
		{
			for (int i = 0; i < this.activate.Length; i++)
			{
				if (!(this.activate[i] == null))
				{
					IActionable[] components = this.activate[i].GetComponents<IActionable>();
					Array.ForEach<IActionable>(components, delegate(IActionable actionable)
					{
						actionable.Use();
					});
				}
			}
		}

		private List<Entity> enemies = new List<Entity>();

		[SerializeField]
		[Range(0f, 2f)]
		private float refreshRate = 0.5f;

		[SerializeField]
		private GameObject[] activate = new GameObject[0];
	}
}
