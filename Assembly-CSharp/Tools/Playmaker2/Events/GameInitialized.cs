using System;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Event")]
	[Tooltip("Event raised on game initialization. DONT USE")]
	public class GameInitialized : FsmStateAction
	{
		public override void OnEnter()
		{
		}

		public override void OnUpdate()
		{
		}

		public override void OnExit()
		{
		}

		public FsmEvent start;

		public FsmEvent debug;

		public FsmEvent destroy;
	}
}
