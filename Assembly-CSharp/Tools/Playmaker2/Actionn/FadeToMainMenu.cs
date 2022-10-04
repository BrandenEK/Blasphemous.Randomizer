using System;
using Framework.Managers;
using Gameplay.UI.Widgets;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Actionn
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Fades to main menu")]
	public class FadeToMainMenu : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.useFade)
			{
				FadeWidget.instance.Fade(true, 1f, 0f, delegate
				{
					Core.Logic.LoadMenuScene(this.useFadeLoadMenu);
				});
			}
			else
			{
				Core.Logic.LoadMenuScene(this.useFadeLoadMenu);
			}
			base.Finish();
		}

		public bool useFade = true;

		public bool useFadeLoadMenu = true;
	}
}
