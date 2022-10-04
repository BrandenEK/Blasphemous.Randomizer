using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Saves the game in the selected level and priedieu persistent ID")]
	public class SaveManually : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.SpawnManager.SetActivePriedieuManually(this.levelID.ToString(), this.prieDieuPersistentID.ToString());
			Core.Persistence.SaveGame(true);
			base.Finish();
		}

		public FsmString levelID;

		public FsmString prieDieuPersistentID;
	}
}
