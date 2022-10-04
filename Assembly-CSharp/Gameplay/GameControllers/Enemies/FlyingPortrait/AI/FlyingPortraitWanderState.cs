using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.FlyingPortrait.AI
{
	public class FlyingPortraitWanderState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._flyingPortrait = machine.GetComponent<FlyingPortrait>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this._flyingPortrait.DamageByContact = false;
		}

		public override void Update()
		{
			base.Update();
			if (!this._flyingPortrait.Behaviour.IsAwake)
			{
				return;
			}
			this._flyingPortrait.Behaviour.Wander();
			if (!this._flyingPortrait.MotionChecker.HitsBlock && !this._flyingPortrait.MotionChecker.HitsPatrolBlock)
			{
				return;
			}
			EntityOrientation orientation = (this._flyingPortrait.Status.Orientation != EntityOrientation.Right) ? EntityOrientation.Right : EntityOrientation.Left;
			this._flyingPortrait.SetOrientation(orientation, true, false);
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private FlyingPortrait _flyingPortrait;
	}
}
