using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Audio
{
	public class SpatialModifier : AudioTool
	{
		private const float DOOR_TRANSITION_TIME = 1.5f;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[Range(0f, 30f)]
		private float transitionDistance = 5f;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private AudioParamInitialized[] parameters = new AudioParamInitialized[0];

		private SpatialModifier.SpatialModifierMode currentMode;

		private float[] initialValues;

		private static SpatialModifier previousActive;

		private static SpatialModifier currentlyActive;

		private static DOTween currentTimeTransition;

		private enum SpatialModifierMode
		{
			None,
			In,
			Out
		}
	}
}
