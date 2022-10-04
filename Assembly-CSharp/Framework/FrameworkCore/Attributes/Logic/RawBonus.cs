using System;

namespace Framework.FrameworkCore.Attributes.Logic
{
	public class RawBonus : BaseAttribute
	{
		public RawBonus(float baseValue, float baseMultiplier = 1f) : base(baseValue, baseMultiplier)
		{
		}
	}
}
