using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Sets purge points.")]
	public class PurgeSet : FsmStateAction
	{
		public override void OnEnter()
		{
			float num = this.value.Value;
			if (num < 0f)
			{
				num = 0f;
			}
			Core.Logic.Penitent.Stats.Purge.Current = num;
			base.Finish();
		}

		public FsmFloat value;
	}
}
