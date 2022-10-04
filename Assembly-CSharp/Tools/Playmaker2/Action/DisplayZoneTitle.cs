using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Displays Zone Title screen.")]
	public class DisplayZoneTitle : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.NewMapManager.DisplayZoneName();
			base.Finish();
		}
	}
}
