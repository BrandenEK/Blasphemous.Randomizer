using System;
using System.Collections.Generic;

namespace Framework.FrameworkCore.Attributes.Logic
{
	public class DependantAttribute : Attribute
	{
		public DependantAttribute(float baseValue, float upgradeValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
			this._otherAttributes = new List<Attribute>();
		}

		public void AddAttribute(Attribute attribute)
		{
			this._otherAttributes.Add(attribute);
		}

		public void RemoveAttribute(Attribute attribute)
		{
			if (this._otherAttributes.Contains(attribute))
			{
				this._otherAttributes.Remove(attribute);
			}
		}

		public new virtual float CalculateValue()
		{
			this._finalValue = base.Base;
			base.ApplyRawBonuses();
			base.ApplyFinalBonuses();
			return this._finalValue;
		}

		protected List<Attribute> _otherAttributes;
	}
}
