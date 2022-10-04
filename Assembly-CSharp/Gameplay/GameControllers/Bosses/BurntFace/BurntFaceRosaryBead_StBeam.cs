using System;
using Gameplay.GameControllers.Bosses.BurntFace.Rosary;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurntFaceRosaryBead_StBeam : State<BurntFaceRosaryBead>
	{
		public override void Enter(BurntFaceRosaryBead owner)
		{
			owner.ActivateBeam();
		}

		public override void Execute(BurntFaceRosaryBead owner)
		{
			if (!owner.IsInsideActiveAngle())
			{
				owner.ChangeToActive();
			}
		}

		public override void Exit(BurntFaceRosaryBead owner)
		{
			owner.DeactivateBeam();
		}
	}
}
