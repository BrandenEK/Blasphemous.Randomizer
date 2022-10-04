using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	public class CheckPurchasedSkills : FsmStateAction
	{
		public override void OnUpdate()
		{
			int lockedSkillsNumber = Core.SkillManager.GetLockedSkillsNumber();
			if (lockedSkillsNumber == 0)
			{
				base.Fsm.Event(this.allPurchased);
			}
			if (lockedSkillsNumber > 0)
			{
				base.Fsm.Event(this.notPurchased);
			}
			base.Finish();
		}

		public FsmEvent notPurchased;

		public FsmEvent allPurchased;
	}
}
