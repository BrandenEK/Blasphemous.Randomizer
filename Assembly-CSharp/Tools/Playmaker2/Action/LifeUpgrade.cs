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
			Core.Logic.Penitent.Stats.Life.Upgrade();
			Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
			base.Finish();
		}
	}
}
