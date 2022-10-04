using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Show or hide ingame UI.")]
	public class ShowUI : FsmStateAction
	{
		public override void OnEnter()
		{
			bool showGamePlayUI = this.show != null && this.show.Value;
			Core.UI.ShowGamePlayUI = showGamePlayUI;
			base.Finish();
		}

		public FsmBool show;
	}
}
