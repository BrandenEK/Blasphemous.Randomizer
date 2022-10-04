using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BurntFace.AI
{
	public class BurntFaceSt_Intro : State<BurntFaceBehaviour>
	{
		public override void Enter(BurntFaceBehaviour owner)
		{
			owner.StartIntroSequence();
		}

		public override void Execute(BurntFaceBehaviour owner)
		{
		}

		public override void Exit(BurntFaceBehaviour owner)
		{
		}
	}
}
