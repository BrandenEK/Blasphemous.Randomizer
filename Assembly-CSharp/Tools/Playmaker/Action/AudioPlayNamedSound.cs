using System;
using FMOD.Studio;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Plays an name sound.")]
	public class AudioPlayNamedSound : FsmStateAction
	{
		public override void Reset()
		{
			if (this.soundName == null)
			{
				this.soundName = new FsmString(string.Empty);
			}
			if (this.eventName == null)
			{
				this.eventName = new FsmString(string.Empty);
			}
		}

		public override void OnEnter()
		{
			GameModeManager gameModeManager = Core.GameModeManager;
			gameModeManager.OnEnterMenuMode = (Core.SimpleEvent)Delegate.Combine(gameModeManager.OnEnterMenuMode, new Core.SimpleEvent(this.StopSound));
			this.PlaySound();
			base.Finish();
		}

		public void PlaySound()
		{
			string keyName = (this.soundName == null) ? string.Empty : this.soundName.Value;
			string text = (this.eventName == null) ? string.Empty : this.eventName.Value;
			Core.Audio.PlayNamedSound(text, keyName);
		}

		public void StopSound()
		{
			string keyName = (this.soundName == null) ? string.Empty : this.soundName.Value;
			Core.Audio.StopNamedSound(keyName, 0);
		}

		public FsmString soundName;

		public FsmString eventName;

		private EventInstance audioEvent;
	}
}
