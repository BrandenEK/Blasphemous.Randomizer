using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class MeaCulpa : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public MeaCulpa(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}

		public override bool CallArchivementWhenUpgrade()
		{
			return true;
		}
	}
}
