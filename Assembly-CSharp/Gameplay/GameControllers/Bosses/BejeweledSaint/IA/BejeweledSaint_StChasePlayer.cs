using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.IA
{
	public class BejeweledSaint_StChasePlayer : State<BejeweledSaintBehaviour>
	{
		public override void Enter(BejeweledSaintBehaviour owner)
		{
		}

		public override void Execute(BejeweledSaintBehaviour owner)
		{
			if (owner.IsPlayerInStaffRange())
			{
				owner.ChangeToAction();
			}
			else
			{
				owner.Chase(owner.GetTarget());
			}
		}

		public override void Exit(BejeweledSaintBehaviour owner)
		{
		}
	}
}
