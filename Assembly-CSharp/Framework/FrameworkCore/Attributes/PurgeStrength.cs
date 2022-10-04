using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class PurgeStrength : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public PurgeStrength(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
