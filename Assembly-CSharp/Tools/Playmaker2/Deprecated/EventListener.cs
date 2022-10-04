using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Deprecated
{
	[ActionCategory("Blasphemous Deprecated")]
	[Tooltip("Triggers with an event from the event system.")]
	public class EventListener : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Events.OnEventLaunched += this.OnEvent;
		}

		public override void OnExit()
		{
			Core.Events.OnEventLaunched -= this.OnEvent;
		}

		private void OnEvent(string id, string parameter)
		{
			string a = this.eventName.Value.ToUpper().Replace(' ', '_');
			if (a == id)
			{
				base.Finish();
			}
		}

		public FsmString eventName;
	}
}
