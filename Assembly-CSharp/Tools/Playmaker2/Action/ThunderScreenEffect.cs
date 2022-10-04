using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Sets the screen to a black & white effect for a defined time.")]
	public class ThunderScreenEffect : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(this.effectTime.Value);
			base.Finish();
		}

		public FsmFloat effectTime;
	}
}
