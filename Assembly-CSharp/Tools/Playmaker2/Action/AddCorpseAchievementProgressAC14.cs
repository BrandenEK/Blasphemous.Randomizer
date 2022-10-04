using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Checks for achievement AC14.")]
	public class AddCorpseAchievementProgressAC14 : FsmStateAction
	{
		public override void OnEnter()
		{
			AchievementsManager.CheckFlagsToGrantAC14();
			base.Finish();
		}
	}
}
