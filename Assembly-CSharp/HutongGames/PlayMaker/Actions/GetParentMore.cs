using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets the Parent of a Game Object.")]
	public class GetParentMore : FsmStateAction
	{
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.repetitions = 0;
			this.repeat = 0;
		}

		public override void OnEnter()
		{
			this.repeat = this.repetitions.Value;
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this.storeResult.Value = ((!(ownerDefaultTarget.transform.parent == null)) ? ownerDefaultTarget.transform.parent.gameObject : null);
				while (this.repeat > 0)
				{
					GameObject value = this.storeResult.Value;
					this.repeat--;
					this.storeResult.Value = ((!(value.transform.parent == null)) ? value.transform.parent.gameObject : null);
				}
			}
			else
			{
				this.storeResult.Value = null;
			}
			base.Finish();
		}

		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeResult;

		public FsmInt repetitions;

		private int repeat;
	}
}
