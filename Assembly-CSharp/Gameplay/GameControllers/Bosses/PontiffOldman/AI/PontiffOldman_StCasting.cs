using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.PontiffOldman.AI
{
	public class PontiffOldman_StCasting : State<PontiffOldmanBehaviour>
	{
		public override void Enter(PontiffOldmanBehaviour owner)
		{
			owner.PontiffOldman.IsGuarding = true;
		}

		public override void Execute(PontiffOldmanBehaviour owner)
		{
		}

		public override void Exit(PontiffOldmanBehaviour owner)
		{
			owner.PontiffOldman.IsGuarding = false;
		}
	}
}
