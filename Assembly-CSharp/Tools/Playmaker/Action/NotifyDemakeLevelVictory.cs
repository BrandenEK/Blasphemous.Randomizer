using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	public class NotifyDemakeLevelVictory : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.DemakeManager.DemakeLevelVictory();
		}
	}
}
