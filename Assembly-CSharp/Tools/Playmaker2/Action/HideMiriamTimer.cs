using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Hides Miriam Timer")]
	public class HideMiriamTimer : FsmStateAction
	{
		public override void OnEnter()
		{
			UIController.instance.HideMiriamTimer();
			base.Finish();
		}
	}
}
