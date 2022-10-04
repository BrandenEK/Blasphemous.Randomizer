using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using Tools.Level.Actionables;
using UnityEngine;

namespace Framework.Inventory
{
	[RequireComponent(typeof(BaseInventoryObject))]
	public class ObjectEffect : MonoBehaviour
	{
		private List<string> abilityNames
		{
			get
			{
				return (from myType in Assembly.GetExecutingAssembly().GetTypes()
				where myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Ability))
				select myType into t
				select t.Name).ToList<string>();
			}
		}

		public bool IsApplied { get; private set; }

		public void Awake()
		{
			this.IsApplied = false;
			this.IsWatingConditions = false;
			this.InvObj = base.GetComponent<BaseInventoryObject>();
			if (this.effectType == ObjectEffect.EffectType.OnEquip && !this.InvObj.IsEquipable())
			{
				this.ShowError("For OnEquip event you need an equipable object");
			}
			else if (this.effectType == ObjectEffect.EffectType.OnUse)
			{
				Prayer component = base.GetComponent<Prayer>();
				if (!component)
				{
					this.ShowError("Only Prayers can use OnUse type");
				}
			}
			this.OnAwake();
		}

		public void Start()
		{
			this.OnStart();
		}

		private void Update()
		{
			if (!this.IsApplied)
			{
				return;
			}
			if (this.effectType == ObjectEffect.EffectType.OnAbilityCast)
			{
				if (!this.ability)
				{
					return;
				}
				if (this.ability.IsUsingAbility && !this.abilityIsBeingCasted)
				{
					this.abilityIsBeingCasted = true;
					this.abilityWasCasted = true;
					if (this.TriggerOnlyOnce)
					{
						this.ApplyEffect();
					}
				}
				else if (!this.ability.IsUsingAbility && this.abilityIsBeingCasted)
				{
					this.abilityIsBeingCasted = false;
					if (this.TriggerOnlyOnce && !this.LimitTime)
					{
						this.RemoveEffect(false);
					}
				}
			}
			this.OnUpdate();
			if (this.timeLeft <= 0f)
			{
				return;
			}
			if (!this.UseWhenCastingPrayer && Core.Logic.Penitent.PrayerCast.IsUsingAbility)
			{
				return;
			}
			if (this.effectType == ObjectEffect.EffectType.OnAbilityCast && this.TriggerOnlyOnce && !this.LimitTime)
			{
				return;
			}
			if (this.effectType == ObjectEffect.EffectType.OnAbilityCast && !this.abilityWasCasted)
			{
				return;
			}
			if (this.CheckAnyStoppingCondition())
			{
				if (!this.IsTimeBasedEvent())
				{
					this.RemoveEffect(false);
				}
				if (this.effectType == ObjectEffect.EffectType.OnAbilityCast)
				{
					this.abilityWasCasted = false;
				}
			}
			else
			{
				this.timeLeft -= Time.deltaTime;
				if (this.timeLeft <= 0f)
				{
					this.timeLeft = 0f;
					if (this.IsTimeBasedEvent())
					{
						this.ApplyEffect();
					}
					else
					{
						this.RemoveEffect(false);
					}
				}
			}
		}

		private void OnDestroy()
		{
			this.OnDispose();
		}

		private void OnAddInventoryObject()
		{
			if (this.effectType == ObjectEffect.EffectType.OnAdquisition && !this.IsApplied)
			{
				this.ApplyEffect();
			}
			if (this.effectType == ObjectEffect.EffectType.OnInitialization)
			{
				this.ApplyEffect();
			}
			if (!this.InvObj.IsEquipable() && this.IsTimeBasedEvent())
			{
				this.ApplyEffect();
			}
		}

		private void OnRemoveInventoryObject()
		{
			if (this.IsApplied && this.effectType == ObjectEffect.EffectType.OnInitialization)
			{
				this.RemoveEffect(false);
			}
			if (!this.InvObj.IsEquipable() && this.IsTimeBasedEvent())
			{
				this.RemoveEffect(false);
			}
		}

		private void OnEquipInventoryObject()
		{
			if (this.effectType == ObjectEffect.EffectType.OnAbilityCast)
			{
				this.IsApplied = true;
				if (!this.TriggerOnlyOnce)
				{
					this.timeLeft = this.PingTime;
				}
				Type type = Type.GetType("Gameplay.GameControllers.Penitent.Abilities." + this.abilityName);
				this.ability = (Ability)Core.Logic.Penitent.GetComponentInChildren(type, true);
			}
			else if (this.effectType == ObjectEffect.EffectType.OnEquip || this.IsTimeBasedEvent())
			{
				this.ApplyEffect();
			}
		}

		private void OnUnEquipInventoryObject()
		{
			if (!this.IsApplied)
			{
				return;
			}
			if (this.effectType == ObjectEffect.EffectType.OnAbilityCast)
			{
				this.IsApplied = false;
				this.abilityWasCasted = false;
				this.timeLeft = 0f;
				if (!this.IsWatingConditions)
				{
					this.OnRemoveEffect();
				}
			}
			else if (this.effectType == ObjectEffect.EffectType.OnEquip || this.IsTimeBasedEvent())
			{
				this.RemoveEffect(false);
			}
		}

		private void OnUseInventoryObject()
		{
			if (this.effectType == ObjectEffect.EffectType.OnUse)
			{
				if (this.IsApplied)
				{
					this.RemoveEffect(false);
				}
				this.ApplyEffect();
			}
		}

		private void OnHitEnemy(Hit hit)
		{
			if (this.effectType == ObjectEffect.EffectType.OnHitEnemy && (Core.Logic.Penitent.PrayerCast.IsUsingAbility || !this.OnlyWhenUsingPrayer))
			{
				if (this.IsApplied)
				{
					this.RemoveEffect(false);
				}
				this.currentHit = hit;
				this.ApplyEffect();
			}
		}

		private void OnKillEnemy(Enemy e)
		{
			if (this.effectType == ObjectEffect.EffectType.OnKillEnemy && (Core.Logic.Penitent.PrayerCast.IsUsingAbility || !this.OnlyWhenUsingPrayer))
			{
				if (this.IsApplied && this.CheckAllApplyingConditions())
				{
					this.RemoveEffect(false);
				}
				this.ApplyEffect();
			}
		}

		private void OnHitReceived(Hit hit)
		{
			if (this.effectType == ObjectEffect.EffectType.OnHitReceived && (Core.Logic.Penitent.PrayerCast.IsUsingAbility || !this.OnlyWhenUsingPrayer))
			{
				if (this.IsApplied)
				{
					this.RemoveEffect(false);
				}
				this.currentHit = hit;
				this.ApplyEffect();
			}
		}

		private void OnPenitentHealthChanged(float life)
		{
			this.CheckNewEventForConditions();
		}

		private void OnNumberOfCurrentFlasksChanged(float numOfFlasks)
		{
			this.CheckNewEventForConditions();
		}

		private void OnBreakBreakable(BreakableObject breakable)
		{
			if (this.effectType == ObjectEffect.EffectType.OnBreakBreakable && (Core.Logic.Penitent.PrayerCast.IsUsingAbility || !this.OnlyWhenUsingPrayer))
			{
				if (this.IsApplied)
				{
					this.RemoveEffect(false);
				}
				this.breakableObject = breakable;
				this.ApplyEffect();
			}
		}

		private void OnPenitentDead()
		{
			if (this.effectType == ObjectEffect.EffectType.OnPenitentDead)
			{
				this.ApplyEffect();
			}
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual bool OnApplyEffect()
		{
			return false;
		}

		protected virtual void OnRemoveEffect()
		{
		}

		protected virtual void OnDispose()
		{
		}

		protected void ShowError(string error)
		{
			Log.Error("Item Effect", base.gameObject.name + ": " + error, null);
		}

		private void ApplyEffect()
		{
			if (this.IsApplied && this.effectType != ObjectEffect.EffectType.OnInitialization && this.effectType != ObjectEffect.EffectType.OnAbilityCast && !this.IsTimeBasedEvent() && this.CheckAllApplyingConditions())
			{
				this.ShowError("Applied without ending last, ignoring");
				return;
			}
			if (!this.IsApplied && this.effectType == ObjectEffect.EffectType.OnceOnTimer)
			{
				this.IsApplied = true;
				this.timeLeft = this.TimeToWait;
			}
			else
			{
				this.timeLeft = 0f;
				bool flag = true;
				if (this.CanBeTriggeredMultiple() && this.percentToExecute < 100)
				{
					flag = (UnityEngine.Random.Range(0, 100) <= this.percentToExecute);
				}
				if (flag)
				{
					if (this.CheckAllApplyingConditions())
					{
						this.IsWatingConditions = false;
						this.IsApplied = this.OnApplyEffect();
						if (this.IsApplied && this.ActivationFxSound != string.Empty)
						{
							this.PlayFxSound(this.ActivationFxSound);
						}
					}
					else
					{
						this.IsWatingConditions = true;
					}
				}
			}
			if (this.CanBeTriggeredMultiple() && !this.LimitTime)
			{
				this.IsApplied = false;
			}
			if (this.IsApplied && (this.effectType == ObjectEffect.EffectType.OnUpdate || !this.TriggerOnlyOnce))
			{
				this.timeLeft = this.PingTime;
			}
			else if (this.IsApplied && this.LimitTime)
			{
				this.timeLeft = ((!this.UsePrayerDurationAddition) ? this.EffectTime : (this.EffectTime + Core.Logic.Penitent.Stats.PrayerDurationAddition.Final));
			}
		}

		private void PlayFxSound(string eventId)
		{
			if (string.IsNullOrEmpty(eventId))
			{
				return;
			}
			Core.Audio.PlaySfx(eventId, 0f);
		}

		public void RemoveEffect(bool force = false)
		{
			if (!this.IsApplied && !this.IsWatingConditions && !force)
			{
				this.ShowError("Remove without apply");
				return;
			}
			if (this.effectType != ObjectEffect.EffectType.OnAbilityCast)
			{
				this.IsApplied = false;
			}
			this.timeLeft = 0f;
			if (force || !this.IsWatingConditions)
			{
				this.OnRemoveEffect();
			}
			this.IsWatingConditions = false;
		}

		private bool CheckAllApplyingConditions()
		{
			bool flag = true;
			foreach (ObjectEffect.Condition condition in this.Conditions)
			{
				switch (condition.type)
				{
				case ObjectEffect.ConditionType.WhenLifeUnderPercent:
				{
					float num = Core.Logic.Penitent.Stats.Life.Current / Core.Logic.Penitent.Stats.Life.CurrentMax * 100f;
					flag = (num <= condition.value);
					break;
				}
				case ObjectEffect.ConditionType.WhenExecutionDone:
					flag = Core.Logic.Penitent.IsOnExecution;
					break;
				case ObjectEffect.ConditionType.WhenHeavyAttackDone:
					flag = Core.Logic.Penitent.IsAttackCharged;
					break;
				case ObjectEffect.ConditionType.WhenDamageReceived:
					flag = (Core.Logic.Penitent.Stats.Life.Current < this.prevFrameHealth);
					this.prevFrameHealth = Core.Logic.Penitent.Stats.Life.Current;
					break;
				case ObjectEffect.ConditionType.WhenNoFlasksLeft:
					flag = (Core.Logic.Penitent.Stats.Flask.Current <= 0f);
					break;
				}
				if (!flag)
				{
					break;
				}
			}
			return flag;
		}

		private bool CheckAnyStoppingCondition()
		{
			bool flag = false;
			foreach (ObjectEffect.Condition condition in this.StoppingConditions)
			{
				switch (condition.type)
				{
				case ObjectEffect.ConditionType.WhenLifeUnderPercent:
				{
					float num = Core.Logic.Penitent.Stats.Life.Current / Core.Logic.Penitent.Stats.Life.CurrentMax * 100f;
					flag = (num <= condition.value);
					break;
				}
				case ObjectEffect.ConditionType.WhenExecutionDone:
					flag = Core.Logic.Penitent.IsOnExecution;
					break;
				case ObjectEffect.ConditionType.WhenHeavyAttackDone:
					flag = Core.Logic.Penitent.IsAttackCharged;
					break;
				case ObjectEffect.ConditionType.WhenDamageReceived:
					flag = (Core.Logic.Penitent.Stats.Life.Current < this.prevFrameHealth);
					this.prevFrameHealth = Core.Logic.Penitent.Stats.Life.Current;
					break;
				case ObjectEffect.ConditionType.WhenNoFlasksLeft:
					flag = (Core.Logic.Penitent.Stats.Flask.Current <= 0f);
					break;
				}
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		private void CheckNewEventForConditions()
		{
			if (this.IsApplied && !this.CheckAllApplyingConditions())
			{
				this.RemoveEffect(false);
				this.IsWatingConditions = true;
			}
			else if (this.IsWatingConditions && this.CheckAllApplyingConditions())
			{
				this.IsWatingConditions = false;
				this.IsApplied = this.OnApplyEffect();
			}
		}

		private bool VisibleAbilityNames()
		{
			return this.effectType == ObjectEffect.EffectType.OnAbilityCast;
		}

		private bool VisibleTimeLimit()
		{
			return !this.IsTimeBasedEvent() || this.effectType == ObjectEffect.EffectType.OnUse;
		}

		private bool VisibleContinuous()
		{
			return this.effectType == ObjectEffect.EffectType.OnUpdate || !this.TriggerOnlyOnce;
		}

		private bool NotVisibleContinuous()
		{
			return this.effectType != ObjectEffect.EffectType.OnUpdate;
		}

		private bool VisibleOnceOnTimer()
		{
			return this.effectType == ObjectEffect.EffectType.OnceOnTimer;
		}

		private bool HitPrayer()
		{
			return this.effectType == ObjectEffect.EffectType.OnHitReceived || this.effectType == ObjectEffect.EffectType.OnHitEnemy || this.effectType == ObjectEffect.EffectType.OnBreakBreakable || this.effectType == ObjectEffect.EffectType.OnKillEnemy;
		}

		private bool IsTimeBasedEvent()
		{
			return this.effectType == ObjectEffect.EffectType.OnUpdate || this.effectType == ObjectEffect.EffectType.OnceOnTimer || !this.TriggerOnlyOnce;
		}

		private bool CanBeTriggeredMultiple()
		{
			return this.effectType == ObjectEffect.EffectType.OnHitEnemy || this.effectType == ObjectEffect.EffectType.OnKillEnemy || this.effectType == ObjectEffect.EffectType.OnHitReceived || this.effectType == ObjectEffect.EffectType.OnBreakBreakable;
		}

		[BoxGroup("Type Dependant", true, false, 0)]
		public ObjectEffect.EffectType effectType;

		[BoxGroup("Type Dependant", true, false, 0)]
		[ValueDropdown("abilityNames")]
		[ShowIf("VisibleAbilityNames", true)]
		public string abilityName;

		private const string abilityNamePrefix = "Gameplay.GameControllers.Penitent.Abilities.";

		[BoxGroup("Type Dependant", true, false, 0)]
		[ShowIf("VisibleTimeLimit", true)]
		public bool LimitTime;

		[BoxGroup("Type Dependant", true, false, 0)]
		[ShowIf("LimitTime", true)]
		public float EffectTime;

		[BoxGroup("Type Dependant", true, false, 0)]
		[ShowIf("LimitTime", true)]
		public bool UsePrayerDurationAddition;

		[BoxGroup("Type Dependant", true, false, 0)]
		[ShowIf("NotVisibleContinuous", true)]
		public bool TriggerOnlyOnce = true;

		[BoxGroup("Type Dependant", true, false, 0)]
		[ShowIf("HitPrayer", true)]
		public bool OnlyWhenUsingPrayer;

		[BoxGroup("Type Dependant", true, false, 0)]
		[ShowIf("HitPrayer", true)]
		[MinValue(0.0)]
		[MaxValue(100.0)]
		public int percentToExecute = 100;

		[BoxGroup("Type Dependant", true, false, 0)]
		[ShowIf("VisibleContinuous", true)]
		public float PingTime = 0.4f;

		[BoxGroup("Type Dependant", true, false, 0)]
		[ShowIf("VisibleOnceOnTimer", true)]
		public float TimeToWait = 3f;

		[BoxGroup("Type Dependant", true, false, 0)]
		[ShowIf("VisibleContinuous", true)]
		public bool UseWhenCastingPrayer = true;

		[BoxGroup("Applying Conditions", true, false, 0)]
		public List<ObjectEffect.Condition> Conditions = new List<ObjectEffect.Condition>();

		[BoxGroup("Stopping Conditions", true, false, 0)]
		[ShowIf("VisibleContinuous", true)]
		public List<ObjectEffect.Condition> StoppingConditions = new List<ObjectEffect.Condition>();

		[BoxGroup("SFX", true, false, 0)]
		[EventRef]
		public string ActivationFxSound = string.Empty;

		protected BaseInventoryObject InvObj;

		protected bool IsWatingConditions;

		protected Hit currentHit;

		protected BreakableObject breakableObject;

		private float timeLeft;

		private Ability ability;

		private bool abilityIsBeingCasted;

		private bool abilityWasCasted;

		private float prevFrameHealth;

		public enum EffectType
		{
			OnEquip,
			OnUse,
			OnHitEnemy,
			OnInitialization,
			OnUpdate,
			OnHitReceived,
			OnceOnTimer,
			OnBreakBreakable,
			OnKillEnemy,
			OnAdquisition,
			OnPenitentDead,
			OnAbilityCast
		}

		public enum ConditionType
		{
			WhenLifeUnderPercent,
			WhenExecutionDone,
			WhenHeavyAttackDone,
			WhenDamageReceived,
			WhenNoFlasksLeft
		}

		[Serializable]
		public class Condition
		{
			public ObjectEffect.ConditionType type;

			[ShowIf("type", ObjectEffect.ConditionType.WhenLifeUnderPercent, true)]
			public float value;
		}
	}
}
