using System;
using Gameplay.GameControllers.Bosses.BurntFace.Rosary;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurntFaceRosaryBead_StInactive : State<BurntFaceRosaryBead>
	{
		public override void Enter(BurntFaceRosaryBead owner)
		{
			if (owner.currentType == BurntFaceRosaryBead.ROSARY_BEAD_TYPE.BEAM)
			{
				owner.DeactivateBeam();
			}
			else
			{
				owner.ActivateTurret(false);
			}
		}

		public override void Execute(BurntFaceRosaryBead owner)
		{
		}

		public override void Exit(BurntFaceRosaryBead owner)
		{
		}
	}
}
