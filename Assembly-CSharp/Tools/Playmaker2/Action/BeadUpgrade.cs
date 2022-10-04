using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Add or remove a space for placing rosary beads.")]
	public class BeadUpgrade : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.Stats.BeadSlots.Upgrade();
			base.Finish();
		}
	}
}
