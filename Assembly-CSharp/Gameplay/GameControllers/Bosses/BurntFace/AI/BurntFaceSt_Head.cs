using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BurntFace.AI
{
	public class BurntFaceSt_Head : State<BurntFaceBehaviour>
	{
		public override void Enter(BurntFaceBehaviour owner)
		{
			owner.SetHidingLevel(0);
			owner.EnableDamage(true);
		}

		public override void Execute(BurntFaceBehaviour owner)
		{
		}

		public override void Exit(BurntFaceBehaviour owner)
		{
		}
	}
}
