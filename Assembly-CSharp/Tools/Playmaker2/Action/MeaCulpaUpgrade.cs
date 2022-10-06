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
			Core.Logic.Penitent.Stats.MeaCulpa.Upgrade();
			base.Finish();
		}
	}
}
