using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BurntFace.AI
{
	public class BurntFaceSt_Eyes : State<BurntFaceBehaviour>
	{
		public override void Enter(BurntFaceBehaviour owner)
		{
			owner.SetHidingLevel(1);
			owner.EnableDamage(false);
			owner.ShowEyes(true);
		}

		public override void Execute(BurntFaceBehaviour owner)
		{
		}

		public override void Exit(BurntFaceBehaviour owner)
		{
		}
	}
}
