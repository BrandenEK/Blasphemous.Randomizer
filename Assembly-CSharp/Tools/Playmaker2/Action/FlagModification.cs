using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Modifies the value of a flag.")]
	public class FlagModification : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Events.SetFlag(this.flagName.Value, this.state.Value, false);
			base.Finish();
		}

		public FsmString category;

		public FsmString flagName;

		public FsmBool state;

		public bool runtimeFlag;
	}
}
