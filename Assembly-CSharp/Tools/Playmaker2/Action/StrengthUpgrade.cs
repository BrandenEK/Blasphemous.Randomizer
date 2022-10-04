using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Add a Strength Upgrade")]
	public class StrengthUpgrade : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.Stats.Strength.Upgrade();
			base.Finish();
		}
	}
}
