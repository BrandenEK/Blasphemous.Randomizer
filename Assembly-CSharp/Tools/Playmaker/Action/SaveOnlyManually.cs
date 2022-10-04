using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Saves the game")]
	public class SaveOnlyManually : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Persistence.SaveGame(true);
			base.Finish();
		}
	}
}
