using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.Quirce.AI
{
	public class QuirceSwordSt_Spinning : State<QuirceSwordBehaviour>
	{
		public override void Enter(QuirceSwordBehaviour owner)
		{
			owner.ResetRotation();
		}

		public override void Execute(QuirceSwordBehaviour owner)
		{
			if (owner.doFollow)
			{
				owner.UpdateFloatingOffset();
				owner.SetTargetPosition();
				owner.ApplyPosition();
			}
			owner.CheckCollision();
		}

		public override void Exit(QuirceSwordBehaviour owner)
		{
		}
	}
}
