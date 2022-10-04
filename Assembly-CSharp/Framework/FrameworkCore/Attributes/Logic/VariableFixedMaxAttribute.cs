using System;

namespace Framework.FrameworkCore.Attributes.Logic
{
	public class VariableFixedMaxAttribute : VariableAttribute
	{
		public VariableFixedMaxAttribute(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, float.MaxValue, baseMultiplier)
		{
			base.MaxValue = float.MaxValue;
			base.Current = baseValue;
		}
	}
}
