using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Clear all your guilt.")]
	public class GuiltReset : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.GuiltManager.ResetGuilt(true);
			base.Finish();
		}
	}
}
