using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using Tools.DataContainer;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Stop a started rumble")]
	public class RumbleStop : FsmStateAction
	{
		public override void Reset()
		{
			if (this.allRumbles != null)
			{
				this.allRumbles.Value = false;
			}
		}

		public override void OnEnter()
		{
			bool flag = this.allRumbles != null && this.allRumbles.Value;
			if (this.rumble == null && !flag)
			{
				base.LogWarning("PlayMaker Action Rumble Stop - Rumble is blank");
			}
			else if (flag)
			{
				Core.Input.StopAllRumbles();
			}
			else
			{
				Core.Input.StopRumble(this.rumble.name);
			}
			base.Finish();
		}

		public FsmBool allRumbles;

		public RumbleData rumble;
	}
}
