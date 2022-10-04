using System;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses
{
	[Serializable]
	public class BossAwarenessArea
	{
		public string id;

		public Collider2D area;

		public bool containsPlayer;
	}
}
