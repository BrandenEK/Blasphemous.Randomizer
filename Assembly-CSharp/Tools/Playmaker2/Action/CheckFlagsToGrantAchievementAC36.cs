using System;
using System.Collections.Generic;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[HutongGames.PlayMaker.Tooltip("Checks Candelaria Flags To Grant Achievement AC36 if all the items have been bought.")]
	public class CheckFlagsToGrantAchievementAC36 : FsmStateAction
	{
		public override void OnEnter()
		{
			bool flag = true;
			foreach (string str in this.itemsIds)
			{
				string text = str + this.achievementFlagSuffix.Value;
				if (!Core.Events.GetFlag(text))
				{
					Debug.Log("CheckFlagsToGrantAchievementAC36: flag with id: '" + text + "' hasn't been raised!");
					flag = false;
				}
				else
				{
					Debug.Log("CheckFlagsToGrantAchievementAC36: flag with id: '" + text + "' has already been raised!");
				}
			}
			if (flag)
			{
				Debug.Log("CheckFlagsToGrantAchievementAC36: GrantAchievement!");
				Core.AchievementsManager.GrantAchievement("AC36");
			}
			base.Finish();
		}

		[RequiredField]
		public FsmString achievementFlagSuffix;

		private readonly List<string> itemsIds = new List<string>
		{
			"QI58",
			"RB05",
			"RB09",
			"QI11",
			"RB37",
			"RB02",
			"QI71",
			"RB12",
			"QI49"
		};
	}
}
