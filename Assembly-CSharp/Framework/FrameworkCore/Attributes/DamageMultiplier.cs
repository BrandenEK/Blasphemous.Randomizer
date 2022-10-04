using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class DamageMultiplier : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public DamageMultiplier(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
