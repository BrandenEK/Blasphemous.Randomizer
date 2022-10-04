using System;
using Maikel.StatelessFSM;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace.AI
{
	public class BurntFaceSt_Hidden : State<BurntFaceBehaviour>
	{
		public override void Enter(BurntFaceBehaviour owner)
		{
			Debug.Log("ENTER STATE: St_HIDDEN");
			owner.SetHidingLevel(2);
			owner.ShowEyes(false);
		}

		public override void Execute(BurntFaceBehaviour owner)
		{
		}

		public override void Exit(BurntFaceBehaviour owner)
		{
		}
	}
}
