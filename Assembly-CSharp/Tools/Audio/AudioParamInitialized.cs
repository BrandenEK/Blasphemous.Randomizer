using System;
using Sirenix.OdinInspector;

namespace Tools.Audio
{
	[Serializable]
	public struct AudioParamInitialized
	{
		public string name;

		[PropertyRange(0.0, 1.0)]
		public float enterValue;

		[PropertyRange(0.0, 1.0)]
		public float exitValue;

		[ReadOnly]
		public float currentValue;
	}
}
