using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	public class DeathFall : MonoBehaviour
	{
		private void Start()
		{
		}

		private void OnDrawGizmos()
		{
		}

		public const string TRAP_LAYER = "Trap";

		public const string SPIKE_TRAP_TAG = "SpikeTrap";

		public const string ABYSS_TRAP_TAG = "AbyssTrap";

		public const string UNNAVOIDABLE_TRAP_TAG = "UnnavoidableTrap";

		public const string HW_TRAP_TAG = "HWTrap";
	}
}
