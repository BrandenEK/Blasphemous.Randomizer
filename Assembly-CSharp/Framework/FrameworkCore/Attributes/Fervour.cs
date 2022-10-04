using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class Fervour : VariableAttribute
	{
		public Fervour(float baseValue, float upgradeValue, float fervorMaxValue, float baseMultiplier) : base(baseValue, upgradeValue, fervorMaxValue, baseMultiplier)
		{
			base.Current = 0f;
		}

		public override bool CallArchivementWhenUpgrade()
		{
			return true;
		}
	}
}
