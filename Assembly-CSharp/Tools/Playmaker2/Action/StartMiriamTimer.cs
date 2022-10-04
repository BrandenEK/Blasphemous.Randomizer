using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Starts Miriam Timer.")]
	public class StartMiriamTimer : FsmStateAction
	{
		public override void OnEnter()
		{
			UIController.instance.StartMiriamTimer();
			base.Finish();
		}
	}
}
