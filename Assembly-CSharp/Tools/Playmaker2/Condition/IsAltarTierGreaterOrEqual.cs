using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the altar tier is greater or equal to a given tier.")]
	public class IsAltarTierGreaterOrEqual : FsmStateAction
	{
		public override void Reset()
		{
			if (this.tierToCheck == null)
			{
				this.tierToCheck = new FsmInt();
			}
			this.tierToCheck.Value = 0;
		}

		public override void OnEnter()
		{
			int num = (this.tierToCheck == null) ? 0 : this.tierToCheck.Value;
			if (Core.Alms.GetAltarLevel() >= num)
			{
				base.Fsm.Event(this.currentIsGreaterOrEqual);
			}
			else
			{
				base.Fsm.Event(this.currentIsLower);
			}
			base.Finish();
		}

		[RequiredField]
		public FsmInt tierToCheck;

		public FsmEvent currentIsGreaterOrEqual;

		public FsmEvent currentIsLower;
	}
}
