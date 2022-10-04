using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Framework.Managers;

namespace Framework.FrameworkCore.Attributes.Logic
{
	public class Attribute : BaseAttribute
	{
		public Attribute(float baseValue, float upgradeValue, float baseMultiplier = 1f) : base(baseValue, baseMultiplier)
		{
			this._rawBonuses = new List<RawBonus>();
			this._finalBonuses = new List<FinalBonus>();
			this._finalValue = baseValue;
			this._bonusValue = 0f;
			this.PermanetBonus = 0f;
			this._initialValue = baseValue;
			this._upgradeValue = upgradeValue;
		}

		public float PermanetBonus { get; private set; }

		public virtual bool IsVariable()
		{
			return false;
		}

		public virtual bool CallArchivementWhenUpgrade()
		{
			return false;
		}

		public void ResetBonus()
		{
			this._rawBonuses.Clear();
			this._finalBonuses.Clear();
		}

		public void Upgrade()
		{
			this.PermanetBonus += this._upgradeValue;
			if (this.CallArchivementWhenUpgrade())
			{
				Core.AchievementsManager.CheckProgressToAC46();
			}
		}

		public int GetUpgrades()
		{
			return (int)(this.PermanetBonus / this._upgradeValue);
		}

		public void ResetUpgrades()
		{
			this.PermanetBonus = 0f;
		}

		public void SetPermanentBonus(float value)
		{
			this.PermanetBonus = value;
		}

		public void AddRawBonus(RawBonus bonus)
		{
			this._rawBonuses.Add(bonus);
		}

		public void AddFinalBonus(FinalBonus bonus)
		{
			this._finalBonuses.Add(bonus);
		}

		public void RemoveRawBonus(RawBonus bonus)
		{
			if (this._rawBonuses.Contains(bonus))
			{
				this._rawBonuses.Remove(bonus);
			}
		}

		public void RemoveFinalBonus(FinalBonus bonus)
		{
			if (this._finalBonuses.Contains(bonus))
			{
				this._finalBonuses.Remove(bonus);
			}
		}

		protected void ApplyRawBonuses()
		{
			float num = 0f;
			float num2 = 1f;
			for (int i = 0; i < this._rawBonuses.Count; i++)
			{
				num += this._rawBonuses[i].Base;
				num2 *= this._rawBonuses[i].Multiplier;
			}
			this._bonusValue = num;
			this._finalValue += num;
			this._finalValue *= num2;
		}

		protected void ApplyFinalBonuses()
		{
			float num = 0f;
			float num2 = 1f;
			for (int i = 0; i < this._finalBonuses.Count; i++)
			{
				num += this._finalBonuses[i].Base;
				num2 *= this._finalBonuses[i].Multiplier;
			}
			this._bonusValue += num;
			this._finalValue += num;
			this._finalValue *= num2;
		}

		public ReadOnlyCollection<RawBonus> GetRawBonus()
		{
			return this._rawBonuses.AsReadOnly();
		}

		public float CalculateValue()
		{
			this._finalValue = base.Base;
			this.ApplyRawBonuses();
			this.ApplyFinalBonuses();
			this._bonusValue += this.PermanetBonus;
			this._finalValue += this.PermanetBonus;
			return this._finalValue;
		}

		public virtual float Final
		{
			get
			{
				return this.CalculateValue();
			}
		}

		public float Bonus
		{
			get
			{
				this.CalculateValue();
				return this._bonusValue;
			}
			set
			{
				this._bonusValue = value;
			}
		}

		public void ConsoleSet(float newValue)
		{
			base.Base = newValue;
		}

		private List<RawBonus> _rawBonuses;

		private List<FinalBonus> _finalBonuses;

		protected float _bonusValue;

		protected float _finalValue;

		protected float _initialValue;

		protected float _upgradeValue;
	}
}
