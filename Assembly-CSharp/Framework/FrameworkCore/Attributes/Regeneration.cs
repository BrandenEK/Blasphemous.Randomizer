using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class Regeneration : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public Regeneration(float baseValue, float upgradeValue, float baseMultiplier = 0f) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
