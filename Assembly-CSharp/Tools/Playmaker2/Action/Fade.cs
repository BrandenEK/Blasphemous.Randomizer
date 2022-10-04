using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Deprecated")]
	[Tooltip("Starts/Ends the cinematic mode.")]
	public class Fade : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.UI.Fade.Fade(this.showing.Value, this.duration.Value, 0f, null);
			base.Finish();
		}

		public FsmBool showing = false;

		public FsmFloat duration = 1f;
	}
}
