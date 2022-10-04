using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class Life : VariableAttribute
	{
		public Life(float baseValue, float upgradeValue, float maxValue, float baseMultiplier) : base(baseValue, upgradeValue, maxValue, baseMultiplier)
		{
		}

		public float MissingRatio
		{
			get
			{
				return base.Current / this.Final;
			}
		}

		public override bool CallArchivementWhenUpgrade()
		{
			return true;
		}
	}
}
