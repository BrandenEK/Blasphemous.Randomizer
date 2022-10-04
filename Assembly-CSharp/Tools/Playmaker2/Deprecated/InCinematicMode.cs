using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Deprecated
{
	[ActionCategory("Blasphemous Deprecated")]
	[Tooltip("Starts/Ends the cinematic mode.")]
	public class InCinematicMode : FsmStateAction
	{
		public override void OnEnter()
		{
			if (Core.UI.Cinematic.InCinematicMode)
			{
				base.Fsm.Event(this.onSuccess);
			}
		}

		public FsmEvent onSuccess;
	}
}
