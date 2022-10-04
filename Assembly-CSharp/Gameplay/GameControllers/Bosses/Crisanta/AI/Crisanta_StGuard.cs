using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.Crisanta.AI
{
	public class Crisanta_StGuard : State<CrisantaBehaviour>
	{
		public override void Enter(CrisantaBehaviour owner)
		{
			owner.Crisanta.IsGuarding = true;
		}

		public override void Execute(CrisantaBehaviour owner)
		{
		}

		public override void Exit(CrisantaBehaviour owner)
		{
			owner.Crisanta.IsGuarding = false;
		}
	}
}
