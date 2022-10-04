using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the teleport is active.")]
	public class TeleportIsActive : FsmStateAction
	{
		public override void Reset()
		{
			if (this.teleportId == null)
			{
				this.teleportId = new FsmString();
			}
			this.teleportId.Value = string.Empty;
		}

		public override void OnEnter()
		{
			string value = (this.teleportId == null) ? string.Empty : this.teleportId.Value;
			if (string.IsNullOrEmpty(value))
			{
				base.LogWarning("PlayMaker Action TeleportIsActive - teleportId is blank");
			}
			else if (Core.SpawnManager.IsTeleportActive(value))
			{
				base.Fsm.Event(this.teleportAvailable);
			}
			else
			{
				base.Fsm.Event(this.teleportUnavailable);
			}
			base.Finish();
		}

		public FsmString teleportId;

		public FsmEvent teleportAvailable;

		public FsmEvent teleportUnavailable;
	}
}
