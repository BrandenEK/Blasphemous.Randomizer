using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Snake
{
	[CreateAssetMenu(menuName = "Blasphemous/Bosses/Snake/SnakeScriptableConfig", fileName = "SnakeScriptableConfig")]
	public class SnakeScriptableConfig : SerializedScriptableObject
	{
		public List<SnakeBehaviour.SNAKE_ATTACKS> GetAttackIds(bool onlyActive, bool useHP, float hpPercentage)
		{
			List<SnakeBehaviour.SNAKE_ATTACKS> list = new List<SnakeBehaviour.SNAKE_ATTACKS>();
			foreach (SnakeScriptableConfig.SnakeAttackConfig atk in this.attacksConfig)
			{
				if (this.ShouldReturnAttack(atk, onlyActive, useHP, hpPercentage))
				{
					list.Add(atk.attackID);
				}
			}
			return list;
		}

		public int GetAttackRepetitions(SnakeBehaviour.SNAKE_ATTACKS atk, bool useHP, float hpPercentage)
		{
			SnakeScriptableConfig.SnakeAttackConfig attackConfig = this.GetAttackConfig(atk);
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

		public List<float> GetFilteredAttacksWeights(List<SnakeBehaviour.SNAKE_ATTACKS> filteredAtks, bool useHP, float hpPercentage)
		{
			List<float> list = new List<float>();
			foreach (SnakeBehaviour.SNAKE_ATTACKS atk in filteredAtks)
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

		public SnakeScriptableConfig.SnakeAttackConfig GetAttackConfig(SnakeBehaviour.SNAKE_ATTACKS atk)
		{
			return this.attacksConfig.Find((SnakeScriptableConfig.SnakeAttackConfig x) => x.attackID == atk);
		}

		private bool ShouldReturnAttack(SnakeScriptableConfig.SnakeAttackConfig atk, bool onlyActive, bool useHP, float hpPercentage)
		{
			bool flag = !onlyActive || atk.active;
			if (useHP)
			{
				flag = (flag && this.IsActiveInHpSection(atk, hpPercentage));
			}
			return flag;
		}

		private bool IsActiveInHpSection(SnakeScriptableConfig.SnakeAttackConfig atk, float hpPercentage)
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
		public List<SnakeScriptableConfig.SnakeAttackConfig> attacksConfig;

		[OdinSerialize]
		[FoldoutGroup("Debug", 0)]
		public Dictionary<KeyCode, SnakeBehaviour.SNAKE_ATTACKS> debugActions;

		[Serializable]
		public struct SnakeAttackConfig
		{
			[TableColumnWidth(200)]
			public SnakeBehaviour.SNAKE_ATTACKS attackID;

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
			public List<SnakeBehaviour.SNAKE_ATTACKS> alwaysFollowedBy;

			[TableColumnWidth(200)]
			[DrawWithUnity]
			public List<SnakeBehaviour.SNAKE_ATTACKS> cantBeFollowedBy;
		}
	}
}
