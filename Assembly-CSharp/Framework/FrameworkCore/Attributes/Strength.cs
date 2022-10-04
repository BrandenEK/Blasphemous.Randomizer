using System;
using Framework.FrameworkCore.Attributes.Logic;

namespace Framework.FrameworkCore.Attributes
{
	public class Strength : Framework.FrameworkCore.Attributes.Logic.Attribute
	{
		public Strength(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
		}

		public float FinalStrengthMultiplier
		{
			get
			{
				return this.finalStrengthMultiplier;
			}
			set
			{
				this.finalStrengthMultiplier = value;
			}
		}

		public override float Final
		{
			get
			{
				return base.Final * this.FinalStrengthMultiplier;
			}
		}

		private float finalStrengthMultiplier = 1f;
	}
}
