using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.TresAngustias.AI
{
	public class SingleAnguishSt_Dance : State<SingleAnguishBehaviour>
	{
		public override void Enter(SingleAnguishBehaviour owner)
		{
		}

		public override void Execute(SingleAnguishBehaviour owner)
		{
			owner.UpdateDanceState();
		}

		public override void Exit(SingleAnguishBehaviour owner)
		{
		}
	}
}
