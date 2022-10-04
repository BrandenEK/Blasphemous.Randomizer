using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Allows or blocks the input from the player.")]
	public class InputBlock : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Input.SetBlocker(this.inputBlockName.Value, this.active.Value);
			base.Finish();
		}

		public FsmString inputBlockName;

		public FsmBool active;
	}
}
