using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class NormalDmgReduction : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public NormalDmgReduction(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
