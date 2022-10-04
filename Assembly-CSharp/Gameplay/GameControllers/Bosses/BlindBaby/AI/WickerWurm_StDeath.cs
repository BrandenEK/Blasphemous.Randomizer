using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BlindBaby.AI
{
	public class WickerWurm_StDeath : State<WickerWurmBehaviour>
	{
		public override void Enter(WickerWurmBehaviour owner)
		{
			owner.StartDeathSequence();
		}

		public override void Execute(WickerWurmBehaviour owner)
		{
		}

		public override void Exit(WickerWurmBehaviour owner)
		{
		}
	}
}
