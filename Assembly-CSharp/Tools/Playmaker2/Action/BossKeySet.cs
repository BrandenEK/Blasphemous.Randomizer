using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Add or remove purge points.")]
	public class BossKeySet : FsmStateAction
	{
		public override void OnEnter()
		{
			int num = (this.slot == null) ? 0 : this.slot.Value;
			bool flag = this.value == null || this.value.Value;
			if (flag)
			{
				Core.InventoryManager.AddBossKey(num);
			}
			else
			{
				Core.InventoryManager.RemoveBossKey(num);
			}
			base.Finish();
		}

		public FsmInt slot;

		public FsmBool value;
	}
}
