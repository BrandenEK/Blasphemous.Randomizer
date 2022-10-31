using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Add or remove meaculpa.")]
	public class MeaCulpaUpgrade : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Randomizer.giveReward("Sword[" + Core.LevelManager.currentLevel.LevelName + "]", true);
			base.Finish();
		}
	}
}
