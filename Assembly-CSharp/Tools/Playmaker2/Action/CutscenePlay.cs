using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using Tools.DataContainer;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Pauses the game state and starts a video reproduction.")]
	public class CutscenePlay : FsmStateAction
	{
		public override void Reset()
		{
			this.muteAudio = new FsmBool();
			this.muteAudio.UseVariable = false;
			this.muteAudio.Value = true;
			this.useStartFade = new FsmBool();
			this.useStartFade.UseVariable = false;
			this.useStartFade.Value = true;
			this.useEndFade = new FsmBool();
			this.useEndFade.UseVariable = false;
			this.useEndFade.Value = true;
			this.timeStartFade = new FsmFloat();
			this.timeStartFade.Value = 1.5f;
			this.timeStartFade.UseVariable = false;
			this.timeEndFade = new FsmFloat();
			this.timeEndFade.Value = 1.5f;
			this.timeEndFade.UseVariable = false;
			this.useSubtitleBackground = new FsmBool();
			this.useSubtitleBackground.UseVariable = false;
			this.useSubtitleBackground.Value = true;
		}

		public override void OnEnter()
		{
			bool flag = this.muteAudio == null || this.muteAudio.Value;
			bool flag2 = this.useStartFade == null || this.useStartFade.Value;
			bool flag3 = this.useEndFade == null || this.useEndFade.Value;
			float fadeStart = (this.timeStartFade == null) ? 1.5f : this.timeStartFade.Value;
			float fadeEnd = (this.timeEndFade == null) ? 1.5f : this.timeEndFade.Value;
			if (!flag2)
			{
				fadeStart = 0f;
			}
			if (!flag3)
			{
				fadeEnd = 0f;
			}
			bool useBackground = this.useSubtitleBackground == null || this.useSubtitleBackground.Value;
			if (this.cutscene != null)
			{
				Core.Cinematics.CinematicEnded += this.OnCinematicEnded;
				Core.Cinematics.StartCutscene(this.cutscene, flag, fadeStart, fadeEnd, useBackground);
			}
			base.Finish();
		}

		public override void OnUpdate()
		{
		}

		public override void OnExit()
		{
			Core.Cinematics.CinematicEnded -= this.OnCinematicEnded;
		}

		private void OnCinematicEnded(bool cancelled)
		{
			base.Fsm.Event((!cancelled) ? this.onSuccess : this.onFailure);
		}

		public CutsceneData cutscene;

		public FsmBool muteAudio;

		public FsmBool useStartFade;

		public FsmFloat timeStartFade;

		public FsmBool useEndFade;

		public FsmFloat timeEndFade;

		public FsmBool useSubtitleBackground;

		public FsmEvent onSuccess;

		public FsmEvent onFailure;

		private const float DEFAULT_FADE = 1.5f;
	}
}
