using System;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	public class EntityIsPenitent : FsmStateAction
	{
		public override void OnEnter()
		{
			base.Fsm.Event((!this.entity.Value.CompareTag("Penitent")) ? this.onFailure : this.onSuccess);
			base.Finish();
		}

		public FsmGameObject entity;

		public FsmEvent onSuccess;

		public FsmEvent onFailure;
	}
}
