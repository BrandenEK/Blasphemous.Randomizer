using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.DataContainer
{
	[CreateAssetMenu(fileName = "GuiltConfig", menuName = "Blasphemous/Guilt")]
	public class GuiltConfigData : SerializedScriptableObject
	{
		public AnimationCurve fervourGainFactor;

		public AnimationCurve fervourMaxFactor;

		public AnimationCurve purgeGainFactor;

		public int maxDeathsDrops;

		public GameObject dropPrefab;

		public float MaxDistanceToLink;
	}
}
