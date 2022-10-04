using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Entity")]
	[Tooltip("Event raised when a destructible is destroyed.")]
	public class CinematicStarted : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Cinematics.CinematicEnded += this.CinematicEnded;
		}

		private void CinematicEnded(bool cancelled)
		{
			base.Fsm.Event(this.onSuccess);
			base.Finish();
		}

		public override void OnExit()
		{
			Core.Cinematics.CinematicEnded -= this.CinematicEnded;
		}

		public FsmEvent onSuccess;
	}
}
