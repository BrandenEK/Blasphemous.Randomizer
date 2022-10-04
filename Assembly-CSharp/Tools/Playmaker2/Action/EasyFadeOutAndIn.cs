using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Fades out for a given number of seconds and then fades in.")]
	public class EasyFadeOutAndIn : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.UI.Fade.Fade(true, 1f, 0f, delegate
			{
				this.OnFadeInFinished();
			});
			base.Finish();
		}

		private void OnFadeInFinished()
		{
			Core.UI.Fade.Fade(false, 1f, this.TimeInFade.Value, null);
		}

		public FsmFloat TimeInFade;
	}
}
