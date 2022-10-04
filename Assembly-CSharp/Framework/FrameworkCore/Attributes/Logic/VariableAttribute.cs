using System;
using System.Diagnostics;
using Framework.Managers;
using UnityEngine;

namespace Framework.FrameworkCore.Attributes.Logic
{
	public class VariableAttribute : Attribute
	{
		public VariableAttribute(float baseValue, float upgradeValue, float maxValue, float baseMultiplier) : base(baseValue, upgradeValue, baseMultiplier)
		{
			this._maxValue = maxValue;
			this.Current = baseValue;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnChanged;

		public override bool IsVariable()
		{
			return true;
		}

		public void SetToCurrentMax()
		{
			this.Current = this.CurrentMax;
		}

		public float MaxValue
		{
			get
			{
				return this._maxValue;
			}
			set
			{
				this._maxValue = value;
				base.Base = value;
				this.CalculateValue();
			}
		}

		public float MaxFactor
		{
			get
			{
				return this._maxFactor;
			}
			set
			{
				this._maxFactor = value;
				this.CalculateValue();
			}
		}

		public float Current
		{
			get
			{
				this.CalculateValue();
				return this._currentValue;
			}
			set
			{
				float currentValue = this._currentValue;
				this._currentValue = value;
				this.CalculateValue();
				if (currentValue != this._currentValue && this.OnChanged != null)
				{
					this.OnChanged();
				}
			}
		}

		public float CurrentMax
		{
			get
			{
				return this.CurrentMaxWithoutFactor * this.MaxFactor;
			}
		}

		public float CurrentMaxWithoutFactor
		{
			get
			{
				return Mathf.Min(base.CalculateValue(), this.MaxValue);
			}
		}

		public new virtual float CalculateValue()
		{
			float currentMax = this.CurrentMax;
			if (this._currentValue > currentMax)
			{
				this._currentValue = currentMax;
			}
			return this._currentValue;
		}

		protected float _currentValue;

		private float _maxValue;

		private float _maxFactor = 1f;
	}
}
