using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class ActiveRiposteWindow : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public ActiveRiposteWindow(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}
	}
}
