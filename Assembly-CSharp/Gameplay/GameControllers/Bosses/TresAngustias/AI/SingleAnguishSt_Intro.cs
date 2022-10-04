using System;
using Maikel.StatelessFSM;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.TresAngustias.AI
{
	public class SingleAnguishSt_Intro : State<SingleAnguishBehaviour>
	{
		public override void Enter(SingleAnguishBehaviour owner)
		{
			Debug.Log("ON ENTER INTRO");
		}

		public override void Execute(SingleAnguishBehaviour owner)
		{
		}

		public override void Exit(SingleAnguishBehaviour owner)
		{
		}
	}
}
