using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class MagicDmgReduction : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public MagicDmgReduction(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
