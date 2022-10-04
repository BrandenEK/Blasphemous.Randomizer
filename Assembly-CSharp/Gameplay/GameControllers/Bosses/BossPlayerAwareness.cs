using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses
{
	public class BossPlayerAwareness : MonoBehaviour
	{
		private void Start()
		{
			this.results = new Collider2D[1];
		}

		private void Update()
		{
			this.CheckAwarenessAreas();
		}

		private void CheckAwarenessAreas()
		{
			foreach (BossAwarenessArea a in this.awarenessAreas)
			{
				this.UpdateArea(a);
			}
		}

		public bool AreaContainsPlayer(string id)
		{
			return this.awarenessAreas.Find((BossAwarenessArea x) => x.id == id).containsPlayer;
		}

		private void UpdateArea(BossAwarenessArea a)
		{
			a.containsPlayer = (a.area.OverlapCollider(this.playerFilter, this.results) > 0);
		}

		public List<BossAwarenessArea> awarenessAreas;

		public ContactFilter2D playerFilter;

		private Collider2D[] results;
	}
}
