using System;
using Gameplay.UI.Widgets;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Wait for credits end scene.")]
	public class WaitForCreditsEnd : FsmStateAction
	{
		public override void OnUpdate()
		{
			if (CreditsWidget.instance == null || CreditsWidget.instance.HasEnded)
			{
				base.Finish();
			}
		}
	}
}
