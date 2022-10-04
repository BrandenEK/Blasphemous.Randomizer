using System;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Counts the number of already retrieved collectibles.")]
	public class CountRetrievedCollectibles : FsmStateAction
	{
		public override void Reset()
		{
			this.output = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			this.output.Value = OssuaryManager.CountAlreadyRetrievedCollectibles();
			base.Finish();
		}

		public FsmInt output;
	}
}
