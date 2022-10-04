using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Teleport to a TeleportLocation.")]
	public class Teleport : FsmStateAction
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
				base.LogWarning("PlayMaker Action Teleport - teleportId is blank");
			}
			else
			{
				Core.SpawnManager.Teleport(value);
				base.Finish();
			}
		}

		[RequiredField]
		public FsmString teleportId;
	}
}
