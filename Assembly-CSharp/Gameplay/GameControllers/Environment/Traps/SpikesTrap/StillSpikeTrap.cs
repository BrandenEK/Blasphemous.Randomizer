using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.SpikesTrap
{
	[RequireComponent(typeof(Collider2D))]
	public class StillSpikeTrap : MonoBehaviour
	{
		private void Awake()
		{
			this.TrapCollider = base.GetComponent<Collider2D>();
			this.contacts = new Collider2D[5];
		}

		private void Start()
		{
			this.SetNoSafePositionToOverlappedCollider();
		}

		private void SetNoSafePositionToOverlappedCollider()
		{
			int num = this.TrapCollider.OverlapCollider(this.overlapFilter, this.contacts);
			if (num <= 0)
			{
				return;
			}
			foreach (Collider2D collider2D in this.contacts)
			{
				if (collider2D)
				{
					collider2D.gameObject.AddComponent<NoSafePosition>();
				}
			}
		}

		private Collider2D TrapCollider;

		[FoldoutGroup("Overlap fixer settings", 0)]
		public ContactFilter2D overlapFilter;

		[FoldoutGroup("Overlap fixer settings", 0)]
		public Collider2D[] contacts;
	}
}
