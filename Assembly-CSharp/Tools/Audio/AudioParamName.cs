using System;
using Sirenix.OdinInspector;

namespace Tools.Audio
{
	[Serializable]
	public struct AudioParamName
	{
		public string name;

		[ReadOnly]
		public float currentValue;
	}
}
