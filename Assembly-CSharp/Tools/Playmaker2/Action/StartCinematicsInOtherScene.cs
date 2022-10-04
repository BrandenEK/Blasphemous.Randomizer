using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Start a playmaker cinematics in other scene.")]
	public class StartCinematicsInOtherScene : FsmStateAction
	{
		public override void Reset()
		{
			if (this.LevelName == null)
			{
				this.LevelName = new FsmString();
				this.LevelName.Value = string.Empty;
			}
			if (this.PlayMakerEventName == null)
			{
				this.PlayMakerEventName = new FsmString();
				this.PlayMakerEventName.Value = string.Empty;
			}
			if (this.HidePlayer == null)
			{
				this.HidePlayer = new FsmBool();
				this.HidePlayer.Value = true;
			}
		}

		public override void OnEnter()
		{
			base.OnEnter();
			string levelName = (this.LevelName == null) ? string.Empty : this.LevelName.Value;
			string eventName = (this.PlayMakerEventName == null) ? string.Empty : this.PlayMakerEventName.Value;
			bool hideplayer = this.HidePlayer == null || this.HidePlayer.Value;
			Core.LevelManager.ChangeLevelAndPlayEvent(levelName, eventName, hideplayer, true, false, null, false);
			base.Finish();
		}

		public FsmString LevelName;

		public FsmString PlayMakerEventName;

		public FsmBool HidePlayer;
	}
}
