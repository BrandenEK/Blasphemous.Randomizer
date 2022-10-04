using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.PlayMaker.Action
{
	public class PlayerVerticalAttack : FsmStateAction
	{
		public override void OnEnter()
		{
			this.alreadyTriggeredAnEvent = false;
			if (Core.Logic.Penitent.VerticalAttack.Casting)
			{
				base.Fsm.Event(this.IsVerticalAttacking);
				this.alreadyTriggeredAnEvent = true;
			}
			else if (this.CheckOnlyOnce.Value)
			{
				base.Fsm.Event(this.IsNotVerticalAttacking);
			}
		}

		public override void OnUpdate()
		{
			if (!this.CheckOnlyOnce.Value && !this.alreadyTriggeredAnEvent && Core.Logic.Penitent.VerticalAttack.Casting)
			{
				base.Fsm.Event(this.IsVerticalAttacking);
				this.alreadyTriggeredAnEvent = true;
			}
		}

		public FsmBool CheckOnlyOnce;

		public FsmEvent IsVerticalAttacking;

		public FsmEvent IsNotVerticalAttacking;

		private bool alreadyTriggeredAnEvent;
	}
}
