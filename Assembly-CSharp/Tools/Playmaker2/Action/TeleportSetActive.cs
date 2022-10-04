using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Activate or deactivate a TeleportLocation.")]
	public class TeleportSetActive : FsmStateAction
	{
		public override void Reset()
		{
			if (this.teleportId == null)
			{
				this.teleportId = new FsmString();
			}
			this.teleportId.Value = string.Empty;
			if (this.active == null)
			{
				this.active = new FsmBool();
			}
			this.active.Value = true;
		}

		public override void OnEnter()
		{
			string value = (this.teleportId == null) ? string.Empty : this.teleportId.Value;
			bool flag = this.active != null && this.active.Value;
			if (string.IsNullOrEmpty(value))
			{
				base.LogWarning("PlayMaker Action TeleportSetActive - teleportId is blank");
			}
			else
			{
				Core.SpawnManager.SetTeleportActive(value, flag);
				base.Finish();
			}
		}

		[RequiredField]
		public FsmString teleportId;

		public FsmBool active;
	}
}
