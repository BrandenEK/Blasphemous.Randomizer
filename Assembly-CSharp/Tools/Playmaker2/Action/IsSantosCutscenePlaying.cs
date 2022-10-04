using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Check if the Santos cutscene is playing.")]
	public class IsSantosCutscenePlaying : FsmStateAction
	{
		public override void OnEnter()
		{
			if (Core.Cinematics.InSantosCutscene)
			{
				base.Fsm.Event(this.yes);
			}
			else
			{
				base.Fsm.Event(this.no);
			}
			base.Finish();
		}

		public FsmEvent yes;

		public FsmEvent no;
	}
}
