using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class FlaskHealth : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public FlaskHealth(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
