using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Environment.MovingPlatforms;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[HutongGames.PlayMaker.Tooltip("Resets All Waypoint Platforms.")]
	public class ResetAllWaypointPlatforms : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.platforms.Count == 0)
			{
				this.platforms = new List<WaypointsMovingPlatform>(UnityEngine.Object.FindObjectsOfType<WaypointsMovingPlatform>());
			}
			foreach (WaypointsMovingPlatform waypointsMovingPlatform in this.platforms)
			{
				waypointsMovingPlatform.ResetPlatform();
			}
			base.Finish();
		}

		private List<WaypointsMovingPlatform> platforms = new List<WaypointsMovingPlatform>();
	}
}
