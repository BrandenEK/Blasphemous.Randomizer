using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.Quirce.AI
{
	public class QuirceSwordSt_SpinToPoint : State<QuirceSwordBehaviour>
	{
		public override void Enter(QuirceSwordBehaviour owner)
		{
			owner.ResetRotation();
			owner.SetSpeedFactor(0.05f);
		}

		public override void Execute(QuirceSwordBehaviour owner)
		{
			owner.ApplyPosition();
			owner.CheckCollision();
			if (owner.IsCloseToPoint())
			{
				owner.ReturnToSpinFollow();
			}
		}

		public override void Exit(QuirceSwordBehaviour owner)
		{
			owner.SetNormalSpeed();
		}
	}
}
