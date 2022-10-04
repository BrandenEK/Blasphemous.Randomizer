using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Deprecated
{
	[ActionCategory("Blasphemous Deprecated")]
	[Tooltip("Sets the value of a flag.")]
	public class SetFlag : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Events.SetFlag(this.flagName.Value, this.state.Value, false);
			base.Finish();
		}

		public FsmString flagName;

		public FsmBool state;

		public bool runtimeFlag;
	}
}
