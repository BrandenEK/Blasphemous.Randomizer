using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	[CreateAssetMenu(fileName = "PE01Balance", menuName = "Blasphemous/PE01 Balance", order = 0)]
	public class Pe01Balance : ScriptableObject
	{
		[BoxGroup("Mea Culpa Balance", true, false, 0)]
		[MinValue(0.10000000149011612)]
		public float normalStrengthMultiplier = 1f;

		[BoxGroup("Mea Culpa Balance", true, false, 0)]
		[MinValue(0.10000000149011612)]
		public float pe01StrengthMultiplier = 0.5f;

		[BoxGroup("Fervour Balance", true, false, 0)]
		public float timePerRegenerationTick = 3f;

		[BoxGroup("Fervour Balance", true, false, 0)]
		public float fervourRecoveryAmount = 10f;

		[BoxGroup("Fervour Balance", true, false, 0)]
		public float fervourLostAmount = 10f;
	}
}
