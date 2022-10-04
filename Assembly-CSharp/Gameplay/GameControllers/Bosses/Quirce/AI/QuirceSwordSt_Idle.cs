using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.Quirce.AI
{
	public class QuirceSwordSt_Idle : State<QuirceSwordBehaviour>
	{
		public override void Enter(QuirceSwordBehaviour owner)
		{
		}

		public override void Execute(QuirceSwordBehaviour owner)
		{
			if (owner.doFollow)
			{
				owner.UpdateFloatingOffset();
				owner.SetTargetPosition();
				owner.SetTargetRotation();
				owner.ApplyPosition();
				owner.ApplyRotation();
			}
		}

		public override void Exit(QuirceSwordBehaviour owner)
		{
		}
	}
}
