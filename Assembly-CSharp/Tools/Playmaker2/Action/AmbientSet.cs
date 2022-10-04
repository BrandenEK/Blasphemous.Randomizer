using System;
using FMODUnity;
using Framework.Managers;
using HutongGames.PlayMaker;
using Tools.Audio;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Change current ambient data.")]
	public class AmbientSet : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Audio.Ambient.SetSceneParams(this.trackIdentifier, this.idReverb, new AudioParam[0], string.Empty);
			base.Finish();
		}

		[EventRef]
		public string trackIdentifier;

		[EventRef]
		public string idReverb;
	}
}
