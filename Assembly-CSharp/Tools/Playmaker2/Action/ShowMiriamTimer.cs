using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Shows Miriam Timer and sets the Target Time.")]
	public class ShowMiriamTimer : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.SetTargetTime != null && this.SetTargetTime.Value)
			{
				UIController.instance.SetMiriamTimerTargetTime(this.TargetTime.Value);
			}
			UIController.instance.ShowMiriamTimer();
			base.Finish();
		}

		public FsmBool SetTargetTime;

		public FsmFloat TargetTime;
	}
}
