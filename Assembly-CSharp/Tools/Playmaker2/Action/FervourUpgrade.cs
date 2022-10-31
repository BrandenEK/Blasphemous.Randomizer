using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Extend the maximum value of the fervour.")]
	public class FervourUpgrade : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Randomizer.giveReward("Oil[" + Core.LevelManager.currentLevel.LevelName + "]", true);
			base.Finish();
		}
	}
}
