using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Refill the current life to max.")]
	public class LifeRefill : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
			base.Finish();
		}
	}
}
