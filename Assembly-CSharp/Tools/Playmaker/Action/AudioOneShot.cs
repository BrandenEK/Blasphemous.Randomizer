using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Plays an audio.")]
	public class AudioOneShot : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Audio.PlaySfx(this.audioId.Value, 0f);
			base.Finish();
		}

		public FsmString audioId;
	}
}
