using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.TresAngustias.AI
{
	public class SingleAnguishSt_Action : State<SingleAnguishBehaviour>
	{
		public override void Enter(SingleAnguishBehaviour owner)
		{
			owner.SetScroll(true);
		}

		public override void Execute(SingleAnguishBehaviour owner)
		{
		}

		public override void Exit(SingleAnguishBehaviour owner)
		{
			owner.SetScroll(false);
		}
	}
}
