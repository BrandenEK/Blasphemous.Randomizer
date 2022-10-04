using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class Resistance : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public Resistance(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
