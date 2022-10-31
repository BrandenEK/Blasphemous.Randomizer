using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Extend the maximum value of the player life.")]
	public class LifeUpgrade : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Randomizer.giveReward("Lady[" + Core.LevelManager.currentLevel.LevelName + "]", true);
			base.Finish();
		}
	}
}
