using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Get")]
	public class GetLifeMax : FsmStateAction
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
			this.output.Value = Core.Logic.Penitent.Stats.Life.Base + Core.Logic.Penitent.Stats.Life.Bonus;
			base.Finish();
		}

		public FsmFloat output;
	}
}
