using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.PlayMaker.Action
{
	public class PlayerGrounded : FsmStateAction
	{
		public override void OnEnter()
		{
			this.alreadyTriggeredAnEvent = false;
			if (Core.Logic.Penitent.Status.IsGrounded)
			{
				base.Fsm.Event(this.IsGrounded);
				this.alreadyTriggeredAnEvent = true;
			}
			else if (this.CheckOnlyOnce.Value)
			{
				base.Fsm.Event(this.IsNotGrounded);
			}
		}

		public override void OnUpdate()
		{
			if (!this.CheckOnlyOnce.Value && !this.alreadyTriggeredAnEvent && Core.Logic.Penitent.Status.IsGrounded)
			{
				base.Fsm.Event(this.IsGrounded);
				this.alreadyTriggeredAnEvent = true;
			}
		}

		public FsmBool CheckOnlyOnce;

		public FsmEvent IsGrounded;

		public FsmEvent IsNotGrounded;

		private bool alreadyTriggeredAnEvent;
	}
}
