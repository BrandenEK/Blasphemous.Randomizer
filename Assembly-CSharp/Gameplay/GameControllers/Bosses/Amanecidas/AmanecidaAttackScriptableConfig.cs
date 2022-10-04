using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Amanecidas
{
	[CreateAssetMenu(menuName = "Blasphemous/Amanecidas/AmanecidaAttackConfig", fileName = "AmanecidaAttacksConfig")]
	public class AmanecidaAttackScriptableConfig : SerializedScriptableObject
	{
		public static List<AmanecidasBehaviour.AMANECIDA_ATTACKS> GetAttackIds(AmanecidaAttackScriptableConfig.AmanecidaAttacksByWeapon atks, bool onlyActive = true)
		{
			List<AmanecidasBehaviour.AMANECIDA_ATTACKS> list = new List<AmanecidasBehaviour.AMANECIDA_ATTACKS>();
			foreach (AmanecidaAttackScriptableConfig.AmanecidaAttackConfig amanecidaAttackConfig in atks.attacks)
			{
				if (onlyActive && amanecidaAttackConfig.active)
				{
					list.Add(amanecidaAttackConfig.attackID);
				}
			}
			return list;
		}

		private static bool IsActiveInHpSection(AmanecidaAttackScriptableConfig.AmanecidaAttackConfig atk, float hpPercentage)
		{
			if (hpPercentage < 0.33f)
			{
				return atk.activeThirdPart;
			}
			if (hpPercentage < 0.66f)
			{
				return atk.activeSecondThird;
			}
			return atk.activeFirstThird;
		}

		public static List<AmanecidasBehaviour.AMANECIDA_ATTACKS> GetAttackIdsPerHP(AmanecidaAttackScriptableConfig.AmanecidaAttacksByWeapon atks, float hpPercentage)
		{
			List<AmanecidasBehaviour.AMANECIDA_ATTACKS> list = new List<AmanecidasBehaviour.AMANECIDA_ATTACKS>();
			foreach (AmanecidaAttackScriptableConfig.AmanecidaAttackConfig atk in atks.attacks)
			{
				if (AmanecidaAttackScriptableConfig.IsActiveInHpSection(atk, hpPercentage))
				{
					list.Add(atk.attackID);
				}
			}
			return list;
		}

		public List<AmanecidaAttackScriptableConfig.AmanecidaAttackConfig> GetAttacksByWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON weapon)
		{
			return this.attackConfigs.Find((AmanecidaAttackScriptableConfig.AmanecidaAttacksByWeapon x) => x.weapon == weapon).attacks;
		}

		public List<AmanecidasBehaviour.AMANECIDA_ATTACKS> GetAttackIdsByWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON weapon, bool useHP = true, float hpPercentage = 1f)
		{
			if (useHP)
			{
				return AmanecidaAttackScriptableConfig.GetAttackIdsPerHP(this.attackConfigs.Find((AmanecidaAttackScriptableConfig.AmanecidaAttacksByWeapon x) => x.weapon == weapon), hpPercentage);
			}
			return AmanecidaAttackScriptableConfig.GetAttackIds(this.attackConfigs.Find((AmanecidaAttackScriptableConfig.AmanecidaAttacksByWeapon x) => x.weapon == weapon), true);
		}

		public AmanecidaAttackScriptableConfig.AmanecidaAttackConfig GetAttackConfig(AmanecidasAnimatorInyector.AMANECIDA_WEAPON weapon, AmanecidasBehaviour.AMANECIDA_ATTACKS atk)
		{
			List<AmanecidaAttackScriptableConfig.AmanecidaAttackConfig> attacksByWeapon = this.GetAttacksByWeapon(weapon);
			return attacksByWeapon.Find((AmanecidaAttackScriptableConfig.AmanecidaAttackConfig x) => x.attackID == atk);
		}

		public List<float> GetWeights(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> filteredAtks, AmanecidasAnimatorInyector.AMANECIDA_WEAPON weapon)
		{
			List<float> list = new List<float>();
			foreach (AmanecidasBehaviour.AMANECIDA_ATTACKS atk in filteredAtks)
			{
				list.Add(this.GetAttackConfig(weapon, atk).weight);
			}
			return list;
		}

		public List<AmanecidaAttackScriptableConfig.AmanecidaAttacksByWeapon> attackConfigs;

		[OdinSerialize]
		[FoldoutGroup("Debug", 0)]
		public Dictionary<KeyCode, AmanecidasBehaviour.AMANECIDA_ATTACKS> debugActions;

		[Serializable]
		public struct AmanecidaAttackConfig
		{
			[TableColumnWidth(200)]
			public AmanecidasBehaviour.AMANECIDA_ATTACKS attackID;

			[VerticalGroup("Active", 0)]
			[TableColumnWidth(125)]
			[LabelText("100% HP%")]
			[LabelWidth(80f)]
			public bool activeFirstThird;

			[VerticalGroup("Active", 0)]
			[TableColumnWidth(125)]
			[LabelText("<66% HP%")]
			[LabelWidth(80f)]
			public bool activeSecondThird;

			[VerticalGroup("Active", 0)]
			[TableColumnWidth(125)]
			[LabelText("<33% HP%")]
			[LabelWidth(80f)]
			public bool activeThirdPart;

			[VerticalGroup("Enabled", 0)]
			[TableColumnWidth(60)]
			[HideLabel]
			public bool active;

			[TableColumnWidth(80)]
			[VerticalGroup("Recovery", 0)]
			[HideLabel]
			[SuffixLabel("seconds", false, Overlay = true)]
			public float recoverySeconds;

			[TableColumnWidth(80)]
			[VerticalGroup("Recovery", 0)]
			[HideLabel]
			[SuffixLabel("seconds", false, Overlay = true)]
			public float recoverySeconds2;

			[TableColumnWidth(80)]
			[VerticalGroup("Recovery", 0)]
			[HideLabel]
			[SuffixLabel("seconds", false, Overlay = true)]
			public float recoverySeconds3;

			[VerticalGroup("Repetitions", 0)]
			[TableColumnWidth(80)]
			[HideLabel]
			public int repetitions;

			[VerticalGroup("Repetitions", 0)]
			[TableColumnWidth(80)]
			[HideLabel]
			public int repetitions2nd;

			[VerticalGroup("Repetitions", 0)]
			[TableColumnWidth(80)]
			[HideLabel]
			public int repetitions3rd;

			[TableColumnWidth(100)]
			[ProgressBar(0.0, 5.0, 0.15f, 0.47f, 0.74f)]
			[VerticalGroup("Weight", 0)]
			[HideLabel]
			public float weight;

			[TableColumnWidth(100)]
			[ProgressBar(0.0, 5.0, 0.15f, 0.47f, 0.74f)]
			[VerticalGroup("Weight", 0)]
			[HideLabel]
			public float weight2;

			[TableColumnWidth(100)]
			[ProgressBar(0.0, 5.0, 0.15f, 0.47f, 0.74f)]
			[VerticalGroup("Weight", 0)]
			[HideLabel]
			public float weight3;

			[TableColumnWidth(200)]
			[DrawWithUnity]
			public List<AmanecidasBehaviour.AMANECIDA_ATTACKS> alwaysFollowedBy;

			[TableColumnWidth(200)]
			[DrawWithUnity]
			public List<AmanecidasBehaviour.AMANECIDA_ATTACKS> cantBeFollowedBy;
		}

		[Serializable]
		public struct AmanecidaAttacksByWeapon
		{
			[EnumToggleButtons]
			[HideLabel]
			public AmanecidasAnimatorInyector.AMANECIDA_WEAPON weapon;

			[TableList]
			[FoldoutGroup("Attacks table", 0)]
			public List<AmanecidaAttackScriptableConfig.AmanecidaAttackConfig> attacks;
		}
	}
}
