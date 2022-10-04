using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.FlyingPortrait.AI
{
	public class FlyingPortraitDeathState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._flyingPortrait = machine.GetComponent<FlyingPortrait>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			if (this._flyingPortrait.Status.Dead)
			{
				this._flyingPortrait.AnimatorInjector.Death();
			}
		}

		private FlyingPortrait _flyingPortrait;
	}
}
