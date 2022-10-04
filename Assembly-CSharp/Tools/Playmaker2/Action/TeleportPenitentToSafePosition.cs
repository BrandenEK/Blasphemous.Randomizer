using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Teleports TPO at Last Safe Position.")]
	public class TeleportPenitentToSafePosition : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.Teleport(Core.LevelManager.LastSafePosition);
			base.Finish();
		}
	}
}
