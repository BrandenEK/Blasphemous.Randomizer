using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.TresAngustias.AI
{
	public class SingleAnguishSt_GoToDance : State<SingleAnguishBehaviour>
	{
		public override void Enter(SingleAnguishBehaviour owner)
		{
			owner.ActivateSteering(true);
			owner.ActivateGhostMode(true);
		}

		public override void Execute(SingleAnguishBehaviour owner)
		{
			owner.UpdateGoToDancePointState();
			if (owner.IsCloseToTargetPoint(0.25f))
			{
				owner.ChangeToDance();
			}
		}

		public override void Exit(SingleAnguishBehaviour owner)
		{
			owner.ActivateSteering(false);
			owner.ActivateGhostMode(false);
		}
	}
}
