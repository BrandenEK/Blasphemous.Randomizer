using System;
using Framework.Managers;
using UnityEngine;

namespace Framework.Achievements
{
	public class AC44Checker : MonoBehaviour
	{
		private void Start()
		{
			float currentTimePlayedForAC = Core.Persistence.GetCurrentTimePlayedForAC44();
			if (currentTimePlayedForAC / 60f < this.MaximunMinutesForAC44)
			{
				Core.AchievementsManager.Achievements["AC44"].Grant();
			}
		}

		public float MaximunMinutesForAC44 = 180f;
	}
}
