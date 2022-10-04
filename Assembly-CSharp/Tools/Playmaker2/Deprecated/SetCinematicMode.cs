using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Deprecated
{
	[ActionCategory("Blasphemous Deprecated")]
	[Tooltip("Starts/Ends the cinematic mode.")]
	public class SetCinematicMode : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.UI.Cinematic.CinematicMode(this.cinematic.Value);
			base.Finish();
		}

		public FsmBool cinematic;
	}
}
