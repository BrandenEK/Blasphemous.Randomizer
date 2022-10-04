using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.TresAngustias.AI
{
	public class SingleAnguishSt_Merged : State<SingleAnguishBehaviour>
	{
		public override void Enter(SingleAnguishBehaviour owner)
		{
			owner.ActivateSprite(false);
			owner.ActivateSteering(false);
			owner.ActivateWeapon(false);
		}

		public override void Execute(SingleAnguishBehaviour owner)
		{
		}

		public override void Exit(SingleAnguishBehaviour owner)
		{
			owner.ActivateSprite(true);
			owner.ActivateWeapon(true);
		}
	}
}
