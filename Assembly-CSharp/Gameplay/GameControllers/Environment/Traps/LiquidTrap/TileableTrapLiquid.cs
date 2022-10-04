using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.LiquidTrap
{
	public class TileableTrapLiquid : MonoBehaviour
	{
		[FoldoutGroup("References", 0)]
		public Animator bodyAnimator;

		[FoldoutGroup("References", 0)]
		public GameObject liquidBody;

		[FoldoutGroup("References", 0)]
		public Transform origin;

		[FoldoutGroup("Animation settings", 0)]
		public float fallDelay = 0.3f;
	}
}
