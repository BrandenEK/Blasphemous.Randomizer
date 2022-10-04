using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.IA
{
	public class BejeweledSaint_StMoveToPoint : State<BejeweledSaintBehaviour>
	{
		public override void Enter(BejeweledSaintBehaviour owner)
		{
		}

		public override void Execute(BejeweledSaintBehaviour owner)
		{
			if (owner.IsCloseToPoint(owner.movePoint))
			{
				owner.ChangeToAction();
			}
			else
			{
				owner.MoveTowards(owner.movePoint);
			}
		}

		public override void Exit(BejeweledSaintBehaviour owner)
		{
		}
	}
}
