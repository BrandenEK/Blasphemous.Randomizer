using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Marks the current Penitence as completed.")]
	public class PenitenceCompleteCurrent : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.PenitenceManager.MarkCurrentPenitenceAsCompleted();
			base.Finish();
		}
	}
}
