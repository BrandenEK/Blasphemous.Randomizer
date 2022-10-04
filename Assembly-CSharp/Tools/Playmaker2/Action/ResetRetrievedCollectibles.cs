using System;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Resets the already retrieved collectibles.")]
	public class ResetRetrievedCollectibles : FsmStateAction
	{
		public override void OnEnter()
		{
			OssuaryManager.ResetAlreadyRetrievedCollectibles();
			base.Finish();
		}
	}
}
