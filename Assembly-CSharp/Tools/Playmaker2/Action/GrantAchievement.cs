using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Grants an Achievement.")]
	public class GrantAchievement : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.AchievementsManager.GrantAchievement(this.achievementId.Value);
			base.Finish();
		}

		[RequiredField]
		public FsmString achievementId;
	}
}
