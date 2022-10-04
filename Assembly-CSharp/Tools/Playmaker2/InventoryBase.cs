using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2
{
	public abstract class InventoryBase : FsmStateAction
	{
		public virtual bool UseObject
		{
			get
			{
				return true;
			}
		}

		public virtual bool UseSlot
		{
			get
			{
				return false;
			}
		}

		public override void Reset()
		{
			if (this.objectId != null)
			{
				this.objectId.Value = string.Empty;
			}
		}

		public override void OnEnter()
		{
			string text = (this.objectId == null) ? string.Empty : this.objectId.Value;
			int objType = (this.itemType == null) ? 0 : this.itemType.Value;
			int num = (this.slot == null) ? 0 : this.slot.Value;
			if (this.UseObject && string.IsNullOrEmpty(text))
			{
				base.LogWarning("PlayMaker Inventory Action - objectId is blank");
			}
			else
			{
				bool flag = this.executeAction(text, (InventoryManager.ItemType)objType, num);
				if (flag && this.onSuccess != null)
				{
					base.Fsm.Event(this.onSuccess);
				}
				if (!flag && this.onFailure != null)
				{
					base.Fsm.Event(this.onFailure);
				}
				base.Finish();
			}
		}

		public abstract bool executeAction(string objectIdStting, InventoryManager.ItemType objType, int slot);

		[Tooltip("Type of object")]
		public FsmInt itemType;

		[Tooltip("Object ID")]
		public FsmString objectId;

		[Tooltip("Slot")]
		public FsmInt slot;

		public FsmEvent onSuccess;

		public FsmEvent onFailure;
	}
}
