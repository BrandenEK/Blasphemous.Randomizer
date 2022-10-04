using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua.AI
{
	public class Perpetua_StFollowPlayer : State<PerpetuaBehaviour>
	{
		public override void Enter(PerpetuaBehaviour owner)
		{
			owner.ResetVelocity();
			owner.ChooseSide();
		}

		public override void Execute(PerpetuaBehaviour owner)
		{
			owner.FollowPlayer();
		}

		public override void Exit(PerpetuaBehaviour owner)
		{
		}
	}
}
