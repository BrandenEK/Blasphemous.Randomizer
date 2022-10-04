using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Stop Current Music.")]
	public class StopCurrentMusic : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Audio.Ambient.StopCurrent();
			base.Finish();
		}
	}
}
