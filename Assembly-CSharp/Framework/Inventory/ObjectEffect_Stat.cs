using System;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes.Logic;
using Framework.Managers;
using Framework.Penitences;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Inventory
{
	public class ObjectEffect_Stat : ObjectEffect
	{
		private bool CheckOnHit()
		{
			return this.effectType == ObjectEffect.EffectType.OnHitEnemy || this.effectType == ObjectEffect.EffectType.OnHitReceived;
		}

		private bool IsValueType()
		{
			return this.valueType == ObjectEffect_Stat.ValueType.Value;
		}

		protected override void OnAwake()
		{
			if (this.effectType == ObjectEffect.EffectType.OnUse && !this.LimitTime && this.effectMode == ObjectEffect_Stat.EffectMode.Bonus)
			{
				base.ShowError("OnUse effect bonus without limittime");
			}
			if (this.effectType == ObjectEffect.EffectType.OnHitEnemy && !this.LimitTime && this.effectMode == ObjectEffect_Stat.EffectMode.Bonus)
			{
				base.ShowError("OnHitEnemy effect bonus without limittime");
			}
			if (this.effectType == ObjectEffect.EffectType.OnKillEnemy && !this.LimitTime && this.effectMode == ObjectEffect_Stat.EffectMode.Bonus)
			{
				base.ShowError("OnKillEnemy effect bonus without limittime");
			}
			if (this.effectType == ObjectEffect.EffectType.OnHitReceived && !this.LimitTime && this.effectMode == ObjectEffect_Stat.EffectMode.Bonus)
			{
				base.ShowError("OnHitReceived effect bonus without limittime");
			}
			if (this.effectType == ObjectEffect.EffectType.OnBreakBreakable && !this.LimitTime && this.effectMode == ObjectEffect_Stat.EffectMode.Bonus)
			{
				base.ShowError("OnBreakBreakable effect bonus without limittime");
			}
		}

		protected override bool OnApplyEffect()
		{
			if (this.AddsLife() && Core.PenitenceManager.GetCurrentPenitence() is PenitencePE02)
			{
				this.AddTemporalRegenFactor();
				return true;
			}
			EntityStats stats = Core.Logic.Penitent.Stats;
			Framework.FrameworkCore.Attributes.Logic.Attribute byType = stats.GetByType(this.statType);
			this.currentValue = this.value;
			if (this.CheckOnHit() && this.UseHitAsBaseValue)
			{
				this.currentValue += this.currentHit.DamageAmount;
				Debug.Log(string.Concat(new object[]
				{
					"**** PRAYER WITH HIT Damage:",
					this.currentHit.DamageAmount.ToString(),
					"  Final base:",
					this.currentValue.ToString(),
					"  Multiplier:",
					this.multiplier
				}));
			}
			if (this.valueType == ObjectEffect_Stat.ValueType.BasedOnCurrentStat || this.valueType == ObjectEffect_Stat.ValueType.BasedOnMaxStat)
			{
				VariableAttribute variableAttribute = (VariableAttribute)stats.GetByType(this.statValueType);
				if (variableAttribute != null)
				{
					if (this.valueType == ObjectEffect_Stat.ValueType.BasedOnCurrentStat)
					{
						this.currentValue += variableAttribute.Current;
					}
					else
					{
						this.currentValue += variableAttribute.CurrentMax;
					}
				}
				else
				{
					base.ShowError("Variable attr " + this.statValueType.ToString() + " not found when BasedOnStat");
				}
			}
			if (this.effectMode == ObjectEffect_Stat.EffectMode.Bonus)
			{
				this.bonus = new RawBonus(this.currentValue, this.multiplier);
				byType.AddRawBonus(this.bonus);
			}
			else
			{
				this.currentValue *= this.multiplier;
				if (byType.IsVariable())
				{
					VariableAttribute variableAttribute2 = (VariableAttribute)byType;
					if (variableAttribute2 != null)
					{
						variableAttribute2.Current += this.currentValue;
					}
				}
				else
				{
					base.ShowError("Effect setted as Current needs a variable stat, STAT:" + this.statType.ToString() + "  OBJECT:" + base.gameObject.name);
				}
			}
			return true;
		}

		protected override void OnRemoveEffect()
		{
			EntityStats stats = Core.Logic.Penitent.Stats;
			Framework.FrameworkCore.Attributes.Logic.Attribute byType = stats.GetByType(this.statType);
			if (this.effectMode == ObjectEffect_Stat.EffectMode.Bonus)
			{
				byType.RemoveRawBonus(this.bonus);
			}
			else
			{
				VariableAttribute variableAttribute = (VariableAttribute)byType;
				variableAttribute.Current -= this.currentValue;
			}
		}

		private bool AddsLife()
		{
			return this.statType == EntityStats.StatsTypes.Life && this.effectMode == ObjectEffect_Stat.EffectMode.Current && this.value > 1f;
		}

		private void AddTemporalRegenFactor()
		{
			Core.PenitenceManager.AddFlasksPassiveHealthRegen(this.value * 0.1f);
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			Core.PenitenceManager.AddFlasksPassiveHealthRegen(this.value * -0.1f);
		}

		[BoxGroup("Bonus", true, false, 0)]
		[ShowIf("CheckOnHit", true)]
		public bool UseHitAsBaseValue;

		[BoxGroup("Bonus", true, false, 0)]
		public ObjectEffect_Stat.EffectMode effectMode;

		[BoxGroup("Bonus", true, false, 0)]
		public EntityStats.StatsTypes statType;

		[BoxGroup("Bonus", true, false, 0)]
		public ObjectEffect_Stat.ValueType valueType;

		[HideIf("IsValueType", true)]
		[BoxGroup("Bonus", true, false, 0)]
		public EntityStats.StatsTypes statValueType;

		[BoxGroup("Bonus", true, false, 0)]
		public float value;

		[BoxGroup("Bonus", true, false, 0)]
		public float multiplier = 1f;

		private RawBonus bonus;

		private float currentValue;

		public enum EffectMode
		{
			Bonus,
			Current
		}

		public enum ValueType
		{
			Value,
			BasedOnCurrentStat,
			BasedOnMaxStat
		}
	}
}
