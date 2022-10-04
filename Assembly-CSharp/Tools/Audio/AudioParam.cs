using System;
using Sirenix.OdinInspector;

namespace Tools.Audio
{
	[Serializable]
	public struct AudioParam
	{
		public string name;

		[PropertyRange(0.0, 1.0)]
		public float targetValue;

		[ReadOnly]
		public float currentValue;
	}
}
