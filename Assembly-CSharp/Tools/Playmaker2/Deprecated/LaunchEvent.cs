using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Deprecated
{
	[ActionCategory("Blasphemous Deprecated")]
	[Tooltip("Raises an event on the event system.")]
	public class LaunchEvent : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Events.LaunchEvent(this.eventName.Value, string.Empty);
			base.Finish();
		}

		public FsmString eventName;
	}
}
