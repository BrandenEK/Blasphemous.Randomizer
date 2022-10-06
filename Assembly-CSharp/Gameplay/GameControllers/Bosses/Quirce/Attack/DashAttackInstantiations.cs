using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Attack
{
	[Serializable]
	public struct DashAttackInstantiations
	{
		[Range(0f, 1f)]
		public float dashMoment;

		[InlineEditor(4)]
		public GameObject prefabToInstantiate;

		public bool keepRotation;

		public Vector2 offset;
	}
}
