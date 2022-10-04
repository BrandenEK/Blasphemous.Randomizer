using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BurntFace.AI
{
	public class BurntFaceSt_Death : State<BurntFaceBehaviour>
	{
		public override void Enter(BurntFaceBehaviour owner)
		{
			owner.StartDeathSequence();
		}

		public override void Execute(BurntFaceBehaviour owner)
		{
		}

		public override void Exit(BurntFaceBehaviour owner)
		{
		}
	}
}
