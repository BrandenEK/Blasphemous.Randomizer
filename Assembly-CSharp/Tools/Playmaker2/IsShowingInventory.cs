using System;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the inventory is showing.")]
	public class IsShowingInventory : FsmStateAction
	{
		public override void OnEnter()
		{
			if (UIController.instance.IsShowingInventory)
			{
				base.Fsm.Event(this.isShowing);
			}
			else
			{
				base.Fsm.Event(this.notShowing);
			}
			base.Finish();
		}

		public FsmEvent isShowing;

		public FsmEvent notShowing;
	}
}
