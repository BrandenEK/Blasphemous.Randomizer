using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Checks if an Achievement is granted.")]
	public class CheckAchievement : FsmStateAction
	{
		public override void OnEnter()
		{
			bool flag = Core.AchievementsManager.CheckAchievementGranted(this.achievementId.Value);
			if (flag)
			{
				base.Fsm.Event(this.achievementGranted);
			}
			else
			{
				base.Fsm.Event(this.achievementNotGranted);
			}
			base.Finish();
		}

		[RequiredField]
		public FsmString achievementId;

		public FsmEvent achievementGranted;

		public FsmEvent achievementNotGranted;
	}
}
