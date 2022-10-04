using System;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Starts Miriam Timer.")]
	public class WaitForMiriamTimerToRunOut : FsmStateAction
	{
		public override void OnEnter()
		{
			MiriamTimer.OnTimerRunOut = (Core.SimpleEvent)Delegate.Combine(MiriamTimer.OnTimerRunOut, new Core.SimpleEvent(this.OnTimerRunOut));
		}

		private void OnTimerRunOut()
		{
			MiriamTimer.OnTimerRunOut = (Core.SimpleEvent)Delegate.Remove(MiriamTimer.OnTimerRunOut, new Core.SimpleEvent(this.OnTimerRunOut));
			base.Finish();
		}
	}
}
