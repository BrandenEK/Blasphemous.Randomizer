using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("End cinematics started by StartCinematicsInOtherScene.")]
	public class EndCinematicsInOtherScene : FsmStateAction
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Core.LevelManager.RestoreFromChangeLevelAndPlayEvent(true, null);
			base.Finish();
		}
	}
}
