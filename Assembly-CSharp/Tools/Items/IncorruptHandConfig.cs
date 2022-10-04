using System;
using Sirenix.OdinInspector;

namespace Tools.Items
{
	[Serializable]
	public struct IncorruptHandConfig
	{
		[HorizontalGroup(0f, 0, 0, 0)]
		[PropertyRange(0.0, 1.0)]
		public float haloTransparency;

		[HorizontalGroup(0f, 0, 0, 0)]
		[PropertyRange(0.0, 2.0)]
		public float haloDuration;
	}
}
