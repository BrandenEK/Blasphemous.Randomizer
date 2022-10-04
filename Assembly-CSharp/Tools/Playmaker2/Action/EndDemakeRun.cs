using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Ends Demake Run.")]
	public class EndDemakeRun : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.DemakeManager.EndDemakeRun(true, this.numSpecialItems.Value);
			base.Finish();
		}

		public FsmInt numSpecialItems;
	}
}
