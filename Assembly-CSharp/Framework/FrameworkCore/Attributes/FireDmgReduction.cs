using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class FireDmgReduction : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public FireDmgReduction(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
