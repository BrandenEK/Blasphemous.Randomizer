using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BlindBaby.AI
{
	public class WickerWurm_StFixed : State<WickerWurmBehaviour>
	{
		public override void Enter(WickerWurmBehaviour owner)
		{
			owner.AffixBody(true);
			owner.lookAtPlayer = true;
		}

		public override void Execute(WickerWurmBehaviour owner)
		{
		}

		public override void Exit(WickerWurmBehaviour owner)
		{
			owner.AffixBody(false);
			owner.lookAtPlayer = false;
		}
	}
}
