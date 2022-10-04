using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class AttackSpeed : VariableAttribute
	{
		public AttackSpeed(float baseValue, float upgradeValue, float maxValue, float baseMultiplier) : base(baseValue, upgradeValue, maxValue, baseMultiplier)
		{
		}
	}
}
