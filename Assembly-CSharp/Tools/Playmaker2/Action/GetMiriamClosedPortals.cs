using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Get")]
	public class GetMiriamClosedPortals : FsmStateAction
	{
		public override void Reset()
		{
			this.output = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			this.output.Value = Core.Events.GetMiriamClosedPortals().Count;
			base.Finish();
		}

		public FsmInt output;
	}
}
