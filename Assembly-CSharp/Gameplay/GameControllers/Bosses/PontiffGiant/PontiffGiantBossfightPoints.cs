using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffGiant
{
	public class PontiffGiantBossfightPoints : MonoBehaviour
	{
		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform leftLimitTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform rightLimitTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform fightCenterTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public List<Transform> magicPoints;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public List<Transform> beamPoints;
	}
}
