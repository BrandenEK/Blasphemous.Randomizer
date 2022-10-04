using System;
using Gameplay.UI.Widgets;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	public class CameraFade : FsmStateAction
	{
		public override void OnEnter()
		{
			FadeWidget instance = FadeWidget.instance;
			if (instance == null)
			{
				return;
			}
			FadeWidget.OnFadeShowEnd += this.FadeEnd;
			FadeWidget.OnFadeHidedEnd += this.FadeEnd;
			instance.StartEasyFade(this.originColor.Value, this.endColor.Value, this.Duration.Value, this.FadeOut.Value);
		}

		private void FadeEnd()
		{
			FadeWidget.OnFadeShowEnd -= this.FadeEnd;
			FadeWidget.OnFadeHidedEnd -= this.FadeEnd;
			base.Finish();
		}

		public FsmBool FadeOut;

		public FsmFloat Duration;

		public FsmColor originColor;

		public FsmColor endColor;
	}
}
