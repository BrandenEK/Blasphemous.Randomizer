using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Audio
{
	[RequireComponent(typeof(Collider2D))]
	public class DangerEmitter : AudioTool
	{
		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private bool combatStartEvent = true;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[BoxGroup("Debug Information", true, false, 0)]
		[ReadOnly]
		private int enemiesDetected;
	}
}
