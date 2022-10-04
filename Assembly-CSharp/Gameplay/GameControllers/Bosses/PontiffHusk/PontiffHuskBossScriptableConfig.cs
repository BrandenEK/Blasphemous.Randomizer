using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffHusk
{
	[CreateAssetMenu(menuName = "Blasphemous/Bosses/PontiffHuskBoss/PontiffHuskBossAttacksConfig", fileName = "PontiffHuskBossAttacksConfig")]
	public class PontiffHuskBossScriptableConfig : SerializedScriptableObject
	{
		public List<PontiffHuskBossBehaviour.PH_ATTACKS> GetAttackIds(bool onlyActive, bool useHP, float hpPercentage)
		{
			List<PontiffHuskBossBehaviour.PH_ATTACKS> list = new List<PontiffHuskBossBehaviour.PH_ATTACKS>();
			foreach (PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig atk in this.attacksConfig)
			{
				if (this.ShouldReturnAttack(atk, onlyActive, useHP, hpPercentage))
				{
					list.Add(atk.attackID);
				}
			}
			return list;
		}

		public int GetAttackRepetitions(PontiffHuskBossBehaviour.PH_ATTACKS atk, bool useHP, float hpPercentage)
		{
			PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig attackConfig = this.GetAttackConfig(atk);
			if (useHP)
			{
				if (hpPercentage < 0.33f)
				{
					return attackConfig.repetitions3;
				}
				if (hpPercentage < 0.66f)
				{
					return attackConfig.repetitions2;
				}
			}
			return attackConfig.repetitions1;
		}

		public float GetAttackRecoverySeconds(PontiffHuskBossBehaviour.PH_ATTACKS atk, bool useHP, float hpPercentage)
		{
			PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig attackConfig = this.GetAttackConfig(atk);
			if (useHP)
			{
				if (hpPercentage < 0.33f)
				{
					return attackConfig.recoverySeconds3;
				}
				if (hpPercentage < 0.66f)
				{
					return attackConfig.recoverySeconds2;
				}
			}
			return attackConfig.recoverySeconds1;
		}

		public List<float> GetFilteredAttacksWeights(List<PontiffHuskBossBehaviour.PH_ATTACKS> filteredAtks, bool useHP, float hpPercentage)
		{
			List<float> list = new List<float>();
			foreach (PontiffHuskBossBehaviour.PH_ATTACKS atk in filteredAtks)
			{
				if (useHP)
				{
					if (hpPercentage < 0.33f)
					{
						list.Add(this.GetAttackConfig(atk).weight3);
					}
					else if (hpPercentage < 0.66f)
					{
						list.Add(this.GetAttackConfig(atk).weight2);
					}
					else
					{
						list.Add(this.GetAttackConfig(atk).weight1);
					}
				}
				else
				{
					list.Add(this.GetAttackConfig(atk).weight1);
				}
			}
			return list;
		}

		public PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig GetAttackConfig(PontiffHuskBossBehaviour.PH_ATTACKS atk)
		{
			return this.attacksConfig.Find((PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig x) => x.attackID == atk);
		}

		private bool ShouldReturnAttack(PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig atk, bool onlyActive, bool useHP, float hpPercentage)
		{
			bool flag = !onlyActive || atk.active;
			if (useHP)
			{
				flag = (flag && this.IsActiveInHpSection(atk, hpPercentage));
			}
			return flag;
		}

		private bool IsActiveInHpSection(PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig atk, float hpPercentage)
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

		[TableList]
		public List<PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig> attacksConfig;

		[OdinSerialize]
		[FoldoutGroup("Debug", 0)]
		public Dictionary<KeyCode, PontiffHuskBossBehaviour.PH_ATTACKS> debugActions;

		[Serializable]
		public struct PontiffHuskBossAttackConfig
		{
			[TableColumnWidth(200)]
			public PontiffHuskBossBehaviour.PH_ATTACKS attackID;

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
			public float recoverySeconds1;

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
			public int repetitions1;

			[VerticalGroup("Repetitions", 0)]
			[TableColumnWidth(80)]
			[HideLabel]
			public int repetitions2;

			[VerticalGroup("Repetitions", 0)]
			[TableColumnWidth(80)]
			[HideLabel]
			public int repetitions3;

			[TableColumnWidth(100)]
			[ProgressBar(0.0, 5.0, 0.15f, 0.47f, 0.74f)]
			[VerticalGroup("Weight", 0)]
			[HideLabel]
			public float weight1;

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
			public List<PontiffHuskBossBehaviour.PH_ATTACKS> alwaysFollowedBy;

			[TableColumnWidth(200)]
			[DrawWithUnity]
			public List<PontiffHuskBossBehaviour.PH_ATTACKS> cantBeFollowedBy;
		}
	}
}
