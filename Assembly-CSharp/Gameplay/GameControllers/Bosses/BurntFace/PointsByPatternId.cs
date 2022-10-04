using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	[Serializable]
	public struct PointsByPatternId
	{
		public string id;

		public List<Transform> points;
	}
}
