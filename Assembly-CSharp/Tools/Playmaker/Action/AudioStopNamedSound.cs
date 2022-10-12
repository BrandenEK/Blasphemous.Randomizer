using System;
using FMOD.Studio;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Stops an name sound.")]
	public class AudioStopNamedSound : FsmStateAction
	{
		public override void Reset()
		{
			if (this.soundName == null)
			{
				this.soundName = new FsmString(string.Empty);
			}
		}

		public override void OnEnter()
		{
			string keyName = (this.soundName == null) ? string.Empty : this.soundName.Value;
			Core.Audio.StopNamedSound(keyName, this.stopMode);
			base.Finish();
		}

		public FsmString soundName;

		public FMOD.Studio.STOP_MODE stopMode;
	}
}
