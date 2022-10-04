using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Effects
{
	public struct LevelColorEffectData
	{
		[BoxGroup("Advanced Settings", true, false, 0)]
		[SerializeField]
		public Color colorizeColor;

		[BoxGroup("Advanced Settings", true, false, 0)]
		[SerializeField]
		[Range(0f, 1f)]
		public float colorizeAmount;

		[BoxGroup("Advanced Settings", true, false, 0)]
		[SerializeField]
		public Color colorizeMultColor;
	}
}
