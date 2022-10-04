using System;
using Gameplay.GameControllers.Bosses.BurntFace.Rosary;
using Maikel.StatelessFSM;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurntFaceRosaryBead_StProjectile : State<BurntFaceRosaryBead>
	{
		public override void Enter(BurntFaceRosaryBead owner)
		{
			owner.ActivateTurret(true);
			Debug.Log("ACTIVATE ROSARY TURRET");
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
			owner.ActivateTurret(false);
			Debug.Log("DEACTIVATE ROSARY TURRET");
		}
	}
}
