using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Add or remove purge points.")]
	public class BossKeyCheck : FsmStateAction
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
			int num = (this.slot == null) ? 0 : this.slot.Value;
			bool flag = Core.InventoryManager.CheckBossKey(num);
			if (flag)
			{
				base.Fsm.Event(this.bossKeyFound);
			}
			else
			{
				base.Fsm.Event(this.bossKeyNotFound);
			}
			if (this.outValue != null)
			{
				this.outValue.Value = flag;
			}
			base.Finish();
		}

		public FsmInt slot;

		public FsmBool outValue;

		public FsmEvent bossKeyFound;

		public FsmEvent bossKeyNotFound;
	}
}
