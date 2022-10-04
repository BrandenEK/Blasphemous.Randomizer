using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Get")]
	public class GetLife : FsmStateAction
	{
		public override void Reset()
		{
			this.output = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			this.output.Value = Core.Logic.Penitent.Stats.Life.Current;
			base.Finish();
		}

		public FsmFloat output;
	}
}
