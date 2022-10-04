using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Set if the Santos cutscene is playing.")]
	public class SetSantosCutscenePlaying : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Cinematics.InSantosCutscene = this.isPlaying.Value;
			base.Finish();
		}

		public FsmBool isPlaying;
	}
}
