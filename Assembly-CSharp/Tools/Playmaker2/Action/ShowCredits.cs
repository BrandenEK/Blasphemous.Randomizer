using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Play credits scene.")]
	public class ShowCredits : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.LoadCreditsScene(this.useFade);
		}

		public bool useFade;
	}
}
