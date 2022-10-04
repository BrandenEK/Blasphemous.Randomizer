using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	public class CompletitionPercentageAdd : FsmStateAction
	{
		public override void Reset()
		{
			this.percentageType = PersistentManager.PercentageType.BossDefeated_1;
			this.CustomValue = 0f;
		}

		public override void OnEnter()
		{
			base.Finish();
		}

		public PersistentManager.PercentageType percentageType;

		public float CustomValue;
	}
}
