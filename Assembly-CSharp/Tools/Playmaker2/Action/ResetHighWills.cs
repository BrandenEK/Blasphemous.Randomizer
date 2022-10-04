using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Reset High Wills scene.")]
	public class ResetHighWills : FsmStateAction
	{
		public override void OnEnter()
		{
			PlayMakerFSM.BroadcastEvent("ON HW RESET");
			Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
			Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax();
			Core.Logic.Penitent.Stats.Flask.SetToCurrentMax();
			Core.Logic.Penitent.Physics.Enable2DCollision(true);
			Core.Logic.Penitent.VerticalAttack.StopCast();
			base.Finish();
		}
	}
}
