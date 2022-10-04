using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.FrameworkCore
{
	[CreateAssetMenu(fileName = "skill", menuName = "Blasphemous/Unlockable CONFIG")]
	public class UnlockableSkillConfiguration : ScriptableObject
	{
		public int GetMaxTier()
		{
			return this.TierConfiguration.Count;
		}

		public int GetUnlockNeeded(int tier)
		{
			int result = -1;
			if (tier < this.TierConfiguration.Count)
			{
				result = this.TierConfiguration[tier];
			}
			return result;
		}

		[SerializeField]
		private List<int> TierConfiguration = new List<int>
		{
			0,
			1,
			3,
			5,
			7,
			9,
			14
		};
	}
}
