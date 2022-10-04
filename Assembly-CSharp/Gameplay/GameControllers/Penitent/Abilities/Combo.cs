using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class Combo : Ability
	{
		public bool IsAvailable
		{
			get
			{
				return base.CanExecuteSkilledAbility();
			}
		}

		public UnlockableSkill GetMaxSkill
		{
			get
			{
				return base.GetLastUnlockedSkill();
			}
		}

		[SerializeField]
		public Combo.ExecutionTier DefaulExecutionTier;

		[SerializeField]
		public Combo.ExecutionTier FirstUpgradeExecutionTier;

		[SerializeField]
		public Combo.ExecutionTier SecondUpgradeExecutionTier;

		[Serializable]
		public struct ExecutionTier
		{
			public ExecutionTier(int minExecutionTier, int maxExecutionTier)
			{
				this.MinExecutionTier = minExecutionTier;
				this.MaxExecutionTier = maxExecutionTier;
			}

			[Range(1f, 100f)]
			public int MinExecutionTier;

			[Range(1f, 100f)]
			public int MaxExecutionTier;
		}
	}
}
