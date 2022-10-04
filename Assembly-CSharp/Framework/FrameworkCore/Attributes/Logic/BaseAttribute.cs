using System;

namespace Framework.FrameworkCore.Attributes.Logic
{
	public class BaseAttribute
	{
		public BaseAttribute(float baseValue, float baseMultiplier = 1f)
		{
			this.Base = baseValue;
			this.Multiplier = baseMultiplier;
		}

		public float Base { get; protected set; }

		public float Multiplier { get; protected set; }
	}
}
