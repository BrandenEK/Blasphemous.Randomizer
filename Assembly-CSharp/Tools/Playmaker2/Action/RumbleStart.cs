using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using Tools.DataContainer;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Rumble the pad")]
	public class RumbleStart : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.rumble)
			{
				Core.Input.ApplyRumble(this.rumble);
			}
			base.Finish();
		}

		public RumbleData rumble;
	}
}
