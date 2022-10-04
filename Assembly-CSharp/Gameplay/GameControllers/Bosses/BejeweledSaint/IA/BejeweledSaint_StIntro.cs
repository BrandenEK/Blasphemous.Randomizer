using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.IA
{
	public class BejeweledSaint_StIntro : State<BejeweledSaintBehaviour>
	{
		public override void Enter(BejeweledSaintBehaviour owner)
		{
			owner.StartIntro();
		}

		public override void Execute(BejeweledSaintBehaviour owner)
		{
		}

		public override void Exit(BejeweledSaintBehaviour owner)
		{
			owner.OnIntroEnds();
		}
	}
}
