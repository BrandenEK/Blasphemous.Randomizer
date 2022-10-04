using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class ToxicDmgReduction : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public ToxicDmgReduction(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
