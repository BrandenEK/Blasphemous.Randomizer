using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Start Miriam Quest.")]
	public class MiriamQuestStart : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Events.StartMiriamQuest();
			base.Finish();
		}
	}
}
