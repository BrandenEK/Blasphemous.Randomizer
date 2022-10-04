using System;
using Gameplay.GameControllers.Bosses.BurntFace.Rosary;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurntFaceRosaryBead_StActive : State<BurntFaceRosaryBead>
	{
		public override void Enter(BurntFaceRosaryBead owner)
		{
		}

		public override void Execute(BurntFaceRosaryBead owner)
		{
			if (owner.IsInsideActiveAngle())
			{
				owner.ChangeToCharging();
			}
		}

		public override void Exit(BurntFaceRosaryBead owner)
		{
		}
	}
}
