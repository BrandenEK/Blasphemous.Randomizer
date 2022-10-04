using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BlindBaby.AI
{
	public class WickerWurm_StStun : State<WickerWurmBehaviour>
	{
		public override void Enter(WickerWurmBehaviour owner)
		{
			owner.EnterStun();
		}

		public override void Execute(WickerWurmBehaviour owner)
		{
		}

		public override void Exit(WickerWurmBehaviour owner)
		{
			owner.ExitStun();
		}
	}
}
