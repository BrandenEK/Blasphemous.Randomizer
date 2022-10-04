using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class Flask : VariableAttribute
	{
		public Flask(float baseValue, float upgradeValue, float maxValue, float baseMultiplier) : base(baseValue, upgradeValue, maxValue, baseMultiplier)
		{
		}
	}
}
