using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Stops Miriam Timer.")]
	public class StopMiriamTimer : FsmStateAction
	{
		public override void OnEnter()
		{
			UIController.instance.StopMiriamTimer();
			base.Finish();
		}
	}
}
