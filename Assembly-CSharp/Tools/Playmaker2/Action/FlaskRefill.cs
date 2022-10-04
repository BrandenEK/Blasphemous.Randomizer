using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Refill the current unused flask to max.")]
	public class FlaskRefill : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.Stats.Flask.SetToCurrentMax();
			base.Finish();
		}
	}
}
