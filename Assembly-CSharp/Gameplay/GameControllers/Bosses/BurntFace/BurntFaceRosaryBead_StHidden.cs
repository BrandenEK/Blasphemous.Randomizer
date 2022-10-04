using System;
using Gameplay.GameControllers.Bosses.BurntFace.Rosary;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurntFaceRosaryBead_StHidden : State<BurntFaceRosaryBead>
	{
		public override void Enter(BurntFaceRosaryBead owner)
		{
			owner.PlayHideAnimation();
		}

		public override void Execute(BurntFaceRosaryBead owner)
		{
		}

		public override void Exit(BurntFaceRosaryBead owner)
		{
			owner.PlayShowAnimation();
		}
	}
}
