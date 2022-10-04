using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Entity")]
	[Tooltip("Event raised when a destructible is destroyed.")]
	public class CinematicEnded : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Cinematics.CinematicStarted += this.CinematicStarted;
		}

		private void CinematicStarted()
		{
			base.Fsm.Event(this.onSuccess);
			base.Finish();
		}

		public override void OnExit()
		{
			Core.Cinematics.CinematicStarted -= this.CinematicStarted;
		}

		public FsmEvent onSuccess;
	}
}
