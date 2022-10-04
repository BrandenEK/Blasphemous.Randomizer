using System;
using System.Diagnostics;
using Framework.Managers;
using UnityEngine;

namespace Tools.Level
{
	[RequireComponent(typeof(Collider2D))]
	public class Region : MonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.RegionEvent OnRegionEnter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.RegionEvent OnRegionExit;

		private void Start()
		{
			this.regionCollider = base.GetComponent<Collider2D>();
			this.regionCollider.isTrigger = true;
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (Region.OnRegionEnter != null)
			{
				Region.OnRegionEnter(this);
			}
		}

		private void OnTriggerExit2D(Collider2D col)
		{
			if (Region.OnRegionExit != null)
			{
				Region.OnRegionExit(this);
			}
		}

		public int EntitiesInside { get; private set; }

		private Collider2D regionCollider;
	}
}
