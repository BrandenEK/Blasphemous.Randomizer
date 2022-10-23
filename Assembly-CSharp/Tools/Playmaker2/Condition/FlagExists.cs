using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the chosen flags exists.")]
	public class FlagExists : FsmStateAction
	{
		public override void Reset()
		{
			this.outValue = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			string text = this.flagName.Value.ToUpper().Replace(' ', '_');
			Core.Randomizer.Log(string.Concat(new object[]
			{
				"Object ",
				base.Owner.transform.position.GetHashCode(),
				" is checking for flag ",
				text
			}), 0);
			bool flag = Core.Events.GetFlag(text);
			if (base.Owner.transform.position.GetHashCode() == -948699136)
			{
				if (text == "D01Z06S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI38");
				}
				else if (text == "D02Z05S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI39");
				}
				else if (text == "D03Z04S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI40");
				}
			}
			if (this.outValue != null)
			{
				this.outValue.Value = flag;
			}
			if (flag)
			{
				base.Fsm.Event(this.flagAvailable);
			}
			else
			{
				base.Fsm.Event(this.flagUnavailable);
			}
			base.Finish();
		}

		public FsmString category;

		public FsmString flagName;

		public bool runtimeFlag;

		public FsmEvent flagAvailable;

		public FsmEvent flagUnavailable;

		public FsmBool outValue;
	}
}
