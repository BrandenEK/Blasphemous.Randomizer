using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Stops the current video reproduction.")]
	public class CutsceneStop : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Cinematics.EndCutscene(true);
		}
	}
}
