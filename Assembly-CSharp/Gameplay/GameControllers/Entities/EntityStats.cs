using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes;
using Framework.FrameworkCore.Attributes.Logic;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[Serializable]
	public class EntityStats : PersistentInterface
	{
		public AttackSpeed AttackSpeed { get; set; }

		public Agility Agility { get; set; }

		public Defense Defense { get; set; }

		public Fervour Fervour { get; set; }

		public FervourStrength FervourStrength { get; set; }

		public Life Life { get; set; }

		public Resistance Resistance { get; set; }

		public Strength Strength { get; set; }

		public RangedStrength RangedStrength { get; set; }

		public Speed Speed { get; set; }

		public Flask Flask { get; set; }

		public FlaskHealth FlaskHealth { get; set; }

		public BeadSlots BeadSlots { get; set; }

		public CriticalChance CriticalChance { get; set; }

		public CriticalMultiplier CriticalMultiplier { get; set; }

		public DamageMultiplier DamageMultiplier { get; set; }

		public Purge Purge { get; set; }

		public MeaCulpa MeaCulpa { get; set; }

		public PurgeStrength PurgeStrength { get; set; }

		public ParryWindow ParryWindow { get; set; }

		public ActiveRiposteWindow ActiveRiposteWindow { get; set; }

		public DashCooldown DashCooldown { get; set; }

		public DashRide DashRide { get; set; }

		public FireDmgReduction FireDmgReduction { get; set; }

		public ToxicDmgReduction ToxicDmgReduction { get; set; }

		public MagicDmgReduction MagicDmgReduction { get; set; }

		public LightningDmgReduction LightningDmgReduction { get; set; }

		public ContactDmgReduction ContactDmgReduction { get; set; }

		public NormalDmgReduction NormalDmgReduction { get; set; }

		public Framework.FrameworkCore.Attributes.Logic.Attribute PrayerDurationAddition { get; set; }

		public Framework.FrameworkCore.Attributes.Logic.Attribute PrayerStrengthMultiplier { get; set; }

		public Framework.FrameworkCore.Attributes.Logic.Attribute PrayerCostAddition { get; set; }

		public Framework.FrameworkCore.Attributes.Logic.Attribute AirImpulses { get; set; }

		public void Initialize()
		{
			this.AttackSpeed = new AttackSpeed(this.AttackSpeedBase, this.AttackSpeedUpgrade, this.AttackSpeedMaximum, 1f);
			this.Life = new Life(this.LifeBase, this.LifeUpgrade, this.LifeMaximun, 1f);
			this.Fervour = new Fervour(this.FervorBase, this.FervorUpgrade, this.FervorMaxValue, 1f);
			this.FervourStrength = new FervourStrength(this.FervourStrengthBase, this.FervourStrengthUpgrade, 1f);
			this.Agility = new Agility(this.AgilityBase, this.AgilityUpgrade, 1f);
			this.Defense = new Defense(this.DefenseBase, this.DefenseUpgrade, 1f);
			this.Resistance = new Resistance(this.ResistanceBase, this.ResistanceUpgrade, 1f);
			this.Strength = new Strength(this.StrengthBase, this.StrengthUpgrade, 1f);
			this.Speed = new Speed(this.SpeedBase, this.SpeedUpgrade, 1f);
			this.Flask = new Flask(this.FlaskBase, this.FlaskUpgrade, this.FlaskMaximun, 1f);
			this.BeadSlots = new BeadSlots(this.BeadSlotsBase, this.BeadSlotsUpgrade, 1f);
			this.CriticalChance = new CriticalChance(this.CriticalChanceBase, this.CriticalChanceUpgrade, 1f);
			this.CriticalMultiplier = new CriticalMultiplier(this.CriticalMultiplierBase, this.CriticalMultiplierUpgrade, 1f);
			this.DamageMultiplier = new DamageMultiplier(this.DamageMultiplierBase, this.DamageMultiplierUpgrade, 1f);
			this.FlaskHealth = new FlaskHealth(this.FlaskHealthBase, this.FlaskHealthUpgrade, 1f);
			this.Purge = new Purge(this.InitialPurge, this.PurgeUpgrade, 1f);
			this.MeaCulpa = new MeaCulpa(this.InitialMeaCulpa, this.MeaCulpaUpgrade, 1f);
			this.PurgeStrength = new PurgeStrength(this.PurgeStrengthBase, this.PurgeStrengthUpgrade, 1f);
			this.ParryWindow = new ParryWindow(this.ParryWindowBase, this.ParryWindowUpgrade, 1f);
			this.ActiveRiposteWindow = new ActiveRiposteWindow(this.RiposteWindowBase, this.RiposteWindowUpgrade, 1f);
			this.DashCooldown = new DashCooldown(this.DashCooldownBase, this.DashCooldownUpgrade, 1f);
			this.DashRide = new DashRide(this.DashRideBase, this.DashRideUpgrade, 1f);
			this.FireDmgReduction = new FireDmgReduction(this.FireDmgReductionBase, this.FireDmgReductionUpgrade, 1f);
			this.ToxicDmgReduction = new ToxicDmgReduction(this.ToxicDmgReductionBase, this.ToxicDmgReductionUpgrade, 1f);
			this.MagicDmgReduction = new MagicDmgReduction(this.MagicDmgReductionBase, this.MagicDmgReductionUpgrade, 1f);
			this.LightningDmgReduction = new LightningDmgReduction(this.LightningDmgReductionBase, this.LightningDmgReductionUpgrade, 1f);
			this.ContactDmgReduction = new ContactDmgReduction(this.ContactDmgReductionBase, this.ContactDmgReductionUpgrade, 1f);
			this.NormalDmgReduction = new NormalDmgReduction(this.NormalDmgReductionBase, this.NormalDmgReductionUpgrade, 1f);
			this.RangedStrength = new RangedStrength(this.RangedStrengthBase, this.RangedStrengthUpgrade, 1f);
			this.PrayerDurationAddition = new Framework.FrameworkCore.Attributes.Logic.Attribute(0f, 1f, 1f);
			this.PrayerStrengthMultiplier = new Framework.FrameworkCore.Attributes.Logic.Attribute(1f, 1f, 1f);
			this.PrayerCostAddition = new Framework.FrameworkCore.Attributes.Logic.Attribute(0f, 1f, 1f);
			this.AirImpulses = new AirImpulses(this.AirImpulsesBase, this.AirImpulsesUpgrade, 1f);
		}

		public Framework.FrameworkCore.Attributes.Logic.Attribute GetByType(EntityStats.StatsTypes nameType)
		{
			Framework.FrameworkCore.Attributes.Logic.Attribute result = null;
			switch (nameType)
			{
			case EntityStats.StatsTypes.AttackSpeed:
				result = this.AttackSpeed;
				break;
			case EntityStats.StatsTypes.Agility:
				result = this.Agility;
				break;
			case EntityStats.StatsTypes.Defense:
				result = this.Defense;
				break;
			case EntityStats.StatsTypes.DashCooldown:
				result = this.DashCooldown;
				break;
			case EntityStats.StatsTypes.Fervour:
				result = this.Fervour;
				break;
			case EntityStats.StatsTypes.Life:
				result = this.Life;
				break;
			case EntityStats.StatsTypes.DashRide:
				result = this.DashRide;
				break;
			case EntityStats.StatsTypes.Resistance:
				result = this.Resistance;
				break;
			case EntityStats.StatsTypes.Strength:
				result = this.Strength;
				break;
			case EntityStats.StatsTypes.Speed:
				result = this.Speed;
				break;
			case EntityStats.StatsTypes.FervourStrength:
				result = this.FervourStrength;
				break;
			case EntityStats.StatsTypes.Flask:
				result = this.Flask;
				break;
			case EntityStats.StatsTypes.BeadSlots:
				result = this.BeadSlots;
				break;
			case EntityStats.StatsTypes.CriticalChance:
				result = this.CriticalChance;
				break;
			case EntityStats.StatsTypes.CriticalMultiplier:
				result = this.CriticalMultiplier;
				break;
			case EntityStats.StatsTypes.DamageMultiplier:
				result = this.DamageMultiplier;
				break;
			case EntityStats.StatsTypes.FlaskHealth:
				result = this.FlaskHealth;
				break;
			case EntityStats.StatsTypes.Purge:
				result = this.Purge;
				break;
			case EntityStats.StatsTypes.MeaCulpa:
				result = this.MeaCulpa;
				break;
			case EntityStats.StatsTypes.PurgeStrength:
				result = this.PurgeStrength;
				break;
			case EntityStats.StatsTypes.ParryWindow:
				result = this.ParryWindow;
				break;
			case EntityStats.StatsTypes.FireDmgReduction:
				result = this.FireDmgReduction;
				break;
			case EntityStats.StatsTypes.ToxicDmgReduction:
				result = this.ToxicDmgReduction;
				break;
			case EntityStats.StatsTypes.MagicDmgReduction:
				result = this.MagicDmgReduction;
				break;
			case EntityStats.StatsTypes.LightningDmgReduction:
				result = this.LightningDmgReduction;
				break;
			case EntityStats.StatsTypes.RangedStrength:
				result = this.RangedStrength;
				break;
			case EntityStats.StatsTypes.PrayerDurationAddition:
				result = this.PrayerDurationAddition;
				break;
			case EntityStats.StatsTypes.PrayerStrengthMultiplier:
				result = this.PrayerStrengthMultiplier;
				break;
			case EntityStats.StatsTypes.PrayerCostAddition:
				result = this.PrayerCostAddition;
				break;
			case EntityStats.StatsTypes.ContactDmgReduction:
				result = this.ContactDmgReduction;
				break;
			case EntityStats.StatsTypes.ActiveRiposteWindow:
				result = this.ActiveRiposteWindow;
				break;
			case EntityStats.StatsTypes.AirImpulses:
				result = this.AirImpulses;
				break;
			case EntityStats.StatsTypes.NormalDmgReduction:
				result = this.NormalDmgReduction;
				break;
			}
			return result;
		}

		public void ResetAllBonus()
		{
			IEnumerator enumerator = Enum.GetValues(typeof(EntityStats.StatsTypes)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					EntityStats.StatsTypes nameType = (EntityStats.StatsTypes)obj;
					Framework.FrameworkCore.Attributes.Logic.Attribute byType = this.GetByType(nameType);
					byType.ResetBonus();
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public string GetPersistenID()
		{
			return "ID_STATS";
		}

		public int GetOrder()
		{
			return 0;
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			EntityStats.StatsPersistenceData statsPersistenceData = new EntityStats.StatsPersistenceData();
			IEnumerator enumerator = Enum.GetValues(typeof(EntityStats.StatsTypes)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					EntityStats.StatsTypes statsTypes = (EntityStats.StatsTypes)obj;
					Framework.FrameworkCore.Attributes.Logic.Attribute byType = this.GetByType(statsTypes);
					statsPersistenceData.permanetBonus[statsTypes] = byType.PermanetBonus;
					if (byType.IsVariable())
					{
						VariableAttribute variableAttribute = (VariableAttribute)byType;
						statsPersistenceData.currentValues[statsTypes] = variableAttribute.Current;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return statsPersistenceData;
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			EntityStats.StatsPersistenceData statsPersistenceData = (EntityStats.StatsPersistenceData)data;
			foreach (KeyValuePair<EntityStats.StatsTypes, float> keyValuePair in statsPersistenceData.permanetBonus)
			{
				Framework.FrameworkCore.Attributes.Logic.Attribute byType = this.GetByType(keyValuePair.Key);
				byType.SetPermanentBonus(keyValuePair.Value);
			}
			foreach (KeyValuePair<EntityStats.StatsTypes, float> keyValuePair2 in statsPersistenceData.currentValues)
			{
				Framework.FrameworkCore.Attributes.Logic.Attribute byType2 = this.GetByType(keyValuePair2.Key);
				if (byType2.IsVariable())
				{
					VariableAttribute variableAttribute = (VariableAttribute)byType2;
					variableAttribute.Current = keyValuePair2.Value;
				}
			}
			if (isloading)
			{
				this.Fervour.Current = 0f;
				this.Life.SetToCurrentMax();
				this.Flask.SetToCurrentMax();
			}
		}

		public void ResetPersistence()
		{
			IEnumerator enumerator = Enum.GetValues(typeof(EntityStats.StatsTypes)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					EntityStats.StatsTypes nameType = (EntityStats.StatsTypes)obj;
					Framework.FrameworkCore.Attributes.Logic.Attribute byType = this.GetByType(nameType);
					byType.SetPermanentBonus(0f);
					if (byType.IsVariable())
					{
						VariableAttribute variableAttribute = (VariableAttribute)byType;
						variableAttribute.Current = 0f;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.Life.SetToCurrentMax();
			this.Flask.SetToCurrentMax();
		}

		[SerializeField]
		private float AttackSpeedBase = 1f;

		[SerializeField]
		private float AttackSpeedMaximum = 1.5f;

		[SerializeField]
		private float AttackSpeedUpgrade = 1f;

		[SerializeField]
		public float LifeBase = 100f;

		[SerializeField]
		public float LifeMaximun = 500f;

		[SerializeField]
		public float LifeUpgrade = 20f;

		[SerializeField]
		private float FervorBase = 40f;

		[SerializeField]
		private float FervorMaxValue = 120f;

		[SerializeField]
		private float FervorUpgrade = 20f;

		[SerializeField]
		private float FervourStrengthBase = 2f;

		[SerializeField]
		private float FervourStrengthUpgrade = 1f;

		[SerializeField]
		private float RangedStrengthBase = 1f;

		[SerializeField]
		private float RangedStrengthUpgrade = 1f;

		[SerializeField]
		private float AgilityBase = 10f;

		[SerializeField]
		private float AgilityUpgrade = 1f;

		[SerializeField]
		private float DefenseBase;

		[SerializeField]
		private float DefenseUpgrade = 1f;

		[SerializeField]
		private float ResistanceBase = 10f;

		[SerializeField]
		private float ResistanceUpgrade = 1f;

		[SerializeField]
		public float StrengthBase = 10f;

		[SerializeField]
		public float StrengthUpgrade = 1f;

		[SerializeField]
		private float SpeedBase = 10f;

		[SerializeField]
		private float SpeedUpgrade = 1f;

		[SerializeField]
		private float FlaskBase = 2f;

		[SerializeField]
		private float FlaskMaximun = 10f;

		[SerializeField]
		private float FlaskUpgrade = 1f;

		[SerializeField]
		private float FlaskHealthBase = 20f;

		[SerializeField]
		public float FlaskHealthUpgrade = 10f;

		[SerializeField]
		private float BeadSlotsBase = 2f;

		[SerializeField]
		private float BeadSlotsUpgrade = 1f;

		[SerializeField]
		private float CriticalChanceBase;

		[SerializeField]
		private float CriticalChanceUpgrade = 1f;

		[SerializeField]
		private float CriticalMultiplierBase = 2f;

		[SerializeField]
		private float CriticalMultiplierUpgrade = 1f;

		[SerializeField]
		private float DamageMultiplierBase = 1f;

		[SerializeField]
		private float DamageMultiplierUpgrade = 1f;

		[SerializeField]
		private float InitialPurge;

		[SerializeField]
		private float PurgeUpgrade = 1f;

		[SerializeField]
		private float InitialMeaCulpa;

		[SerializeField]
		private float MeaCulpaUpgrade = 1f;

		[SerializeField]
		private float PurgeStrengthBase = 1f;

		[SerializeField]
		private float PurgeStrengthUpgrade = 1f;

		[SerializeField]
		private float ParryWindowBase = 0.5f;

		[SerializeField]
		private float ParryWindowUpgrade = 1f;

		[SerializeField]
		private float RiposteWindowBase = 0.15f;

		[SerializeField]
		private float RiposteWindowUpgrade = 1f;

		[SerializeField]
		private float DashCooldownBase = 1f;

		[SerializeField]
		private float DashCooldownUpgrade = 1f;

		[SerializeField]
		private float DashRideBase = 0.35f;

		[SerializeField]
		private float DashRideUpgrade = 1f;

		[SerializeField]
		private float FireDmgReductionBase;

		[SerializeField]
		private float FireDmgReductionUpgrade = 0.25f;

		[SerializeField]
		private float ToxicDmgReductionBase;

		[SerializeField]
		private float ToxicDmgReductionUpgrade = 0.25f;

		[SerializeField]
		private float MagicDmgReductionBase;

		[SerializeField]
		private float MagicDmgReductionUpgrade = 0.25f;

		[SerializeField]
		private float LightningDmgReductionBase;

		[SerializeField]
		private float LightningDmgReductionUpgrade = 0.25f;

		[SerializeField]
		private float ContactDmgReductionBase;

		[SerializeField]
		private float ContactDmgReductionUpgrade = 0.4f;

		[SerializeField]
		private float NormalDmgReductionBase;

		[SerializeField]
		private float NormalDmgReductionUpgrade = 0.4f;

		[SerializeField]
		private float AirImpulsesBase = 2f;

		[SerializeField]
		private float AirImpulsesUpgrade = 1f;

		private const string PERSITENT_ID = "ID_STATS";

		public enum StatsTypes
		{
			AttackSpeed,
			Agility,
			Defense,
			DashCooldown,
			Fervour,
			Life,
			DashRide,
			Resistance,
			Strength,
			Speed,
			FervourStrength,
			Flask,
			BeadSlots,
			CriticalChance,
			CriticalMultiplier,
			DamageMultiplier,
			FlaskHealth,
			Purge,
			MeaCulpa,
			PurgeStrength,
			ParryWindow,
			FireDmgReduction,
			ToxicDmgReduction,
			MagicDmgReduction,
			LightningDmgReduction,
			RangedStrength,
			PrayerDurationAddition,
			PrayerStrengthMultiplier,
			PrayerCostAddition,
			ContactDmgReduction,
			ActiveRiposteWindow,
			AirImpulses,
			NormalDmgReduction
		}

		public class StatsPersistenceData : PersistentManager.PersistentData
		{
			public StatsPersistenceData() : base("ID_STATS")
			{
			}

			public Dictionary<EntityStats.StatsTypes, float> currentValues = new Dictionary<EntityStats.StatsTypes, float>();

			public Dictionary<EntityStats.StatsTypes, float> permanetBonus = new Dictionary<EntityStats.StatsTypes, float>();
		}
	}
}
